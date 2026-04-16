using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class IndustryCategoryDto
{
    public int? IndustryCodeCategoryId { get; init; }

    public string? Name { get; init; }

    public bool? Featured { get; init; }

    public int? Id { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


