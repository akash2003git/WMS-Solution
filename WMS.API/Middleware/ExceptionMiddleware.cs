using System.Net;
using System.Text.Json;
using WMS.Application.Common.Exceptions;
using WMS.Application.Common.Responses;

namespace WMS.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        object response;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                response = ApiResponse<object>.FailureResponse(
                    validationException.Message,
                    validationException.Errors);

                break;

            case UnauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                response = ApiResponse<object>.FailureResponse(exception.Message);
                break;

            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                response = ApiResponse<object>.FailureResponse(exception.Message);
                break;

            case BusinessRuleException:
                statusCode = HttpStatusCode.BadRequest;
                response = ApiResponse<object>.FailureResponse(exception.Message);
                break;

            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                response = ApiResponse<object>.FailureResponse(exception.Message);
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                response = ApiResponse<object>.FailureResponse(
                    "An unexpected error occurred",
                    _environment.IsDevelopment()
                        ? new List<string> { exception.Message }
                        : null);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}
