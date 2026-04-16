using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class DeleteLocationRequestDto
{
    public string ApplicationId { get; init; } = string.Empty;

    [JsonPropertyName("delete")]
    public bool Delete { get; init; }

    public LocationDeleteDto Location { get; init; } = new();
}


