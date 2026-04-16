using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class UpdateApplicationRequestValidator : AbstractValidator<UpdateApplicationRequestDto>
{
    public UpdateApplicationRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleForEach(x => x.Contacts).SetValidator(new ContactDtoValidator());
        RuleForEach(x => x.Policies).SetValidator(new PolicyDtoValidator());
    }
}

