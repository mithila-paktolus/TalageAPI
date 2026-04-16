using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Talage.SDK.Models;
using Talage.SDK.Internal.Interfaces;
using Talage.SDK.Internal.Auth;

namespace Talage.SDK.Internal.ApiClient;

public sealed class TalageApiClient(
    HttpClient httpClient,
    IAccessTokenStore accessTokenStore,
    IOptions<TalageApiOptions> talageApiOptions,
    ILogger<TalageApiClient> logger) : ITalageApiClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private async Task<(System.Net.HttpStatusCode StatusCode, string Body)> SendTalageAsync(
        HttpRequestMessage message,
        string operation,
        CancellationToken cancellationToken)
    {
        var method = message.Method.Method;
        var uri = message.RequestUri?.ToString() ?? "(null)";
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation("Talage API {Operation} started. {Method} {Uri}", operation, method, uri);

            using var response = await httpClient.SendAsync(message, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            stopwatch.Stop();

            logger.LogInformation(
                "Talage API {Operation} completed. {Method} {Uri} {StatusCode} {ElapsedMs}",
                operation,
                method,
                uri,
                (int)response.StatusCode,
                stopwatch.ElapsedMilliseconds);

            return (response.StatusCode, body);
        }
        catch (HttpRequestException exception)
        {
            stopwatch.Stop();
            throw new TalageApiRequestException(
                "Talage API request failed.",
                operation,
                method,
                uri,
                stopwatch.ElapsedMilliseconds,
                System.Net.HttpStatusCode.BadGateway,
                exception);
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            throw new TalageApiRequestException(
                "Talage API request timed out.",
                operation,
                method,
                uri,
                stopwatch.ElapsedMilliseconds,
                System.Net.HttpStatusCode.GatewayTimeout,
                exception);
        }
    }

    public async Task<IReadOnlyCollection<IndustryCategoryDto>> GetIndustryCategoriesAsync(CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, "industry-categories");
        var (statusCode, body) = await SendTalageAsync(message, nameof(GetIndustryCategoriesAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage industry category retrieval failed.", statusCode, body);
        }

        return DeserializeListOrSingle<IndustryCategoryDto>(body);
    }

    public async Task<IReadOnlyCollection<IndustryCodeDto>> GetIndustryCodesAsync(CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, "industry-codes");
        var (statusCode, body) = await SendTalageAsync(message, nameof(GetIndustryCodesAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage industry code retrieval failed.", statusCode, body);
        }

        return DeserializeListOrSingle<IndustryCodeDto>(body);
    }

    public async Task<IReadOnlyCollection<ActivityCodeDto>> GetActivityCodesAsync(int industryCode, string territory, CancellationToken cancellationToken)
    {
        var uri = $"activity-codes?industry_code={UrlEncode(industryCode.ToString())}&territory={UrlEncode(territory)}";

        using var message = new HttpRequestMessage(HttpMethod.Get, uri);
        var (statusCode, body) = await SendTalageAsync(message, nameof(GetActivityCodesAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage activity code retrieval failed.", statusCode, body);
        }

        return DeserializeListOrSingle<ActivityCodeDto>(body);
    }

    public Task<ApplicationResponseDto> CreateApplicationAsync(CreateApplicationRequestDto request, CancellationToken cancellationToken) =>
        SendAsync<ApplicationResponseDto>(HttpMethod.Post, "application", request, cancellationToken);

    public async Task<ApplicationResponseDto> GetApplicationAsync(string applicationId, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"application/{applicationId}");
        var (statusCode, body) = await SendTalageAsync(message, nameof(GetApplicationAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage application retrieval failed.", statusCode, body);
        }

        var result = JsonSerializer.Deserialize<ApplicationResponseDto>(body, SerializerOptions);
        return result ?? throw new TalageApiException("Talage application response was empty or invalid.", (int)statusCode, body, mappedStatusCode: System.Net.HttpStatusCode.BadGateway);
    }

    public async Task<ApplicationResponseDto> UpdateApplicationAsync(UpdateApplicationRequestDto request, CancellationToken cancellationToken)
    {
        var payload = new UpdateApplicationRequestDto
        {
            ApplicationId = request.ApplicationId,
            RefreshToken = true,
            BusinessName = request.BusinessName,
            AgencyId = request.AgencyId,
            AgencyLocationId = request.AgencyLocationId,
            IndustryCode = request.IndustryCode,
            Dba = request.Dba,
            Contacts = request.Contacts,
            Policies = request.Policies,
            AdditionalData = request.AdditionalData
        };

        var response = await SendAsync<ApplicationResponseDto>(HttpMethod.Put, "application", payload, cancellationToken);

        var refreshedToken = TryExtractRefreshedToken(response);
        if (!string.IsNullOrWhiteSpace(refreshedToken))
        {
            var expires = DateTimeOffset.Now.AddMinutes(talageApiOptions.Value.TokenLifetimeMinutes);
            await accessTokenStore.ReplaceActiveAsync(refreshedToken, "Bearer", expires, cancellationToken);
            logger.LogInformation("Refreshed Talage token from update application response. {Expires}", expires);
        }

        return response;
    }

    private static string? TryExtractRefreshedToken(ApplicationResponseDto response)
    {
        if (response.Token is null)
        {
            return null;
        }

        return response.Token.Value.ValueKind == JsonValueKind.String
            ? response.Token.Value.GetString()
            : null;
    }

    public async Task<ValidateApplicationResponseDto> ValidateApplicationAsync(ValidateApplicationRequestDto request, CancellationToken cancellationToken)
    {
        var uri = $"application/{request.ApplicationId}/validate";

        using var message = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = JsonContent.Create(request)
        };

        var (statusCode, body) = await SendTalageAsync(message, nameof(ValidateApplicationAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage application validation failed.", statusCode, body);
        }

        var result = JsonSerializer.Deserialize<ValidateApplicationResponseDto>(body, SerializerOptions);
        return result ?? new ValidateApplicationResponseDto { PassedValidation = false };
    }

    public async Task<ApplicationListResponseDto> GetApplicationListAsync(ApplicationListQueryDto query, CancellationToken cancellationToken)
    {
        var uri = BuildApplicationListUri(query);

        using var message = new HttpRequestMessage(HttpMethod.Get, uri);
        var (statusCode, body) = await SendTalageAsync(message, nameof(GetApplicationListAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage application list retrieval failed.", statusCode, body);
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return new ApplicationListResponseDto();
        }

        using var document = JsonDocument.Parse(body);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            var applications = JsonSerializer.Deserialize<List<ApplicationResponseDto>>(body, SerializerOptions) ?? [];
            return new ApplicationListResponseDto
            {
                Applications = applications
            };
        }

        return JsonSerializer.Deserialize<ApplicationListResponseDto>(body, SerializerOptions) ?? new ApplicationListResponseDto();
    }

    public async Task<LocationResponseDto> AddLocationAsync(AddLocationRequestDto request, CancellationToken cancellationToken) =>
        await UpsertLocationAsync("Talage add location request failed.", request, cancellationToken);

    public async Task<LocationResponseDto> UpdateLocationAsync(UpdateLocationRequestDto request, CancellationToken cancellationToken) =>
        await UpsertLocationAsync("Talage update location request failed.", request, cancellationToken);

    public async Task<DeleteLocationResponseDto> DeleteLocationAsync(DeleteLocationRequestDto request, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, "application/location")
        {
            Content = JsonContent.Create(request)
        };

        var (statusCode, body) = await SendTalageAsync(message, nameof(DeleteLocationAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage delete location request failed.", statusCode, body);
        }

        var result = JsonSerializer.Deserialize<DeleteLocationResponseDto>(body, SerializerOptions);
        return result ?? new DeleteLocationResponseDto();
    }

    public async Task<ApplicationQuestionsResponseDto> GetQuestionsAsync(string applicationId, string? questionSubjectArea, CancellationToken cancellationToken)
    {
        var uri = string.IsNullOrWhiteSpace(questionSubjectArea)
            ? $"application/{applicationId}/questions"
            : $"application/{applicationId}/questions?questionSubjectArea={UrlEncode(questionSubjectArea)}";

        using var message = new HttpRequestMessage(HttpMethod.Get, uri);
        var (statusCode, body) = await SendTalageAsync(message, nameof(GetQuestionsAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage question retrieval failed.", statusCode, body);
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return new ApplicationQuestionsResponseDto
            {
                ApplicationId = applicationId
            };
        }

        using var document = JsonDocument.Parse(body);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            var questions = JsonSerializer.Deserialize<List<QuestionDto>>(body, SerializerOptions) ?? [];
            return new ApplicationQuestionsResponseDto
            {
                ApplicationId = applicationId,
                Questions = questions
            };
        }

        var parsed = JsonSerializer.Deserialize<ApplicationQuestionsResponseDto>(body, SerializerOptions);
        return parsed ?? new ApplicationQuestionsResponseDto { ApplicationId = applicationId };
    }

    public async Task StartQuoteAsync(StartQuoteRequestDto request, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, "application/quote")
        {
            Content = JsonContent.Create(request)
        };

        var (statusCode, body) = await SendTalageAsync(message, nameof(StartQuoteAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage start quote request failed.", statusCode, body);
        }
    }

    public async Task<QuoteListResponseDto> GetQuotesAsync(string applicationId, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"application/{applicationId}/quotes");
        var (statusCode, body) = await SendTalageAsync(message, nameof(GetQuotesAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage quote retrieval failed.", statusCode, body);
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return new QuoteListResponseDto
            {
                ApplicationId = applicationId
            };
        }

        using var document = JsonDocument.Parse(body);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            var quotes = JsonSerializer.Deserialize<List<QuoteDto>>(body, SerializerOptions) ?? [];
            return new QuoteListResponseDto
            {
                ApplicationId = applicationId,
                Quotes = quotes
            };
        }

        return JsonSerializer.Deserialize<QuoteListResponseDto>(body, SerializerOptions) ?? new QuoteListResponseDto
        {
            ApplicationId = applicationId
        };
    }

    public async Task<QuoteBindResponseDto> BindQuoteAsync(BindQuoteRequestDto request, CancellationToken cancellationToken) =>
        await SendAsyncOrEmptyAsync<QuoteBindResponseDto>(HttpMethod.Put, "application/bind-quote", request, "Talage bind quote request failed.", cancellationToken);

    public async Task<QuoteBindResponseDto> RequestBindQuoteAsync(RequestBindQuoteRequestDto request, CancellationToken cancellationToken) =>
        await SendAsyncOrEmptyAsync<QuoteBindResponseDto>(HttpMethod.Put, "application/request-bind-quote", request, "Talage request bind quote failed.", cancellationToken);

    public async Task<QuoteBindResponseDto> MarkQuoteBoundAsync(MarkQuoteBoundRequestDto request, CancellationToken cancellationToken) =>
        await SendAsyncOrEmptyAsync<QuoteBindResponseDto>(HttpMethod.Put, "application/mark-quote-bound", request, "Talage mark quote bound request failed.", cancellationToken);

    public async Task<IReadOnlyCollection<AppetiteCheckResultDto>> CheckAppetiteAsync(string applicationId, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"application/{applicationId}/checkappetite");
        var (statusCode, body) = await SendTalageAsync(message, nameof(CheckAppetiteAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage appetite check failed.", statusCode, body);
        }

        return DeserializeListOrSingle<AppetiteCheckResultDto>(body);
    }

    public async Task<IReadOnlyCollection<NcciActivityCodeDto>> GetNcciActivityCodesAsync(string applicationId, string territory, string ncciCode, CancellationToken cancellationToken)
    {
        var uri = $"application/{applicationId}/ncci-activity-codes?territory={UrlEncode(territory)}&ncci_code={UrlEncode(ncciCode)}";

        using var message = new HttpRequestMessage(HttpMethod.Get, uri);
        var (statusCode, body) = await SendTalageAsync(message, nameof(GetNcciActivityCodesAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage NCCI activity code lookup failed.", statusCode, body);
        }

        return DeserializeListOrSingle<NcciActivityCodeDto>(body);
    }

    public async Task<RequiredFieldsResponseDto> GetRequiredFieldsAsync(string applicationId, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get, $"application/{applicationId}/getrequiredfields");
        var (statusCode, body) = await SendTalageAsync(message, nameof(GetRequiredFieldsAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage required fields retrieval failed.", statusCode, body);
        }

        return JsonSerializer.Deserialize<RequiredFieldsResponseDto>(body, SerializerOptions) ?? new RequiredFieldsResponseDto();
    }

    public async Task<PriceIndicationResponseDto> PutPriceIndicationAsync(PriceIndicationRequestDto request, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, "application/price")
        {
            Content = JsonContent.Create(request)
        };

        // Log request details for debugging
        var requestBody = await message.Content.ReadAsStringAsync(cancellationToken);
        logger.LogInformation("Price Indication Request - Method: {Method}, URI: {Uri}, Content: {Content}", 
            message.Method, 
            message.RequestUri, 
            requestBody);

        var (statusCode, body) = await SendTalageAsync(message, nameof(PutPriceIndicationAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create("Talage price indication request failed.", statusCode, body);
        }

        try
        {
            return JsonSerializer.Deserialize<PriceIndicationResponseDto>(body, SerializerOptions) ?? new PriceIndicationResponseDto();
        }
        catch (JsonException)
        {
            throw new TalageApiException(
                "Talage price indication response was invalid.",
                (int)statusCode,
                body,
                mappedStatusCode: System.Net.HttpStatusCode.BadGateway);
        }
    }

    private static T DeserializeObjectOrFirstArray<T>(string body, string uri, int statusCode)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new TalageApiException(
                $"Talage response for '{uri}' was empty.",
                statusCode,
                body,
                mappedStatusCode: System.Net.HttpStatusCode.BadGateway);
        }

        try
        {
            using var document = JsonDocument.Parse(body);

            if (document.RootElement.ValueKind == JsonValueKind.Array)
            {
                var items = JsonSerializer.Deserialize<List<T>>(body, SerializerOptions) ?? [];
                var first = items.FirstOrDefault();
                if (first is not null)
                {
                    return first;
                }
            }

            var result = JsonSerializer.Deserialize<T>(body, SerializerOptions);
            return result ?? throw new TalageApiException(
                $"Talage response for '{uri}' was empty or invalid.",
                statusCode,
                body,
                mappedStatusCode: System.Net.HttpStatusCode.BadGateway);
        }
        catch (JsonException)
        {
            throw new TalageApiException(
                $"Talage response for '{uri}' was invalid.",
                statusCode,
                body,
                mappedStatusCode: System.Net.HttpStatusCode.BadGateway);
        }
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string uri, object payload, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(method, uri)
        {
            Content = JsonContent.Create(payload)
        };

        var (statusCode, body) = await SendTalageAsync(message, nameof(SendAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create($"Talage request to '{uri}' failed.", statusCode, body);
        }

        return DeserializeObjectOrFirstArray<T>(body, uri, (int)statusCode);
    }

    private async Task<T> SendAsyncOrEmptyAsync<T>(HttpMethod method, string uri, object payload, string defaultErrorMessage, CancellationToken cancellationToken)
        where T : new()
    {
        using var message = new HttpRequestMessage(method, uri)
        {
            Content = JsonContent.Create(payload)
        };

        var (statusCode, body) = await SendTalageAsync(message, nameof(SendAsyncOrEmptyAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create(defaultErrorMessage, statusCode, body);
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return new T();
        }

        return JsonSerializer.Deserialize<T>(body, SerializerOptions) ?? new T();
    }

    private async Task<LocationResponseDto> UpsertLocationAsync<TRequest>(string defaultErrorMessage, TRequest request, CancellationToken cancellationToken)
    {
        using var message = new HttpRequestMessage(HttpMethod.Put, "application/location")
        {
            Content = JsonContent.Create(request)
        };

        var (statusCode, body) = await SendTalageAsync(message, nameof(UpsertLocationAsync), cancellationToken);

        if ((int)statusCode < 200 || (int)statusCode > 299)
        {
            throw TalageApiException.Create(defaultErrorMessage, statusCode, body);
        }

        using var document = JsonDocument.Parse(body);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            var locations = JsonSerializer.Deserialize<List<LocationResponseDto>>(body, SerializerOptions) ?? [];
            return locations.FirstOrDefault() ?? new LocationResponseDto();
        }

        return JsonSerializer.Deserialize<LocationResponseDto>(body, SerializerOptions) ?? new LocationResponseDto();
    }

    private static IReadOnlyCollection<T> DeserializeListOrSingle<T>(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return Array.Empty<T>();
        }

        using var document = JsonDocument.Parse(body);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<T>>(body, SerializerOptions) ?? [];
        }

        var single = JsonSerializer.Deserialize<T>(body, SerializerOptions);
        return single is null ? Array.Empty<T>() : new[] { single };
    }

    private static string BuildApplicationListUri(ApplicationListQueryDto query)
    {
        var parameters = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        if (query.Limit is not null)
        {
            parameters["limit"] = query.Limit.Value.ToString();
        }

        if (query.Page is not null)
        {
            parameters["page"] = query.Page.Value.ToString();
        }

        if (query.Count is not null)
        {
            parameters["count"] = query.Count.Value ? "1" : "0";
        }

        if (!string.IsNullOrWhiteSpace(query.Sort))
        {
            parameters["sort"] = query.Sort;
        }

        if (query.Desc is not null)
        {
            parameters["desc"] = query.Desc.Value ? "1" : "0";
        }

        if (query.SearchBeginDate is not null)
        {
            parameters["searchbegindate"] = query.SearchBeginDate.Value.ToString("O");
        }

        if (query.SearchEndDate is not null)
        {
            parameters["searchenddate"] = query.SearchEndDate.Value.ToString("O");
        }

        if (query.AppStatusId is not null)
        {
            parameters["appStatusId"] = query.AppStatusId.Value.ToString();
        }

        if (query.LtAppStatusId is not null)
        {
            parameters["ltAppStatusId"] = query.LtAppStatusId.Value.ToString();
        }

        if (query.GtAppStatusId is not null)
        {
            parameters["gtAppStatusId"] = query.GtAppStatusId.Value.ToString();
        }

        if (query.AdditionalFilters is not null)
        {
            foreach (var (key, value) in query.AdditionalFilters)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                parameters[key] = value;
            }
        }

        if (parameters.Count == 0)
        {
            return "application";
        }

        var queryString = string.Join("&", parameters
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
            .Select(kvp => $"{UrlEncode(kvp.Key)}={UrlEncode(kvp.Value!)}"));

        return string.IsNullOrWhiteSpace(queryString) ? "application" : $"application?{queryString}";
    }

    private static string UrlEncode(string? value) =>
        Uri.EscapeDataString((value ?? string.Empty).Trim());
}

