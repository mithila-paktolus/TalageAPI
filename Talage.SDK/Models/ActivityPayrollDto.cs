using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class ActivityPayrollDto
{
    public int ActivityCode { get; init; }

    public decimal Payroll { get; init; }

    public string? Description { get; init; }

    public IReadOnlyCollection<EmployeeTypeDto> EmployeeTypeList { get; init; } = Array.Empty<EmployeeTypeDto>();

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}

