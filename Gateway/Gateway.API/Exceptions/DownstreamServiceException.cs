using System.Text.Json;

namespace Gateway.API.Exceptions
{
    public sealed class DownstreamServiceException : Exception
    {
        public int StatusCode { get; }
        public string? ResponseBody { get; }

        public DownstreamServiceException(int statusCode, string? responseBody)
            : base(ExtractMessage(responseBody))
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        private static string ExtractMessage(string? body)
        {
            const string fallback = "A downstream service returned an error.";

            if (string.IsNullOrWhiteSpace(body))
                return fallback;

            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var key in new[] { "error", "detail", "message", "title" })
                    {
                        if (doc.RootElement.TryGetProperty(key, out var v) &&
                            v.ValueKind == JsonValueKind.String)
                            return v.GetString() ?? fallback;
                    }
                }
            }
            catch (JsonException) { }

            return body;
        }
    }
}