using Domain.Exceptions;
using IdentityService.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace IdentityService.API.Middleware
{
    public class GlobalHandlingMiddleware(RequestDelegate _next, ILogger<GlobalHandlingMiddleware> _logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong:{ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = ex switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                BadRequestException => (int)HttpStatusCode.BadRequest,
                ConflictException => (int)HttpStatusCode.Conflict,
                ForbiddenException => (int)HttpStatusCode.Forbidden,
                UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                InternalServerErrorException => (int)HttpStatusCode.InternalServerError,
                InvalidTokenException => (int)HttpStatusCode.Unauthorized,
                TooManyRequestsException => (int)HttpStatusCode.TooManyRequests,
                _ => (int)HttpStatusCode.InternalServerError,
            };

            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new { error = ex.Message });
            return context.Response.WriteAsync(result);
        }
    }
}
