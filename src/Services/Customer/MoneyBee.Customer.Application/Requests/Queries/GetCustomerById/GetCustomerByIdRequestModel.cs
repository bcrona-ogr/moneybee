using MediatR;

namespace MoneyBee.Customer.Application.Requests.Queries.GetCustomerById
{
    public  class GetCustomerByIdRequestModel : IRequest<GetCustomerByIdResponseModel>
    {
        public Guid Id { get; init; }
        public string CorrelationId { get; init; }
    }
}