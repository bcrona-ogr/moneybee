namespace MoneyBee.Transfer.Contracts.Responses.History
{
    public  class TransferHistoryItemHttpResponse
    {
        public Guid Id { get; set; }
        public string TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public int Currency { get; set; }
        public string Status { get; set; }
        public Guid SenderCustomerId { get; set; }
        public Guid ReceiverCustomerId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public DateTime? CancelledAtUtc { get; set; }
    }
}