using MediatR;
using MoneyBee.Shared.Application.Request;

namespace MoneyBee.Transfer.Application.Requests.Commands.CancelTransfer
{
    public  class CancelTransferRequestModel : BaseRequest, IRequest<CancelTransferResponseModel>
    {
        public string TransactionCode { get; init; }
    }
}