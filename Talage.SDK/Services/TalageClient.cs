using Microsoft.Extensions.Logging;
using Talage.SDK.Interfaces;
using Talage.SDK.Logging;
using Talage.SDK.Models;
using Talage.SDK.Internal.Interfaces;
using TalageIntegration.Shared.Models;

namespace Talage.SDK.Services;

public sealed class TalageClient(
    ITalageApiClient talageApiClient,
    IQuotePollingGuard quotePollingGuard,
    IApiLogger apiLogger,
    ILogger<TalageClient> logger) : ITalageClient
{
    public Task<IReadOnlyCollection<IndustryCategoryDto>> GetIndustryCategoriesAsync(CancellationToken cancellationToken) => talageApiClient.GetIndustryCategoriesAsync(cancellationToken);
    public Task<IReadOnlyCollection<IndustryCodeDto>> GetIndustryCodesAsync(CancellationToken cancellationToken) => talageApiClient.GetIndustryCodesAsync(cancellationToken);
    public Task<IReadOnlyCollection<ActivityCodeDto>> GetActivityCodesAsync(int industryCode, string territory, CancellationToken cancellationToken) => talageApiClient.GetActivityCodesAsync(industryCode, territory, cancellationToken);

    public async Task<ApplicationResponseDto> CreateApplicationAsync(CreateApplicationRequestDto request, string user, CancellationToken cancellationToken)
    {
        var response = await talageApiClient.CreateApplicationAsync(request, cancellationToken);
        var envelope = ApiEnvelope<ApplicationResponseDto>.Ok(response, "Application created.");

        if (!string.IsNullOrWhiteSpace(response?.ApplicationId))
        {
            await apiLogger.AddApplicationLogAsync(response.ApplicationId, envelope, user, isUpdate: false);
        }

        return response;
    }

    public Task<ApplicationResponseDto> GetApplicationAsync(string applicationId, CancellationToken cancellationToken) => talageApiClient.GetApplicationAsync(applicationId, cancellationToken);

    public async Task<ApplicationResponseDto> UpdateApplicationAsync(UpdateApplicationRequestDto request, string user, CancellationToken cancellationToken)
    {
        var response = await talageApiClient.UpdateApplicationAsync(request, cancellationToken);
        var envelope = ApiEnvelope<ApplicationResponseDto>.Ok(response, "Application updated.");

        if (!string.IsNullOrWhiteSpace(request.ApplicationId))
        {
            await apiLogger.AddApplicationLogAsync(request.ApplicationId, envelope, user, isUpdate: true);
        }

        return response;
    }

    public async Task<ValidateApplicationResponseDto> ValidateApplicationAsync(string applicationId, CancellationToken cancellationToken)
    {
        logger.LogInformation("ValidateApplicationAsync started for ApplicationId {ApplicationId}", applicationId);
        var response = await talageApiClient.ValidateApplicationAsync(new ValidateApplicationRequestDto { ApplicationId = applicationId, PassedValidation = true }, cancellationToken);
        logger.LogInformation("ValidateApplicationAsync completed for ApplicationId {ApplicationId}", applicationId);
        return response;
    }

    public Task<ApplicationListResponseDto> GetApplicationListAsync(ApplicationListQueryDto query, CancellationToken cancellationToken) => talageApiClient.GetApplicationListAsync(query, cancellationToken);
    public Task<LocationResponseDto> AddLocationAsync(AddLocationRequestDto request, CancellationToken cancellationToken) => talageApiClient.AddLocationAsync(request, cancellationToken);
    public Task<LocationResponseDto> UpdateLocationAsync(UpdateLocationRequestDto request, CancellationToken cancellationToken) => talageApiClient.UpdateLocationAsync(request, cancellationToken);

    public Task<DeleteLocationResponseDto> DeleteLocationAsync(string applicationId, string locationId, CancellationToken cancellationToken) =>
        talageApiClient.DeleteLocationAsync(new DeleteLocationRequestDto
        {
            ApplicationId = applicationId,
            Delete = true,
            Location = new LocationDeleteDto { LocationId = locationId }
        }, cancellationToken);

    public Task<ApplicationQuestionsResponseDto> GetQuestionsAsync(string applicationId, string? questionSubjectArea, CancellationToken cancellationToken) => talageApiClient.GetQuestionsAsync(applicationId, questionSubjectArea, cancellationToken);
    public Task StartQuoteAsync(StartQuoteRequestDto request, CancellationToken cancellationToken) => talageApiClient.StartQuoteAsync(request, cancellationToken);

    public async Task<QuoteListResponseDto> GetQuotesAsync(string applicationId, CancellationToken cancellationToken)
    {
        await quotePollingGuard.EnsureAllowedAsync(applicationId, cancellationToken);
        return await talageApiClient.GetQuotesAsync(applicationId, cancellationToken);
    }

    public Task<QuoteBindResponseDto> BindQuoteAsync(BindQuoteRequestDto request, CancellationToken cancellationToken) => talageApiClient.BindQuoteAsync(request, cancellationToken);
    public Task<QuoteBindResponseDto> RequestBindQuoteAsync(RequestBindQuoteRequestDto request, CancellationToken cancellationToken) => talageApiClient.RequestBindQuoteAsync(request, cancellationToken);
    public Task<QuoteBindResponseDto> MarkQuoteBoundAsync(MarkQuoteBoundRequestDto request, CancellationToken cancellationToken) => talageApiClient.MarkQuoteBoundAsync(request, cancellationToken);
    public Task<IReadOnlyCollection<AppetiteCheckResultDto>> CheckAppetiteAsync(string applicationId, CancellationToken cancellationToken) => talageApiClient.CheckAppetiteAsync(applicationId, cancellationToken);
    public Task<IReadOnlyCollection<NcciActivityCodeDto>> GetNcciActivityCodesAsync(string applicationId, string territory, string ncciCode, CancellationToken cancellationToken) => talageApiClient.GetNcciActivityCodesAsync(applicationId, territory, ncciCode, cancellationToken);
    public Task<RequiredFieldsResponseDto> GetRequiredFieldsAsync(string applicationId, CancellationToken cancellationToken) => talageApiClient.GetRequiredFieldsAsync(applicationId, cancellationToken);
    public Task<PriceIndicationResponseDto> PutPriceIndicationAsync(string applicationId, CancellationToken cancellationToken) => talageApiClient.PutPriceIndicationAsync(new PriceIndicationRequestDto { Id = applicationId }, cancellationToken);
}

