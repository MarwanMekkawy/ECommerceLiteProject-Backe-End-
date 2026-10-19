using Gateway.API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                    throw;

                var error = GatewayError.From(ex);

                if (error.Status >= 500)
                    logger.LogError(ex, "Request failed with {Code}.", error.Code);
                else
                    logger.LogWarning("Request failed with {Code}: {Message}", error.Code, error.Message);

                context.Response.Clear();
                context.Response.StatusCode = error.Status;

                await context.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = error.Status,
                        Title = "The request could not be completed.",
                        Detail = error.Message,
                        Extensions = { ["code"] = error.Code }
                    },
                    context.RequestAborted);
            }
        }
    }
}
