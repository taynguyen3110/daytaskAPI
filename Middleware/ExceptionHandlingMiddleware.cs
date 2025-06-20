using System.Net;
using System.Text.Json;
using daytask.Exceptions;
using daytask.Models;

namespace daytask.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

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
                Message = "An error occurred while processing your request.",
                Errors = new List<string>()
            };

            switch (exception)
            {
                case ValidationException validationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = validationException.Message;
                    break;

                case UnauthorizedException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = exception.Message;
                    break;

                case ForbiddenException:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.Message = exception.Message;
                    break;

                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = exception.Message;
                    break;

                case AppException appException:
                    response.StatusCode = appException.StatusCode;
                    errorResponse.Message = appException.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    _logger.LogError(exception, "An unhandled exception occurred");
                    errorResponse.Message = $"An unexpected error occurred: {exception.Message}";
                    errorResponse.Data = exception;
                    break;
            }
            var result = JsonSerializer.Serialize(errorResponse, _jsonSerializerOptions);
            await response.WriteAsync(result);
        }
    }
} 