using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class EmployeeTypeDto
{
    public string EmployeeType { get; init; } = string.Empty;

    public decimal EmployeeTypePayroll { get; init; }

    public int EmployeeTypeCount { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}

