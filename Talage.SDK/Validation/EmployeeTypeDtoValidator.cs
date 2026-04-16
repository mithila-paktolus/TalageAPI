using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class EmployeeTypeDtoValidator : AbstractValidator<EmployeeTypeDto>
{
    public EmployeeTypeDtoValidator()
    {
        RuleFor(x => x.EmployeeType).NotEmpty();
        RuleFor(x => x.EmployeeTypePayroll).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EmployeeTypeCount).GreaterThanOrEqualTo(0);
    }
}

