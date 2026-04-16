namespace Talage.SDK.Internal.Auth;

public interface IAccessTokenStore
{
    Task<(string Token, string TokenType, DateTimeOffset Expires)?> GetActiveAsync(CancellationToken cancellationToken);

    Task ReplaceActiveAsync(string token, string tokenType, DateTimeOffset expires, CancellationToken cancellationToken);

    Task DeactivateAllAsync(CancellationToken cancellationToken);
}


