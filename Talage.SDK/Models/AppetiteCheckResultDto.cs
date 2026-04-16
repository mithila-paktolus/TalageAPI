using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class AppetiteCheckResultDto
{
    public string? PolicyTypeCd { get; init; }

    public string? AppetiteStatus { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


