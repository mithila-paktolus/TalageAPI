using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class MarkQuoteBoundRequestDto
{
    public string ApplicationId { get; init; } = string.Empty;

    public string QuoteId { get; init; } = string.Empty;

    public bool MarkAsBound { get; init; } = true;

    public decimal? PremiumAmount { get; init; }

    public string? PolicyNumber { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


