namespace MoneyBee.Transfer.UseCase.GetTransferHistory
{
    public  class GetTransferHistoryUseCaseOutputItem
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
        public DateTime? CompletedAtUtc { get; init; }
        public DateTime? CancelledAtUtc { get; init; }
    }
}