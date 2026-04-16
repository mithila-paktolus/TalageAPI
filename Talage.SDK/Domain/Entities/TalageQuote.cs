using System.Text.Json;
using System.Text.Json.Serialization;

namespace TalageIntegration.Domain.Entities;

public sealed class TalageQuote
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

