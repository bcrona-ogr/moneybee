namespace MoneyBee.Transfer.Contracts.Requests.Create
{
    public  class CreateTransferHttpRequest
    {
        public Guid SenderCustomerId { get; init; }
        public Guid ReceiverCustomerId { get; init; }
        public decimal Amount { get; init; }
    }
}