using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class RequestBindQuoteRequestValidator : AbstractValidator<RequestBindQuoteRequestDto>
{
    public RequestBindQuoteRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.QuoteId).NotEmpty();
    }
}


