using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Talage.SDK.Models;
using Talage.SDK.Internal.Interfaces;
using Talage.SDK.Internal.ApiClient;
using TalageIntegration.Shared.Exceptions;

namespace Talage.SDK.Internal.Auth;

public sealed class TelangeService(
    HttpClient httpClient,
    IOptions<TalageApiOptions> options,
    ILogger<TelangeService> logger) : ITelangeService
{
    public async Task<AuthenticationResponseDto> GetTokenAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Talage auth token request started.");

        var apiOptions = options.Value;

        if (string.IsNullOrWhiteSpace(apiOptions.ApiKey) || string.IsNullOrWhiteSpace(apiOptions.ApiSecret))
        {
            throw new TalageConfigurationException("Talage API apiKey/apiSecret are not configured.");
        }

        using var message = new HttpRequestMessage(HttpMethod.Post, "auth/keys")
        {
            Content = JsonContent.Create(new
            {
                apiKey = apiOptions.ApiKey,
                apiSecret = apiOptions.ApiSecret
            })
        };

        using var response = await httpClient.SendAsync(message, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw TalageApiException.Create("Talage authentication failed.", response.StatusCode, responseBody);
        }

        var token = ExtractToken(responseBody);

        logger.LogInformation("Talage auth token request completed.");

        return new AuthenticationResponseDto
        {
            Status = "Created",
            Token = token,
            ExpiresAt = DateTimeOffset.Now.AddMinutes(apiOptions.TokenLifetimeMinutes)
        };
    }

    private static string ExtractToken(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            throw new TalageApiException("Talage authentication response was empty.", 502, responseBody, mappedStatusCode: System.Net.HttpStatusCode.BadGateway);
        }

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        if (root.TryGetProperty("token", out var tokenElement))
        {
            var token = tokenElement.GetString();
            if (!string.IsNullOrWhiteSpace(token))
            {
                return token;
            }
        }

        // Some environments may vary casing or wrap token in another object. Best-effort scan.
        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in root.EnumerateObject())
            {
                if (property.NameEquals("token") || property.Name.Equals("token", StringComparison.OrdinalIgnoreCase))
                {
                    var token = property.Value.GetString();
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        return token;
                    }
                }
            }
        }

        throw new TalageApiException("Talage authentication response did not include a token.", 502, responseBody, mappedStatusCode: System.Net.HttpStatusCode.BadGateway);
    }
}

