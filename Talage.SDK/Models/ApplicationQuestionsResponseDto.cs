using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class ApplicationQuestionsResponseDto
{
    public string ApplicationId { get; init; } = string.Empty;

    public IReadOnlyCollection<QuestionDto> Questions { get; init; } = Array.Empty<QuestionDto>();

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


