using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class DeleteLocationRequestValidator : AbstractValidator<DeleteLocationRequestDto>
{
    public DeleteLocationRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.Delete).Equal(true);
        RuleFor(x => x.Location).NotNull();
        RuleFor(x => x.Location.LocationId).NotEmpty();
    }
}


