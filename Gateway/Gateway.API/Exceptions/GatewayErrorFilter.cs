using HotChocolate.Execution;
using System.Text.Json;

namespace Gateway.API.Exceptions
{
    public class GatewayErrorFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            if (error.Exception is null)
                return error;

            var mapped = GatewayError.From(error.Exception);

            return ErrorBuilder.FromError(error)
                .SetMessage(mapped.Message)
                .SetCode(mapped.Code)
                .SetExtension("statusCode", mapped.Status)
                .SetException(null)
                .Build();
        }
    }
}
