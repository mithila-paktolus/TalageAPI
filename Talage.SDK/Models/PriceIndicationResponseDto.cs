using System.Text.Json;
using System.Text.Json.Serialization;

namespace Talage.SDK.Models;

public sealed class PriceIndicationResponseDto
{
    public bool? gotPricing { get; init; }

    //public decimal? Price { get; init; }

    public bool? outOfAppetite { get; init; }

    public bool? pricingError { get; init; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; init; }
}


