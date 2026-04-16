using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class LocationResponseDto
{
    public string? LocationId { get; init; }

    public string? Address { get; init; }

    public string? City { get; init; }

    public string? State { get; init; }

    public string? Zipcode { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}

