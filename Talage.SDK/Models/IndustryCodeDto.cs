using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class IndustryCodeDto
{
    public int? IndustryCodeId { get; init; }

    public int? IndustryCodeCategoryId { get; init; }

    public string? Description { get; init; }

    public string? Category { get; init; }

    public string? Cgl { get; init; }

    public string? Sic { get; init; }

    public string? Naics { get; init; }

    public string? Iso { get; init; }

    public bool? IsFeatured { get; init; }

    public IReadOnlyCollection<string>? AlternateNames { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


