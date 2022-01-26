using System.Net;

namespace Tgs.Payment.PrimePay;

public class ApiException : Exception
{
    public string ErrorResponseBody { get; }

    public HttpStatusCode StatusCode { get; }

    public ApiException(HttpStatusCode statusCode, string errorResponseBody)
    {
        StatusCode = statusCode;
        ErrorResponseBody = errorResponseBody;
    }
}
