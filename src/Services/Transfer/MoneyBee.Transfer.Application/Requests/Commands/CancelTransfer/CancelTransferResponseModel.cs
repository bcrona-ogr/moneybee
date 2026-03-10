namespace MoneyBee.Transfer.Application.Requests.Commands.CancelTransfer
{
    public  class CancelTransferResponseModel
    {
        public Guid Id { get; init; }
        public string TransactionCode { get; init; }
        public string Status { get; init; }
        public DateTime? CancelledAtUtc { get; init; }
    }
}