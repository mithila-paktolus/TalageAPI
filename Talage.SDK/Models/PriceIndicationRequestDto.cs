using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class PriceIndicationRequestDto
{
    [JsonPropertyName("applicationId")]
    public string Id { get; init; } = string.Empty;
}


