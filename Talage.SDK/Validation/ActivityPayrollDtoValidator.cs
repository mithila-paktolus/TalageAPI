using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class ActivityPayrollDtoValidator : AbstractValidator<ActivityPayrollDto>
{
    public ActivityPayrollDtoValidator()
    {
        RuleFor(x => x.ActivityCode).GreaterThan(0);
        RuleFor(x => x.Payroll).GreaterThanOrEqualTo(0);
        RuleForEach(x => x.EmployeeTypeList).SetValidator(new EmployeeTypeDtoValidator());
    }
}

