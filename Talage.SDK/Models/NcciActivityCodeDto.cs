using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class NcciActivityCodeDto
{
    public int? ActivityCodeId { get; init; }

    public string? Description { get; init; }

    public string? Attributes { get; init; }

    public string? NcciCode { get; init; }

    public string? NcciSubCode { get; init; }

    public string? NcciDesc { get; init; }

    public IReadOnlyCollection<string>? AlternateNames { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


