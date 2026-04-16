using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class PolicyDtoValidator : AbstractValidator<PolicyDto>
{
    public PolicyDtoValidator()
    {
        RuleFor(x => x.PolicyType).NotEmpty();
    }
}

