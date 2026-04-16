using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class QuestionDto
{
    public int? TypeId { get; init; }

    public int? TalageQuestionId { get; init; }

    public int? Parent { get; init; }

    [JsonPropertyName("parent_answer")]
    public int? ParentAnswer { get; init; }

    [JsonPropertyName("sub_level")]
    public int? SubLevel { get; init; }

    public string? Text { get; init; }

    public string? TypeDesc { get; init; }

    public string? Type { get; init; }

    public string? CategoryName { get; init; }

    public int? CategorySortNumber { get; init; }

    public int? SortRanking { get; init; }

    public IReadOnlyCollection<QuestionAnswerDto>? Answers { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


