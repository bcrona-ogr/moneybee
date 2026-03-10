using System.Net;

namespace MoneyBee.Shared.Core.Exceptions
{
    public  class ConcurrencyBusinessException : HttpException
    {
        public ConcurrencyBusinessException(string message, string errorCode = ErrorCodes.ConcurrencyConflict) : base(message, HttpStatusCode.Conflict, errorCode)
        {
        }
    }
}