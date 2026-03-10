namespace MoneyBee.Customer.UseCase.SearchCustomers
{
    public  class SearchCustomersUseCaseInput
    {
        public string Query { get; init; }
        public string CorrelationId { get; init; }
    }
}