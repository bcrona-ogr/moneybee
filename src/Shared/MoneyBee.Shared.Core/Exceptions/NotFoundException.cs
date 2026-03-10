using System.Net;

namespace MoneyBee.Shared.Core.Exceptions
{
    public  class NotFoundException : HttpException
    {
        public NotFoundException(string message, string errorCode = "not_found")
            : base(message, HttpStatusCode.NotFound, errorCode)
        {
        }
    }
}