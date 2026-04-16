using System.Net;
using System.Text.Json;
using TalageIntegration.Shared.Models;

namespace Talage.SDK.Internal.Auth;

public sealed class TalageApiException : Exception
{
    public TalageApiException(
        string message,
        int statusCode,
        string? responseBody = null,
        string? upstreamCode = null,
        string? upstreamMessage = null,
        HttpStatusCode? mappedStatusCode = null) : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        UpstreamCode = upstreamCode;
        UpstreamMessage = upstreamMessage;
        MappedStatusCode = mappedStatusCode ?? MapStatusCode(statusCode);
    }

    public int StatusCode { get; }

    public string? ResponseBody { get; }

    public string? UpstreamCode { get; }

    public string? UpstreamMessage { get; }

    public HttpStatusCode MappedStatusCode { get; }

    public static TalageApiException Create(string defaultMessage, HttpStatusCode statusCode, string? responseBody)
    {
        TalageErrorResponse? parsed = null;

        if (!string.IsNullOrWhiteSpace(responseBody))
        {
            try
            {
                parsed = JsonSerializer.Deserialize<TalageErrorResponse>(responseBody, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }
            catch (JsonException)
            {
            }
        }

        var message = parsed?.Message ?? defaultMessage;
        return new TalageApiException(message, (int)statusCode, responseBody, parsed?.Code, parsed?.Message);
    }

    private static HttpStatusCode MapStatusCode(int statusCode) => statusCode switch
    {
        400 => HttpStatusCode.BadRequest,
        403 => HttpStatusCode.Forbidden,
        404 => HttpStatusCode.NotFound,
        500 => HttpStatusCode.BadGateway,
        _ => statusCode >= 500 ? HttpStatusCode.BadGateway : (HttpStatusCode)statusCode
    };
}

