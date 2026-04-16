namespace TalageIntegration.Shared.Models;

public sealed record ApiError(
    string Code,
    string Message,
    string? Field = null);

