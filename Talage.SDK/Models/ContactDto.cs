using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class ContactDto
{
    public bool Primary { get; init; }

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}

