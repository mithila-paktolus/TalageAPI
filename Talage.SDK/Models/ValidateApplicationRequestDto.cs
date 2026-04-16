namespace Talage.SDK.Models;

public sealed class ValidateApplicationRequestDto
{
    public string ApplicationId { get; init; } = string.Empty;

    public bool PassedValidation { get; init; } = true;
}


