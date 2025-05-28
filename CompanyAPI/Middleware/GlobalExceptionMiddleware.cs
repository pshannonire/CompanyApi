using CompanyAPI.Domain.Exceptions;
using CompanyAPI.Domain.Exceptions.Company;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace CompanyAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                CompanyDomainException domainEx => (HttpStatusCode.BadRequest, domainEx.Message),
                DomainException domainEx => (HttpStatusCode.BadRequest, domainEx.Message),
                ValidationException validationEx => (HttpStatusCode.BadRequest,
                    string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage))),
                ArgumentException argEx => (HttpStatusCode.BadRequest, argEx.Message),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                isSuccess = false,
                value = (object?)null,
                error = message
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
