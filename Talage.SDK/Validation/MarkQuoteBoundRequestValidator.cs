using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class MarkQuoteBoundRequestValidator : AbstractValidator<MarkQuoteBoundRequestDto>
{
    public MarkQuoteBoundRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.QuoteId).NotEmpty();
        RuleFor(x => x.MarkAsBound).Equal(true);
    }
}


