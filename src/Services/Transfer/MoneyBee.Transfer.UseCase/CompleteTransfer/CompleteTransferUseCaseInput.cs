namespace MoneyBee.Transfer.UseCase.CompleteTransfer
{
    public  class CompleteTransferUseCaseInput
    {
        public string TransactionCode { get; init; }
        public Guid ReceiverCustomerId { get; init; }

    }
}