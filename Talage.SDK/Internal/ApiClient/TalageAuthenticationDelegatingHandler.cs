using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Talage.SDK.Internal.Interfaces;
using Talage.SDK.Internal.Auth;

namespace Talage.SDK.Internal.ApiClient;

public sealed class TalageAuthenticationDelegatingHandler(
    ITalageTokenProvider tokenProvider,
    ILogger<TalageAuthenticationDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        var normalizedToken = token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? token.Substring("Bearer ".Length).Trim()
            : token.Trim();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", normalizedToken);

        logger.LogInformation(
            "Talage auth header attached. {Method} {Uri} Authorization: {Authorization}",
            request.Method.Method,
            request.RequestUri?.ToString(),
            request.Headers.Authorization?.ToString() ?? "NULL");

        return await base.SendAsync(request, cancellationToken);
    }
}

