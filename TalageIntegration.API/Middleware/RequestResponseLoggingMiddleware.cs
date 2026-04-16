using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TalageIntegration.API.Middleware;

public sealed class RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
{
    private const int MaxBodyChars = 32_768;
    private const int MaxCapturedResponseBodyBytes = 64 * 1024;
    private const int MaxHeaderValueChars = 2048;
    private const int MaxHeadersToLog = 64;

    private static readonly Regex QuerySensitiveValue =
        new("(?<prefix>[\\?&](?<key>apiKey|apiSecret|token|access_token|refresh_token|refreshToken|authorization|client_secret)=)(?<value>[^&]*)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex JsonKeyStringValue =
        new(
            "(\"(?<key>password|passcode|apiKey|apiSecret|token|access_token|refresh_token|refreshToken|authorization|client_secret)\"\\s*:\\s*\")(?<value>.*?)(\")",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? string.Empty;
        var safeQueryString = RedactQueryString(context.Request.QueryString.Value);

        var includeHeaders = logger.IsEnabled(LogLevel.Debug);

        string? requestBody = null;
        string? responseBody = null;
        Dictionary<string, string?>? headers = null;
        Exception? unhandledException = null;
        var cancelled = false;

        try
        {
            if (includeHeaders)
            {
                headers = GetSafeHeaders(context.Request.Headers);
            }

            requestBody = await TryReadRequestBodyAsync(context);

            var originalBody = context.Response.Body;
            await using var captureStream = new MemoryStream(capacity: Math.Min(MaxCapturedResponseBodyBytes, 16 * 1024));
            await using var teeStream = new TeeResponseStream(originalBody, captureStream, MaxCapturedResponseBodyBytes);
            context.Response.Body = teeStream;

            try
            {
                await next(context);
            }
            finally
            {
                responseBody = await TryReadCapturedResponseBodyAsync(context, captureStream, teeStream.IsTruncated);
                context.Response.Body = originalBody;
            }
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            cancelled = true;
        }
        catch (Exception ex)
        {
            unhandledException = ex;
            logger.LogError(
                ex,
                "HTTP {Method} {Path} failed in {ElapsedMilliseconds} ms. Request: {RequestBody} Response: {ResponseBody} Status: {StatusCode}",
                method,
                path,
                stopwatch.ElapsedMilliseconds,
                requestBody,
                responseBody,
                context.Response.StatusCode);

            throw;
        }
        finally
        {
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }

            if (headers is not null)
            {
                logger.LogDebug(
                    "HTTP {Method} {Path} request headers: {@Headers}",
                    method,
                    path,
                    headers);
            }

            if (cancelled)
            {
                logger.LogInformation(
                    "HTTP {Method} {Path} request cancelled after {ElapsedMilliseconds} ms",
                    method,
                    path,
                    stopwatch.ElapsedMilliseconds);
            }
            else if (unhandledException is null)
            {
                logger.LogInformation(
                    "HTTP {Method} {Path} Request: {RequestBody} Response: {ResponseBody} Status: {StatusCode} Time: {ElapsedMilliseconds} ms",
                    method,
                    path,
                    requestBody,
                    responseBody,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }

        if (cancelled)
        {
            return;
        }
    }

    private static Dictionary<string, string?> GetSafeHeaders(IHeaderDictionary headers)
    {
        var result = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        var headerCount = 0;
        foreach (var (key, values) in headers)
        {
            if (headerCount >= MaxHeadersToLog)
            {
                result["[HeadersTruncated]"] = $"Logged first {MaxHeadersToLog} headers.";
                break;
            }

            if (key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)
                || key.Equals("Cookie", StringComparison.OrdinalIgnoreCase)
                || key.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
            {
                result[key] = "***REDACTED***";
                headerCount++;
                continue;
            }

            var value = values.ToString();
            if (value.Length > MaxHeaderValueChars)
            {
                value = value[..MaxHeaderValueChars] + "...[truncated]";
            }

            result[key] = value;
            headerCount++;
        }

        return result;
    }

    private static bool ShouldReadRequestBody(HttpContext context)
    {
        if (HttpMethods.IsGet(context.Request.Method)
            || HttpMethods.IsHead(context.Request.Method)
            || HttpMethods.IsDelete(context.Request.Method)
            || HttpMethods.IsOptions(context.Request.Method))
        {
            return false;
        }

        return true;
    }

    private static async Task<string?> TryReadRequestBodyAsync(HttpContext context)
    {
        if (!ShouldReadRequestBody(context))
        {
            return null;
        }

        if (context.Request.Body is null)
        {
            return null;
        }

        var contentType = context.Request.ContentType;
        if (!IsTextBasedContentType(contentType))
        {
            return $"[Skipped body logging for content-type '{contentType}']";
        }

        context.Request.EnableBuffering();
        context.Request.Body.Position = 0;

        var bodyText = await ReadStreamUpToAsync(context.Request.Body, context.RequestAborted);
        context.Request.Body.Position = 0;

        bodyText = RedactSensitive(bodyText);
        bodyText = TryNormalizeJson(bodyText, contentType);

        return bodyText;
    }

    private static async Task<string?> TryReadCapturedResponseBodyAsync(
        HttpContext context,
        MemoryStream captureStream,
        bool isTruncated)
    {
        if (captureStream.Length == 0)
        {
            return null;
        }

        var contentType = context.Response.ContentType;
        if (!IsTextBasedContentType(contentType))
        {
            return $"[Skipped body logging for content-type '{contentType}']";
        }

        captureStream.Position = 0;
        var bodyText = await ReadStreamUpToAsync(captureStream, context.RequestAborted);
        captureStream.Position = captureStream.Length;

        bodyText = RedactSensitive(bodyText);
        bodyText = TryNormalizeJson(bodyText, contentType);

        if (isTruncated)
        {
            bodyText += "...[truncated]";
        }

        return bodyText;
    }

    private static string TryNormalizeJson(string body, string? contentType)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return body;
        }

        var looksJson = (contentType?.Contains("json", StringComparison.OrdinalIgnoreCase) ?? false)
                        || body.TrimStart().StartsWith('{')
                        || body.TrimStart().StartsWith('[');
        if (!looksJson)
        {
            return body;
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            return JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = false });
        }
        catch
        {
            return body;
        }
    }

    private static bool IsTextBasedContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        contentType = contentType.ToLowerInvariant();

        if (contentType.StartsWith("multipart/") || contentType.Contains("application/octet-stream"))
        {
            return false;
        }

        return contentType.StartsWith("text/")
               || contentType.Contains("application/json")
               || contentType.Contains("application/problem+json")
               || contentType.Contains("application/xml")
               || contentType.Contains("application/x-www-form-urlencoded")
               || contentType.Contains("+json");
    }

    private static async Task<string> ReadStreamUpToAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true);
        var builder = new StringBuilder();
        var buffer = new char[4096];

        while (builder.Length < MaxBodyChars)
        {
            var remaining = Math.Min(buffer.Length, MaxBodyChars - builder.Length);
            var read = await reader.ReadAsync(buffer.AsMemory(0, remaining), cancellationToken);
            if (read <= 0)
            {
                break;
            }

            builder.Append(buffer, 0, read);
        }

        if (builder.Length >= MaxBodyChars)
        {
            builder.Append("...[truncated]");
        }

        return builder.ToString();
    }

    private static string RedactSensitive(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        return JsonKeyStringValue.Replace(input, "${1}***REDACTED***${3}");
    }

    private static string? RedactQueryString(string? queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
        {
            return queryString;
        }

        return QuerySensitiveValue.Replace(queryString, "${prefix}***REDACTED***");
    }

    private sealed class TeeResponseStream : Stream
    {
        private readonly Stream _inner;
        private readonly MemoryStream _capture;
        private readonly int _maxCaptureBytes;
        private int _capturedBytes;

        public TeeResponseStream(Stream inner, MemoryStream capture, int maxCaptureBytes)
        {
            _inner = inner;
            _capture = capture;
            _maxCaptureBytes = maxCaptureBytes;
        }

        public bool IsTruncated { get; private set; }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush() => _inner.Flush();
        public override Task FlushAsync(CancellationToken cancellationToken) => _inner.FlushAsync(cancellationToken);
        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
        public override void SetLength(long value) => _inner.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            _inner.Write(buffer, offset, count);
            Capture(buffer.AsSpan(offset, count));
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _inner.WriteAsync(buffer, offset, count, cancellationToken);
            Capture(buffer.AsSpan(offset, count));
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await _inner.WriteAsync(buffer, cancellationToken);
            Capture(buffer.Span);
        }

        private void Capture(ReadOnlySpan<byte> bytes)
        {
            if (IsTruncated || _maxCaptureBytes <= 0)
            {
                IsTruncated = true;
                return;
            }

            if (_capturedBytes >= _maxCaptureBytes)
            {
                IsTruncated = true;
                return;
            }

            var remainingBytes = _maxCaptureBytes - _capturedBytes;
            var toWrite = Math.Min(remainingBytes, bytes.Length);

            if (toWrite > 0)
            {
                _capture.Write(bytes[..toWrite]);
                _capturedBytes += toWrite;
            }

            if (toWrite < bytes.Length)
            {
                IsTruncated = true;
            }
        }
    }
}
