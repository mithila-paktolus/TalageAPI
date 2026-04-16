using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talage.SDK.Models;
using Talage.SDK.Internal.Interfaces;
using Talage.SDK.Internal.Auth;
using TalageIntegration.Shared.Models;

namespace TalageIntegration.API.Controllers;

[ApiController]
[Route("api/authentication")]
public sealed class AuthenticationController(ITalageTokenManager tokenManager) : ControllerBase
{
    [HttpPost("talage/token")]
    [ProducesResponseType(typeof(ApiEnvelope<AuthenticationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetToken(CancellationToken cancellationToken)
    {
        var response = await tokenManager.GetOrCreateAsync(cancellationToken);
        return Ok(ApiEnvelope<AuthenticationResponseDto>.Ok(response, "Talage token generated."));
    }
}

