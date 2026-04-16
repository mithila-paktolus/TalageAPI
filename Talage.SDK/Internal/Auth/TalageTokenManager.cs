using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Talage.SDK.Models;
using Talage.SDK.Internal.Interfaces;
using TalageIntegration.Shared.Exceptions;

namespace Talage.SDK.Internal.Auth;

public sealed class TalageTokenManager(
    ITelangeService telangeService,
    IOptions<TalageApiOptions> options,
    IAccessTokenStore accessTokenStore,
    ILogger<TalageTokenManager> logger) : ITalageTokenManager
{
    private static readonly TimeSpan RefreshBuffer = TimeSpan.FromMinutes(1);

    private readonly SemaphoreSlim _lock = new(1, 1);

    public async Task<AuthenticationResponseDto> GetOrCreateAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var now = DateTimeOffset.Now;
            var active = await accessTokenStore.GetActiveAsync(cancellationToken);

            if (active is not null)
            {
                if (active.Value.Expires > now.Add(RefreshBuffer) && !string.IsNullOrWhiteSpace(active.Value.Token))
                {
                    logger.LogInformation("Reusing active Talage token from DB. {Expires}", active.Value.Expires);

                    return new AuthenticationResponseDto
                    {
                        Status = "Reused",
                        Token = active.Value.Token,
                        ExpiresAt = active.Value.Expires
                    };
                }

                logger.LogInformation("Stored Talage token expired. {Expires}", active.Value.Expires);
                await accessTokenStore.DeactivateAllAsync(cancellationToken);
            }

            var apiOptions = options.Value;
            if (string.IsNullOrWhiteSpace(apiOptions.ApiKey) || string.IsNullOrWhiteSpace(apiOptions.ApiSecret))
            {
                throw new TalageConfigurationException("Talage API apiKey/apiSecret are not configured.");
            }

            logger.LogInformation("Requesting new Talage token from auth API.");

            var tokenResponse = await telangeService.GetTokenAsync(cancellationToken);
            var expires = tokenResponse.ExpiresAt == default
                ? now.AddMinutes(apiOptions.TokenLifetimeMinutes)
                : tokenResponse.ExpiresAt;

            await accessTokenStore.ReplaceActiveAsync(tokenResponse.Token, "Bearer", expires, cancellationToken);

            logger.LogInformation("Saved Talage token to DB. {Expires}", expires);

            return new AuthenticationResponseDto
            {
                Status = tokenResponse.Status,
                Token = tokenResponse.Token,
                ExpiresAt = expires
            };
        }
        finally
        {
            _lock.Release();
        }
    }
}

