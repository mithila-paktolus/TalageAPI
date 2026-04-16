using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class QuoteListResponseDto
{
    public string? ApplicationId { get; init; }

    public bool? Complete { get; init; }

    public IReadOnlyCollection<QuoteDto> Quotes { get; init; } = Array.Empty<QuoteDto>();

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}

