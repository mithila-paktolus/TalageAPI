using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class StartQuoteRequestValidator : AbstractValidator<StartQuoteRequestDto>
{
    public StartQuoteRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
    }
}

