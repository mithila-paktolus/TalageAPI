namespace TalageIntegration.Shared.Models;

public sealed class ApiEnvelope<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public static ApiEnvelope<T> Ok(T data, string message = "Request completed successfully.") =>
        new()
        {
            Success = true,
            Message = message,
            Data = data
        };
}

