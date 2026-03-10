namespace MoneyBee.Transfer.Abstraction.Services
{
    public  class CustomerSummary
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityNumber { get; set; }
    }
}