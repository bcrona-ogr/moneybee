using MediatR;

namespace MoneyBee.Customer.Application.Requests.Commands.DeleteCustomer
{
    public  class DeleteCustomerRequestModel : IRequest<Unit>
    {
        public Guid Id { get; init; }
        public string CorrelationId { get; init; }
    }
}