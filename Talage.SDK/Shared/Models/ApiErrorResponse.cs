namespace TalageIntegration.Shared.Models;

public sealed class ApiErrorResponse
{
    public bool Success { get; init; } = false;

    public string Message { get; init; } = string.Empty;

    public string CorrelationId { get; init; } = string.Empty;

    public IReadOnlyCollection<ApiError> Errors { get; init; } = Array.Empty<ApiError>();
}

