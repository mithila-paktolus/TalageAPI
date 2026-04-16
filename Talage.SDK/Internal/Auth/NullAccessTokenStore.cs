using TalageIntegration.Shared.Exceptions;

namespace Talage.SDK.Internal.Auth;

public sealed class NullAccessTokenStore : IAccessTokenStore
{
    private static TalageConfigurationException MissingConnectionString() =>
        new("Token persistence is not configured. Set ConnectionStrings:AppLog to your TalengeIntegration database.");

    public Task<(string Token, string TokenType, DateTimeOffset Expires)?> GetActiveAsync(CancellationToken cancellationToken) =>
        throw MissingConnectionString();

    public Task ReplaceActiveAsync(string token, string tokenType, DateTimeOffset expires, CancellationToken cancellationToken) =>
        throw MissingConnectionString();

    public Task DeactivateAllAsync(CancellationToken cancellationToken) =>
        throw MissingConnectionString();
}


