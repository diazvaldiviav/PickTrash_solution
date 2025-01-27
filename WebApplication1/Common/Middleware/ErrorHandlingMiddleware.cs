using WebApplication1.Common.Exceptions;

namespace WebApplication1.Common.Middleware
{
    // Common/Middleware/ErrorHandlingMiddleware.cs
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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

            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case BaseException baseException:
                    response.StatusCode = baseException.StatusCode;
                    errorResponse.Title = baseException.Title;
                    errorResponse.Message = baseException.Message;
                    break;

                default:
                    // Log the error
                    _logger.LogError(exception, "An unexpected error occurred");

                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    errorResponse.Title = "Server Error";
                    errorResponse.Message = "An unexpected error occurred";
                    break;
            }

            await response.WriteAsJsonAsync(errorResponse);
        }
    }

    // Common/Models/ErrorResponse.cs
    public class ErrorResponse
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
