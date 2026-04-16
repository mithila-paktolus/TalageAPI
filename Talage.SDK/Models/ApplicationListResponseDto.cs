using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class ApplicationListResponseDto
{
    public int? Count { get; init; }

    public IReadOnlyCollection<ApplicationResponseDto>? Applications { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


