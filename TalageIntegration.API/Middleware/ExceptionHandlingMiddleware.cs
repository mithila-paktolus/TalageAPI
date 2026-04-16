using System.Net;
using System.Text.Json;
using FluentValidation;
using Talage.SDK.Internal.Auth;
using Talage.SDK.Internal.ApiClient;
using TalageIntegration.Shared.Exceptions;
using TalageIntegration.Shared.Models;

namespace TalageIntegration.API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation("Request cancelled. {TraceId}", context.TraceIdentifier);
        }
        catch (RequestValidationException exception)
        {
            logger.LogError(exception, "Validation failed for request {TraceId}", context.TraceIdentifier);
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, exception.Message, exception.Errors);
        }
        catch (ValidationException exception)
        {
            var errors = exception.Errors
                .Select(error => new ApiError(
                    Code: "validation_error",
                    Message: error.ErrorMessage,
                    Field: error.PropertyName))
                .ToArray();

            logger.LogError(exception, "Validation failed for request {TraceId}", context.TraceIdentifier);
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, exception.Message, errors);
        }
        catch (TooManyRequestsException exception)
        {
            logger.LogError(exception, "Request throttled for {TraceId}", context.TraceIdentifier);
            await WriteErrorAsync(context, HttpStatusCode.TooManyRequests, exception.Message);
        }
        catch (TalageConfigurationException exception)
        {
            logger.LogError(exception, "Talage configuration error for request {TraceId}", context.TraceIdentifier);
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError, "Service configuration error.");
        }
        catch (TalageApiRequestException exception)
        {
            logger.LogError(
                exception,
                "Talage API request failed. {TraceId} {Operation} {Method} {Uri} {ElapsedMs}",
                context.TraceIdentifier,
                exception.Operation,
                exception.Method,
                exception.Uri,
                exception.ElapsedMilliseconds);

            await WriteErrorAsync(context, exception.MappedStatusCode, exception.Message);
        }
        catch (TalageApiException exception)
        {
            logger.LogError(
                exception,
                "Talage API request failed with upstream status code {StatusCode}. {TraceId}",
                exception.StatusCode,
                context.TraceIdentifier);

            await WriteErrorAsync(context, exception.MappedStatusCode, exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception for request {TraceId}", context.TraceIdentifier);
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message,
        IReadOnlyCollection<ApiError>? errors = null)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = new ApiErrorResponse
        {
            Message = message,
            CorrelationId = context.TraceIdentifier,
            Errors = errors ?? Array.Empty<ApiError>()
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions), context.RequestAborted);
    }
}


