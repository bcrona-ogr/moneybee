using MediatR;
using MoneyBee.Shared.Application.Request;

namespace MoneyBee.Customer.Application.Requests.Commands.CreateCustomer
{
    public  class CreateCustomerRequestModel : BaseRequest, IRequest<CreateCustomerResponseModel>
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string PhoneNumber { get; init; }
        public string Address { get; init; }
        public DateTime DateOfBirth { get; init; }
        public string IdentityNumber { get; init; }
    }
}