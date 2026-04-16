using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class QuoteDto
{
    public string? QuoteId { get; init; }

    public string? PolicyType { get; init; }

    public int? QuoteStatusId { get; init; }

    public string? QuoteStatusDescription { get; init; }

    public decimal? Amount { get; init; }

    public string? InsurerName { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}

