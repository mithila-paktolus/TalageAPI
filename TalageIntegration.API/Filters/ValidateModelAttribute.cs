using Microsoft.AspNetCore.Mvc.Filters;
using TalageIntegration.Shared.Exceptions;
using TalageIntegration.Shared.Models;

namespace TalageIntegration.API.Filters;

public sealed class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        var errors = context.ModelState
            .Where(kvp => kvp.Value is not null && kvp.Value.Errors.Count > 0)
            .SelectMany(kvp => kvp.Value!.Errors.Select(error => new ApiError(
                Code: "validation_error",
                Message: string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Validation error." : error.ErrorMessage,
                Field: kvp.Key)))
            .ToArray();

        throw new RequestValidationException("Validation failed.", errors);
    }
}

