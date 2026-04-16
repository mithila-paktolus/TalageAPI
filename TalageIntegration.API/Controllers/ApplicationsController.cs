using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talage.SDK.Interfaces;
using Talage.SDK.Models;
using TalageIntegration.Shared.Models;

namespace TalageIntegration.API.Controllers;

[Authorize]
[ApiController]
[Route("api/applications")]
public sealed class ApplicationsController(
    ITalageClient talageClient) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiEnvelope<ApplicationListResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplications([FromQuery] ApplicationListQueryDto query, CancellationToken cancellationToken)
    {
        var knownKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "limit",
            "page",
            "count",
            "sort",
            "desc",
            "searchBeginDate",
            "searchEndDate",
            "appStatusId",
            "ltAppStatusId",
            "gtAppStatusId"
        };

        Dictionary<string, string?>? additionalFilters = null;

        foreach (var (key, value) in Request.Query)
        {
            if (knownKeys.Contains(key))
            {
                continue;
            }

            additionalFilters ??= new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            additionalFilters[key] = value.Count > 0 ? value[0] : null;
        }

        var command = new ApplicationListQueryDto
        {
            Limit = query.Limit,
            Page = query.Page,
            Count = query.Count,
            Sort = query.Sort,
            Desc = query.Desc,
            SearchBeginDate = query.SearchBeginDate,
            SearchEndDate = query.SearchEndDate,
            AppStatusId = query.AppStatusId,
            LtAppStatusId = query.LtAppStatusId,
            GtAppStatusId = query.GtAppStatusId,
            AdditionalFilters = additionalFilters
        };

        var response = await talageClient.GetApplicationListAsync(command, cancellationToken);
        return Ok(ApiEnvelope<ApplicationListResponseDto>.Ok(response, "Applications retrieved."));
    }

    [HttpGet("{applicationId}")]
    [ProducesResponseType(typeof(ApiEnvelope<ApplicationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplication(string applicationId, CancellationToken cancellationToken)
    {
        var response = await talageClient.GetApplicationAsync(applicationId, cancellationToken);
        return Ok(ApiEnvelope<ApplicationResponseDto>.Ok(response, "Application retrieved."));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiEnvelope<ApplicationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationRequestDto request, CancellationToken cancellationToken)
    {
        var user = User?.Identity?.Name ?? "System";
        var response = await talageClient.CreateApplicationAsync(request, user, cancellationToken);
        return Ok(ApiEnvelope<ApplicationResponseDto>.Ok(response, "Application created."));
    }

    [HttpPut("{applicationId}")]
    [ProducesResponseType(typeof(ApiEnvelope<ApplicationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateApplication(
        string applicationId,
        [FromBody] UpdateApplicationRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateApplicationRequestDto
        {
            ApplicationId = applicationId,
            RefreshToken = request.RefreshToken,
            BusinessName = request.BusinessName,
            AgencyId = request.AgencyId,
            AgencyLocationId = request.AgencyLocationId,
            IndustryCode = request.IndustryCode,
            Dba = request.Dba,
            Contacts = request.Contacts,
            Policies = request.Policies,
            AdditionalData = request.AdditionalData
        };

        var user = User?.Identity?.Name ?? "System";
        var response = await talageClient.UpdateApplicationAsync(command, user, cancellationToken);
        return Ok(ApiEnvelope<ApplicationResponseDto>.Ok(response, "Application updated."));
    }

    [HttpPut("{applicationId}/validate")]
    [ProducesResponseType(typeof(ApiEnvelope<ValidateApplicationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateApplication(string applicationId, CancellationToken cancellationToken)
    {
        var response = await talageClient.ValidateApplicationAsync(applicationId, cancellationToken);
        return Ok(ApiEnvelope<ValidateApplicationResponseDto>.Ok(response, "Application validated."));
    }

    [HttpPut("{applicationId}/locations")]
    [ProducesResponseType(typeof(ApiEnvelope<LocationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddLocation(
        string applicationId,
        [FromBody] AddLocationRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new AddLocationRequestDto
        {
            ApplicationId = applicationId,
            Location = request.Location
        };

        var response = await talageClient.AddLocationAsync(command, cancellationToken);
        return Ok(ApiEnvelope<LocationResponseDto>.Ok(response, "Location added."));
    }

    [HttpPut("{applicationId}/locations/{locationId}")]
    [ProducesResponseType(typeof(ApiEnvelope<LocationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateLocation(
        string applicationId,
        string locationId,
        [FromBody] LocationUpdateDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationRequestDto
        {
            ApplicationId = applicationId,
            Location = new LocationUpdateDto
            {
                LocationId = locationId,
                Billing = request.Billing,
                Primary = request.Primary,
                ActivityPayrollList = request.ActivityPayrollList,
                Questions = request.Questions,
                Own = request.Own,
                ConstructionType = request.ConstructionType,
                NumStories = request.NumStories,
                YearBuilt = request.YearBuilt,
                BusinessPersonalPropertyLimit = request.BusinessPersonalPropertyLimit,
                BuildingLimit = request.BuildingLimit,
                Address = request.Address,
                Address2 = request.Address2,
                City = request.City,
                State = request.State,
                Zipcode = request.Zipcode,
                FullTimeEmployees = request.FullTimeEmployees,
                PartTimeEmployees = request.PartTimeEmployees,
                SquareFootage = request.SquareFootage,
                UnemploymentNum = request.UnemploymentNum,
                AdditionalData = request.AdditionalData
            }
        };

        var response = await talageClient.UpdateLocationAsync(command, cancellationToken);
        return Ok(ApiEnvelope<LocationResponseDto>.Ok(response, "Location updated."));
    }

    [HttpDelete("{applicationId}/locations/{locationId}")]
    [ProducesResponseType(typeof(ApiEnvelope<DeleteLocationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteLocation(string applicationId, string locationId, CancellationToken cancellationToken)
    {
        var response = await talageClient.DeleteLocationAsync(applicationId, locationId, cancellationToken);
        return Ok(ApiEnvelope<DeleteLocationResponseDto>.Ok(response, "Location deleted."));
    }

    [HttpGet("{applicationId}/questions")]
    [ProducesResponseType(typeof(ApiEnvelope<ApplicationQuestionsResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuestions(
        string applicationId,
        [FromQuery] string? questionSubjectArea,
        CancellationToken cancellationToken)
    {
        var response = await talageClient.GetQuestionsAsync(applicationId, questionSubjectArea, cancellationToken);
        return Ok(ApiEnvelope<ApplicationQuestionsResponseDto>.Ok(response, "Questions retrieved."));
    }

    [HttpPut("{applicationId}/quotes/start")]
    [ProducesResponseType(typeof(ApiEnvelope<string>), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> StartQuote(string applicationId, CancellationToken cancellationToken)
    {
        await talageClient.StartQuoteAsync(new StartQuoteRequestDto { ApplicationId = applicationId }, cancellationToken);
        return Accepted(ApiEnvelope<string>.Ok(applicationId, "Quote process started."));
    }

    [HttpGet("{applicationId}/quotes")]
    [ProducesResponseType(typeof(ApiEnvelope<QuoteListResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuotes(string applicationId, CancellationToken cancellationToken)
    {
        var response = await talageClient.GetQuotesAsync(applicationId, cancellationToken);
        return Ok(ApiEnvelope<QuoteListResponseDto>.Ok(response, "Quotes retrieved."));
    }

    [HttpPost("{applicationId}/quotes/{quoteId}/bind")]
    [ProducesResponseType(typeof(ApiEnvelope<QuoteBindResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BindQuote(
        string applicationId,
        string quoteId,
        [FromBody] BindQuoteBodyDto request,
        CancellationToken cancellationToken)
    {
        var command = new BindQuoteRequestDto
        {
            ApplicationId = applicationId,
            QuoteId = quoteId,
            PaymentPlanId = request.PaymentPlanId,
            AdditionalData = request.AdditionalData
        };

        var response = await talageClient.BindQuoteAsync(command, cancellationToken);
        return Ok(ApiEnvelope<QuoteBindResponseDto>.Ok(response, "Bind request submitted."));
    }

    [HttpPost("{applicationId}/quotes/{quoteId}/request-bind")]
    [ProducesResponseType(typeof(ApiEnvelope<QuoteBindResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RequestBindQuote(
        string applicationId,
        string quoteId,
        [FromBody] RequestBindQuoteBodyDto request,
        CancellationToken cancellationToken)
    {
        var command = new RequestBindQuoteRequestDto
        {
            ApplicationId = applicationId,
            QuoteId = quoteId,
            PaymentPlanId = request.PaymentPlanId,
            PolicyNumber = request.PolicyNumber,
            AdditionalData = request.AdditionalData
        };

        var response = await talageClient.RequestBindQuoteAsync(command, cancellationToken);
        return Ok(ApiEnvelope<QuoteBindResponseDto>.Ok(response, "Bind request submitted."));
    }

    [HttpPost("{applicationId}/quotes/{quoteId}/mark-bound")]
    [ProducesResponseType(typeof(ApiEnvelope<QuoteBindResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkQuoteBound(
        string applicationId,
        string quoteId,
        [FromBody] MarkQuoteBoundBodyDto request,
        CancellationToken cancellationToken)
    {
        var command = new MarkQuoteBoundRequestDto
        {
            ApplicationId = applicationId,
            QuoteId = quoteId,
            MarkAsBound = true,
            PremiumAmount = request.PremiumAmount,
            PolicyNumber = request.PolicyNumber,
            AdditionalData = request.AdditionalData
        };

        var response = await talageClient.MarkQuoteBoundAsync(command, cancellationToken);
        return Ok(ApiEnvelope<QuoteBindResponseDto>.Ok(response, "Quote marked as bound."));
    }

    [HttpGet("{applicationId}/appetite")]
    [ProducesResponseType(typeof(ApiEnvelope<IReadOnlyCollection<AppetiteCheckResultDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckAppetite(string applicationId, CancellationToken cancellationToken)
    {
        var response = await talageClient.CheckAppetiteAsync(applicationId, cancellationToken);
        return Ok(ApiEnvelope<IReadOnlyCollection<AppetiteCheckResultDto>>.Ok(response, "Appetite check completed."));
    }

    [HttpGet("{applicationId}/ncci-activity-codes")]
    [ProducesResponseType(typeof(ApiEnvelope<IReadOnlyCollection<NcciActivityCodeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNcciActivityCodes(
        string applicationId,
        [FromQuery] string territory,
        [FromQuery] string ncciCode,
        CancellationToken cancellationToken)
    {
        var response = await talageClient.GetNcciActivityCodesAsync(applicationId, territory, ncciCode, cancellationToken);
        return Ok(ApiEnvelope<IReadOnlyCollection<NcciActivityCodeDto>>.Ok(response, "NCCI activity codes retrieved."));
    }

    [HttpGet("{applicationId}/required-fields")]
    [ProducesResponseType(typeof(ApiEnvelope<RequiredFieldsResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRequiredFields(string applicationId, CancellationToken cancellationToken)
    {
        var response = await talageClient.GetRequiredFieldsAsync(applicationId, cancellationToken);
        return Ok(ApiEnvelope<RequiredFieldsResponseDto>.Ok(response, "Required fields retrieved."));
    }

    [HttpPut("{applicationId}/price-indication")]
    [ProducesResponseType(typeof(ApiEnvelope<PriceIndicationResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PutPriceIndication(string applicationId, CancellationToken cancellationToken)
    {
        var response = await talageClient.PutPriceIndicationAsync(applicationId, cancellationToken);
        return Ok(ApiEnvelope<PriceIndicationResponseDto>.Ok(response, "Price indication requested."));
    }
}


