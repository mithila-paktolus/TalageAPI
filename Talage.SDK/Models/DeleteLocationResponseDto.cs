using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class DeleteLocationResponseDto
{
    public string? ApplicationId { get; init; }

    [JsonPropertyName("delete")]
    public bool? Delete { get; init; }

    public LocationDeleteDto? Location { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


