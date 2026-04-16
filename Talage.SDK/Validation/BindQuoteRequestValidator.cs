using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class BindQuoteRequestValidator : AbstractValidator<BindQuoteRequestDto>
{
    public BindQuoteRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.QuoteId).NotEmpty();
    }
}


