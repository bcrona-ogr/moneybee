using MediatR;

namespace MoneyBee.Customer.Application.Requests.Queries.SearchCustomers
{
    public  class SearchCustomersRequestModel : IRequest<SearchCustomersResponseModel>
    {
        public string Query { get; init; }
        public string CorrelationId { get; init; }
    }
}