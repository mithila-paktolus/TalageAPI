using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class ValidateApplicationRequestValidator : AbstractValidator<ValidateApplicationRequestDto>
{
    public ValidateApplicationRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.PassedValidation).Equal(true);
    }
}


