using System.Net;

namespace MoneyBee.Shared.Core.Exceptions
{
    public abstract class HttpException : System.Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ErrorCode { get; }

        protected HttpException(string message, HttpStatusCode statusCode, string errorCode) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        protected HttpException(string message, HttpStatusCode statusCode, string errorCode, System.Exception innerException) : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}