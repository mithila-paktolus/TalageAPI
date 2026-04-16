using System.Text.Json;
using System.Text.Json.Serialization;
using TalageIntegration.Shared.Serialization;

namespace Talage.SDK.Models;

public sealed class UpdateApplicationRequestDto
{
    public string ApplicationId { get; init; } = string.Empty;

    public bool? RefreshToken { get; init; }

    public string? BusinessName { get; init; }

    public int? AgencyId { get; init; }

    public int? AgencyLocationId { get; init; }

    [JsonConverter(typeof(StringOrNumberToStringConverter))]
    public string? IndustryCode { get; init; }

    public string? Dba { get; init; }

    public IReadOnlyCollection<ContactDto>? Contacts { get; init; }

    public IReadOnlyCollection<PolicyDto>? Policies { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


