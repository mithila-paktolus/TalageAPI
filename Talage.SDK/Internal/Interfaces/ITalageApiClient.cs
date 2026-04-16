using Talage.SDK.Models;

namespace Talage.SDK.Internal.Interfaces;

public interface ITalageApiClient
{
    Task<IReadOnlyCollection<IndustryCategoryDto>> GetIndustryCategoriesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<IndustryCodeDto>> GetIndustryCodesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ActivityCodeDto>> GetActivityCodesAsync(int industryCode, string territory, CancellationToken cancellationToken);

    Task<ApplicationResponseDto> CreateApplicationAsync(CreateApplicationRequestDto request, CancellationToken cancellationToken);

    Task<ApplicationResponseDto> GetApplicationAsync(string applicationId, CancellationToken cancellationToken);

    Task<ApplicationResponseDto> UpdateApplicationAsync(UpdateApplicationRequestDto request, CancellationToken cancellationToken);

    Task<ValidateApplicationResponseDto> ValidateApplicationAsync(ValidateApplicationRequestDto request, CancellationToken cancellationToken);

    Task<ApplicationListResponseDto> GetApplicationListAsync(ApplicationListQueryDto query, CancellationToken cancellationToken);

    Task<LocationResponseDto> AddLocationAsync(AddLocationRequestDto request, CancellationToken cancellationToken);

    Task<LocationResponseDto> UpdateLocationAsync(UpdateLocationRequestDto request, CancellationToken cancellationToken);

    Task<DeleteLocationResponseDto> DeleteLocationAsync(DeleteLocationRequestDto request, CancellationToken cancellationToken);

    Task<ApplicationQuestionsResponseDto> GetQuestionsAsync(string applicationId, string? questionSubjectArea, CancellationToken cancellationToken);

    Task StartQuoteAsync(StartQuoteRequestDto request, CancellationToken cancellationToken);

    Task<QuoteListResponseDto> GetQuotesAsync(string applicationId, CancellationToken cancellationToken);

    Task<QuoteBindResponseDto> BindQuoteAsync(BindQuoteRequestDto request, CancellationToken cancellationToken);

    Task<QuoteBindResponseDto> RequestBindQuoteAsync(RequestBindQuoteRequestDto request, CancellationToken cancellationToken);

    Task<QuoteBindResponseDto> MarkQuoteBoundAsync(MarkQuoteBoundRequestDto request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AppetiteCheckResultDto>> CheckAppetiteAsync(string applicationId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<NcciActivityCodeDto>> GetNcciActivityCodesAsync(string applicationId, string territory, string ncciCode, CancellationToken cancellationToken);

    Task<RequiredFieldsResponseDto> GetRequiredFieldsAsync(string applicationId, CancellationToken cancellationToken);

    Task<PriceIndicationResponseDto> PutPriceIndicationAsync(PriceIndicationRequestDto request, CancellationToken cancellationToken);
}

