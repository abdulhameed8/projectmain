using System.Net;
using System.Text.Json;
using FluentValidation;
using SaaS.Platform.API.Application.Common;

namespace SaaS.Platform.API.API.Middleware
{
    /// <summary>
    /// Global exception handling middleware to catch and format exceptions
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ApiResponse<object>
            {
                Success = false,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case ValidationException validationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = "Validation failed";
                    errorResponse.Errors = validationException.Errors
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    _logger.LogWarning(validationException, "Validation error occurred");
                    break;

                case KeyNotFoundException keyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = keyNotFoundException.Message;
                    _logger.LogWarning(keyNotFoundException, "Resource not found");
                    break;

                case UnauthorizedAccessException unauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = "Unauthorized access";
                    _logger.LogWarning(unauthorizedAccessException, "Unauthorized access attempt");
                    break;

                case InvalidOperationException invalidOperationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = invalidOperationException.Message;
                    _logger.LogWarning(invalidOperationException, "Invalid operation");
                    break;

                case ArgumentException argumentException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = argumentException.Message;
                    _logger.LogWarning(argumentException, "Invalid argument");
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An internal server error occurred";
                    errorResponse.Errors.Add("Please contact support if the problem persists");
                    _logger.LogError(exception, "Unexpected error occurred: {Message}", exception.Message);
                    break;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var result = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Extension method to add exception handling middleware
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}