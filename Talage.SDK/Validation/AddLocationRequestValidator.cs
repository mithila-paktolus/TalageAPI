using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class AddLocationRequestValidator : AbstractValidator<AddLocationRequestDto>
{
    public AddLocationRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.Location).NotNull().SetValidator(new LocationDtoValidator());
    }
}

