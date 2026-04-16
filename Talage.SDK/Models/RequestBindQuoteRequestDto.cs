using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class RequestBindQuoteRequestDto
{
    public string ApplicationId { get; init; } = string.Empty;

    public string QuoteId { get; init; } = string.Empty;

    public int? PaymentPlanId { get; init; }

    public string? PolicyNumber { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


