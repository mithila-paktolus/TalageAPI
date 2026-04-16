using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class LocationUpdateDto
{
    public string? LocationId { get; init; }

    public bool? Billing { get; init; }

    public bool? Primary { get; init; }

    public IReadOnlyCollection<ActivityPayrollDto>? ActivityPayrollList { get; init; }

    public IReadOnlyCollection<object>? Questions { get; init; }

    public bool? Own { get; init; }

    public string? ConstructionType { get; init; }

    public int? NumStories { get; init; }

    public int? YearBuilt { get; init; }

    public decimal? BusinessPersonalPropertyLimit { get; init; }

    public decimal? BuildingLimit { get; init; }

    public string? Address { get; init; }

    public string? Address2 { get; init; }

    public string? City { get; init; }

    public string? State { get; init; }

    public string? Zipcode { get; init; }

    [JsonPropertyName("full_time_employees")]
    public int? FullTimeEmployees { get; init; }

    [JsonPropertyName("part_time_employees")]
    public int? PartTimeEmployees { get; init; }

    [JsonPropertyName("square_footage")]
    public int? SquareFootage { get; init; }

    [JsonPropertyName("unemployment_num")]
    public string? UnemploymentNum { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


