using System.Net;

namespace MoneyBee.Shared.Core.Exceptions
{
    public  class IdempotencyConflictException : HttpException
    {
        public IdempotencyConflictException(string message, string errorCode = ErrorCodes.IdempotencyConflict) : base(message, HttpStatusCode.Conflict, errorCode)
        {
        }
    }
}