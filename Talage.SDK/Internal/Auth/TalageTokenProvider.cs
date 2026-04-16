using Talage.SDK.Internal.Interfaces;

namespace Talage.SDK.Internal.Auth;

public sealed class TalageTokenProvider(
    ITalageTokenManager tokenManager) : ITalageTokenProvider
{
    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        var response = await tokenManager.GetOrCreateAsync(cancellationToken);
        return response.Token;
    }
}

