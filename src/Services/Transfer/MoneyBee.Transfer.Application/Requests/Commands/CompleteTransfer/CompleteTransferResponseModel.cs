namespace MoneyBee.Transfer.Application.Requests.Commands.CompleteTransfer
{
    public  class CompleteTransferResponseModel
    {
        public Guid Id { get; init; }
        public string TransactionCode { get; init; }
        public string Status { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
    }
}