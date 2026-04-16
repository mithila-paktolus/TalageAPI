using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class RequiredFieldsResponseDto
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Fields { get; init; }
}


