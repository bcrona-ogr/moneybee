using System.Net;

namespace MoneyBee.Shared.Core.Exceptions
{
    public class BusinessException : HttpException
    {
        public BusinessException(string message, string errorCode = "business_error") : base(message, HttpStatusCode.BadRequest, errorCode)
        {
        }

        public BusinessException(string message, string errorCode, System.Exception innerException) : base(message, HttpStatusCode.BadRequest, errorCode, innerException)
        {
        }
    }
}