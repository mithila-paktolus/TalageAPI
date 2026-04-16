using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class PolicyDto
{
    public string PolicyType { get; init; } = string.Empty;

    public DateTimeOffset? EffectiveDate { get; init; }

    public int? Deductible { get; init; }

    public string? Limits { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}

