using MediatR;
using MoneyBee.Shared.Application.Request;

namespace MoneyBee.Transfer.Application.Requests.Commands.CreateTransfer
{
    public  class CreateTransferRequestModel : BaseRequest, IRequest<CreateTransferResponseModel>
    {
        public Guid SenderCustomerId { get; init; }
        public Guid ReceiverCustomerId { get; init; }
        public decimal Amount { get; init; }
        public Guid EmployeeId { get; init; }
        public string IdempotencyKey { get; init; }
    }
}