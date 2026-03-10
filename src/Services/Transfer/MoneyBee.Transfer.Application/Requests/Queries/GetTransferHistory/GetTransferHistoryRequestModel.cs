using MediatR;
using MoneyBee.Shared.Application.Request;

namespace MoneyBee.Transfer.Application.Requests.Queries.GetTransferHistory
{
    public  class GetTransferHistoryRequestModel : BaseRequest, IRequest<GetTransferHistoryResponseModel>
    {
        public Guid CustomerId { get; init; }
        public string Role { get; init; }
    }
}