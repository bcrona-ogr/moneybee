namespace MoneyBee.Transfer.Contracts.Requests.Complete
{
    public  class CompleteTransferHttpRequest
    {
        public string TransactionCode { get; init; }
        public Guid ReceiverCustomerId { get; init; }

    }
}