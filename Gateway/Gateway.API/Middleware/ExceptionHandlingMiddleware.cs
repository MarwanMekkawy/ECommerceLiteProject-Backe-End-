using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Middleware
{
    public class DownstreamServiceException(int statusCode,string? responseBody) : Exception("A downstream service returned an error.")
    {
        public int StatusCode { get; } = statusCode;
        public string? ResponseBody { get; } = responseBody;
    }

    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (DownstreamServiceException ex)
            {
                if (context.Response.HasStarted)
                    throw;

                logger.LogWarning("A downstream service returned status code {StatusCode}.", ex.StatusCode);

                context.Response.Clear();
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = ex.StatusCode,
                        Title = "The request could not be completed.",
                        Detail = GetSafeErrorDetail(ex.ResponseBody)
                    },
                    context.RequestAborted);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // The client disconnected or cancelled the request.
                throw;
            }
            catch (HttpRequestException ex)
            {
                if (context.Response.HasStarted)
                    throw;

                logger.LogError(ex, "A downstream HTTP request failed.");

                context.Response.Clear();

                context.Response.StatusCode = StatusCodes.Status502BadGateway;

                await context.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status502BadGateway,
                        Title = "A downstream service is unavailable.",
                        Detail = "The Gateway could not communicate with a required service."
                    },
                    context.RequestAborted);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                    throw;

                logger.LogError(ex, "An unexpected error occurred in the Gateway.");

                context.Response.Clear();

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An unexpected error occurred.",
                        Detail = "The server could not complete the request."
                    },
                    context.RequestAborted);
            }
        }

        private static string GetSafeErrorDetail(string? responseBody)
        {
            return "The downstream service could not complete the request.";
        }
    }
}
