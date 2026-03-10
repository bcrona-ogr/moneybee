namespace MoneyBee.Transfer.Infrastructure.Services
{
    internal sealed class InternalCustomerSummaryResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityNumber { get; set; }
    }
}