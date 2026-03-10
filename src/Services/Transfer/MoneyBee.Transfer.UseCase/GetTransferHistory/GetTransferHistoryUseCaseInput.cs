namespace MoneyBee.Transfer.UseCase.GetTransferHistory
{
    public  class GetTransferHistoryUseCaseInput
    {
        public Guid CustomerId { get; init; }
        public string Role { get; init; }
    }
}