namespace Talage.SDK.Internal.Auth;

public interface ITalageTokenProvider
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
}

