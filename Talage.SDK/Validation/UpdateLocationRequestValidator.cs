using FluentValidation;
using Talage.SDK.Models;

namespace Talage.SDK.Validation;

public sealed class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationRequestDto>
{
    public UpdateLocationRequestValidator()
    {
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.Location).NotNull();
        RuleFor(x => x.Location.LocationId).NotEmpty();
    }
}


