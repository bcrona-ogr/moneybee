namespace MoneyBee.Transfer.UseCase.CompleteTransfer
{
    public  class CompleteTransferUseCaseOutput
    {
        public Guid Id { get; init; }
        public string TransactionCode { get; init; }
        public string Status { get; init; }
        public DateTime? CompletedAtUtc { get; init; }
    }
}