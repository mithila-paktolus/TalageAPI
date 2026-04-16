namespace Talage.SDK.Models;

public sealed class UpdateLocationRequestDto
{
    public string ApplicationId { get; init; } = string.Empty;

    public LocationUpdateDto Location { get; init; } = new();
}


