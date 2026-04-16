using System.Diagnostics;

namespace TalageIntegration.API.Middleware;

public static class CorrelationIdConstants
{
    public const string HeaderName = "X-Correlation-Id";
    public const string ItemKey = "CorrelationId";
}

public sealed class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context);

        context.Items[CorrelationIdConstants.ItemKey] = correlationId;
        context.TraceIdentifier = correlationId;
        context.Response.Headers.TryAdd(CorrelationIdConstants.HeaderName, correlationId);

        var userAgent = context.Request.Headers.UserAgent.ToString();
        var requestId = Activity.Current?.Id;

        using (logger.BeginScope(new Dictionary<string, object?>
               {
                   ["ApplicationName"] = "TalageIntegration.API",
                   ["CorrelationId"] = correlationId,
                   ["UserAgent"] = string.IsNullOrWhiteSpace(userAgent) ? null : userAgent,
                   ["RequestId"] = requestId
               }))
        {
            await next(context);
        }
    }

    private static string ResolveCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdConstants.HeaderName, out var values))
        {
            var headerValue = values.ToString().Trim();
            if (Guid.TryParse(headerValue, out var parsed))
            {
                return parsed.ToString("D");
            }
        }

        return Guid.NewGuid().ToString("D");
    }
}

