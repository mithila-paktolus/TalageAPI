namespace TalageIntegration.Domain.Entities;

public sealed class TalageToken
{
    public string Value { get; init; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; init; }
}

