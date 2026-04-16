using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class ApplicationResponseDto
{
    public string ApplicationId { get; init; } = string.Empty;

    public int? AgencyId { get; init; }

    public int? AgencyLocationId { get; init; }

    public string? BusinessName { get; init; }

    public int? AppStatusId { get; init; }

    public string? Status { get; init; }

    public JsonElement? Token { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


