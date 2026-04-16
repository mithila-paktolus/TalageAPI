using TalageIntegration.Shared.Models;

namespace TalageIntegration.Shared.Exceptions;

public sealed class RequestValidationException : Exception
{
    public RequestValidationException(string message, IReadOnlyCollection<ApiError> errors) : base(message)
    {
        Errors = errors;
    }

    public IReadOnlyCollection<ApiError> Errors { get; }
}

