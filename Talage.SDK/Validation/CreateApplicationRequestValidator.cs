using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class CreateApplicationRequestValidator : AbstractValidator<CreateApplicationRequestDto>
{
    public CreateApplicationRequestValidator()
    {
        RuleFor(x => x.AgencyId).NotEmpty();
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(250);
        RuleForEach(x => x.Contacts).SetValidator(new ContactDtoValidator());
        RuleForEach(x => x.Policies).SetValidator(new PolicyDtoValidator());
    }
}

