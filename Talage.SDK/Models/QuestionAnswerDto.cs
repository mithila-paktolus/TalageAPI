using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class QuestionAnswerDto
{
    public int? AnswerId { get; init; }

    public string? Answer { get; init; }

    public bool? Default { get; init; }

    public int? Id { get; init; }

    public int? Question { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


