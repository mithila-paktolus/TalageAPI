using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class RequestBindQuoteBodyDto
{
    public int? PaymentPlanId { get; init; }

    public string? PolicyNumber { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


