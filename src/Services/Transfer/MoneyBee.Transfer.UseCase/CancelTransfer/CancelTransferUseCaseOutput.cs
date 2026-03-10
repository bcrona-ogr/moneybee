namespace MoneyBee.Transfer.UseCase.CancelTransfer
{
    public  class CancelTransferUseCaseOutput
    {
        public Guid Id { get; init; }
        public string TransactionCode { get; init; }
        public string Status { get; init; }
        public DateTime? CancelledAtUtc { get; init; }
    }
}