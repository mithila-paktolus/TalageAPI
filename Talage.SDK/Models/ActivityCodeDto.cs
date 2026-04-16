using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class ActivityCodeDto
{
    public int? ActivityCodeId { get; init; }

    public string? Description { get; init; }

    public int? Suggested { get; init; }

    public IReadOnlyCollection<string>? AlternateNames { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


