using Talage.SDK.Models;

namespace Talage.SDK.Internal.Auth;

public interface ITalageTokenManager
{
    Task<AuthenticationResponseDto> GetOrCreateAsync(CancellationToken cancellationToken);
}


