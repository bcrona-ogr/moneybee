using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Shared.API.Response;

namespace MoneyBee.Shared.API.Middlewares
{
    public  class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception exception)
        {
            var correlationId = context.TraceIdentifier;

            _logger.LogError(exception, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";
            var errorCode = "internal_server_error";
            string detail = null;

            if (exception is HttpException httpException)
            {
                statusCode = httpException.StatusCode;
                message = httpException.Message;
                errorCode = httpException.ErrorCode;
            }
            else if (_environment.IsDevelopment() || _environment.IsEnvironment("Testing"))
            {
                detail = exception.ToString();
            }

            var response = new ErrorHttpResponse
            {
                Message = message,
                ErrorCode = errorCode,
                StatusCode = (int) statusCode,
                CorrelationId = correlationId,
                Detail = detail
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}