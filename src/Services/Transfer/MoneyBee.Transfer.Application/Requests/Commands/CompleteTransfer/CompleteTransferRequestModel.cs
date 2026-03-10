using MediatR;
using MoneyBee.Shared.Application.Request;

namespace MoneyBee.Transfer.Application.Requests.Commands.CompleteTransfer
{
    public  class CompleteTransferRequestModel : BaseRequest, IRequest<CompleteTransferResponseModel>
    {
        public string TransactionCode { get; init; }
        public Guid ReceiverCustomerId { get; init; }
    }
}