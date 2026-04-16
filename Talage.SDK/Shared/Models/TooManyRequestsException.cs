namespace TalageIntegration.Shared.Models;

public sealed class TooManyRequestsException(string message) : Exception(message)
{
}

