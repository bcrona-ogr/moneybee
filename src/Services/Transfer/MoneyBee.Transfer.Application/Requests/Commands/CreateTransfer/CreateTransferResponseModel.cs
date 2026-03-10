namespace MoneyBee.Transfer.Application.Requests.Commands.CreateTransfer
{
    public  class CreateTransferResponseModel
    {
        public Guid Id { get; init; }
        public string TransactionCode { get; init; }
        public decimal Amount { get; init; }
        public decimal Fee { get; init; }
        public int Currency { get; init; }
        public string Status { get; init; }
        public Guid SenderCustomerId { get; init; }
        public Guid ReceiverCustomerId { get; init; }
        public DateTime CreatedAtUtc { get; init; }
    }
}