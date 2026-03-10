using System.Net;

namespace MoneyBee.Shared.Core.Exceptions
{
    public  class ArgumentNotValidException : HttpException
    {
        public ArgumentNotValidException(string message, string errorCode = "validation_error")
            : base(message, HttpStatusCode.UnprocessableEntity, errorCode)
        {
        }
    }
}