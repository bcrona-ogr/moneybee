namespace MoneyBee.Customer.Contracts.Responses
{
    public  class CustomerHttpResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string IdentityNumber { get; set; }
    }
}