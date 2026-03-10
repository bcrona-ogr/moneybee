namespace MoneyBee.Transfer.Contracts.Responses
{
    public  class TransferHttpResponse
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
    }
}