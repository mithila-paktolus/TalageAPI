using Microsoft.AspNetCore.Mvc;
using Talage.SDK.Interfaces;
using Talage.SDK.Models;
using TalageIntegration.Shared.Models;

namespace TalageIntegration.API.Controllers;

[ApiController]
[Route("api/codes")]
public sealed class CodesController(ITalageClient talageClient) : ControllerBase
{
    [HttpGet("industry-categories")]
    [ProducesResponseType(typeof(ApiEnvelope<IReadOnlyCollection<IndustryCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIndustryCategories(CancellationToken cancellationToken)
    {
        var response = await talageClient.GetIndustryCategoriesAsync(cancellationToken);
        return Ok(ApiEnvelope<IReadOnlyCollection<IndustryCategoryDto>>.Ok(response, "Industry categories retrieved."));
    }

    [HttpGet("industry-codes")]
    [ProducesResponseType(typeof(ApiEnvelope<IReadOnlyCollection<IndustryCodeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIndustryCodes(CancellationToken cancellationToken)
    {
        var response = await talageClient.GetIndustryCodesAsync(cancellationToken);
        return Ok(ApiEnvelope<IReadOnlyCollection<IndustryCodeDto>>.Ok(response, "Industry codes retrieved."));
    }

    [HttpGet("activity-codes")]
    [ProducesResponseType(typeof(ApiEnvelope<IReadOnlyCollection<ActivityCodeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivityCodes(
        [FromQuery] int industryCode,
        [FromQuery] string territory,
        CancellationToken cancellationToken)
    {
        var response = await talageClient.GetActivityCodesAsync(industryCode, territory, cancellationToken);
        return Ok(ApiEnvelope<IReadOnlyCollection<ActivityCodeDto>>.Ok(response, "Activity codes retrieved."));
    }
}


