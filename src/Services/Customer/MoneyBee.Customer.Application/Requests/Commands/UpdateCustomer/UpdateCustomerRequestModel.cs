using MediatR;
using MoneyBee.Shared.Application.Request;

namespace MoneyBee.Customer.Application.Requests.Commands.UpdateCustomer
{
    public  class UpdateCustomerRequestModel : BaseRequest, IRequest<UpdateCustomerResponseModel>
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string PhoneNumber { get; init; }
        public string Address { get; init; }
        public DateTime DateOfBirth { get; init; }
    }
}