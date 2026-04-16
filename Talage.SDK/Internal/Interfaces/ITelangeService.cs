using Talage.SDK.Models;

namespace Talage.SDK.Internal.Interfaces;

public interface ITelangeService
{
    Task<AuthenticationResponseDto> GetTokenAsync(CancellationToken cancellationToken);
}


