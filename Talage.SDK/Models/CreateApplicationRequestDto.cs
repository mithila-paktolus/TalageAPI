using System.Text.Json;
using System.Text.Json.Serialization;
using TalageIntegration.Shared.Serialization;

namespace Talage.SDK.Models;

public sealed class CreateApplicationRequestDto
{
    [JsonConverter(typeof(StringOrNumberToStringConverter))]
    public string AgencyId { get; init; } = string.Empty;

    public string BusinessName { get; init; } = string.Empty;

    [JsonConverter(typeof(StringOrNumberToStringConverter))]
    public string? IndustryCode { get; init; }

    public string? Dba { get; init; }

    public IReadOnlyCollection<ContactDto>? Contacts { get; init; }

    public IReadOnlyCollection<PolicyDto>? Policies { get; init; }

    public string? Phone { get; init; }

    public string? MailingAddress { get; init; }

    public string? MailingAddress2 { get; init; }

    public string? MailingCity { get; init; }

    public string? MailingState { get; init; }

    public string? MailingZipcode { get; init; }

    public string? EntityType { get; init; }

    public string? Website { get; init; }

    public DateTimeOffset? Founded { get; init; }

    public string? Ein { get; init; }

    [JsonPropertyName("management_structure")]
    public string? ManagementStructure { get; init; }

    public string? CorporationType { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


