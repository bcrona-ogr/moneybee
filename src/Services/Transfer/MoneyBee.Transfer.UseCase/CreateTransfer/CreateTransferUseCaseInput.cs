namespace MoneyBee.Transfer.UseCase.CreateTransfer
{
    public  class CreateTransferUseCaseInput
    {
        public Guid SenderCustomerId { get; init; }
        public Guid ReceiverCustomerId { get; init; }
        public decimal Amount { get; init; }
        public Guid EmployeeId { get; init; }
        public string IdempotencyKey { get; init; }
        public string CorrelationId { get; init; }
    }
}