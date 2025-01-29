using System.Text;

namespace WebApplication1.Common.Middleware
{
    // Middleware/RequestLoggingMiddleware.cs
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await LogRequest(context);
                await _next(context);
                await LogResponse(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no manejado en la solicitud");
                throw;
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            var requestBody = string.Empty;
            if (context.Request.Body.CanRead)
            {
                using var reader = new StreamReader(
                    context.Request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);

                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            _logger.LogInformation(
                "Request: {Method} {Path} {Body}",
                context.Request.Method,
                context.Request.Path,
                requestBody);
        }

        private Task LogResponse(HttpContext context)
        {
            _logger.LogInformation(
                "Response: {StatusCode} for {Method} {Path}",
                context.Response.StatusCode,
                context.Request.Method,
                context.Request.Path);

            return Task.CompletedTask;
        }
    }
}
