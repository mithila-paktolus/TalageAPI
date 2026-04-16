namespace Talage.SDK.Models;

public sealed class ApplicationListQueryDto
{
    public int? Limit { get; init; }

    public int? Page { get; init; }

    public bool? Count { get; init; }

    public string? Sort { get; init; }

    public bool? Desc { get; init; }

    public DateTimeOffset? SearchBeginDate { get; init; }

    public DateTimeOffset? SearchEndDate { get; init; }

    public int? AppStatusId { get; init; }

    public int? LtAppStatusId { get; init; }

    public int? GtAppStatusId { get; init; }

    public IReadOnlyDictionary<string, string?>? AdditionalFilters { get; init; }
}

