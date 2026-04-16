namespace Talage.SDK.Models;

public sealed class AuthenticationResponseDto
{
    public string Status { get; init; } = string.Empty;

    public string Token { get; init; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; init; }
}

