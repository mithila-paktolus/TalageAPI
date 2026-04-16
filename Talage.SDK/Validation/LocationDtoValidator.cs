using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class LocationDtoValidator : AbstractValidator<LocationDto>
{
    public LocationDtoValidator()
    {
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.State).NotEmpty().Length(2);
        RuleFor(x => x.Zipcode).NotEmpty();
        RuleForEach(x => x.ActivityPayrollList).SetValidator(new ActivityPayrollDtoValidator());
    }
}

