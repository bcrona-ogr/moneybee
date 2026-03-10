using MediatR;
using MoneyBee.Shared.Application.Request;

namespace MoneyBee.Transfer.Application.Requests.Queries.GetTransferByCode
{
    public  class GetTransferByCodeRequestModel : BaseRequest, IRequest<GetTransferByCodeResponseModel>
    {
        public string TransactionCode { get; init; }
    }
}