namespace Gateway.API.Exceptions
{
    public sealed record GatewayError(int Status, string Code, string Message)
    {
        public static GatewayError From(Exception ex) => ex switch
        {
            DownstreamServiceException d => new(d.StatusCode, "DOWNSTREAM_ERROR", d.Message),

            HttpRequestException => new(StatusCodes.Status502BadGateway, "SERVICE_UNAVAILABLE",  "A required service is unavailable."),

            _ => new(StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.")
        };
    }
}