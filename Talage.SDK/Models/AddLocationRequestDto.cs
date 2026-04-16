namespace Talage.SDK.Models;

public sealed class AddLocationRequestDto
{
    public string ApplicationId { get; init; } = string.Empty;

    public LocationDto Location { get; init; } = new();
}

