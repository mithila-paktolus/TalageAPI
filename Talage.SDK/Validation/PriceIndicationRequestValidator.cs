using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class PriceIndicationRequestValidator : AbstractValidator<PriceIndicationRequestDto>
{
    public PriceIndicationRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}


