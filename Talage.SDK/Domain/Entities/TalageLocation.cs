using System.Text.Json;
using System.Text.Json.Serialization;

namespace TalageIntegration.Domain.Entities;

public sealed class TalageLocation
{
    public string? LocationId { get; init; }

    public string? Address { get; init; }

    public string? City { get; init; }

    public string? State { get; init; }

    public string? Zipcode { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}

