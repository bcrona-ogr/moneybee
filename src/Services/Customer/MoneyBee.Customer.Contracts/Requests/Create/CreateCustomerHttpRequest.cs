namespace MoneyBee.Customer.Contracts.Requests.Create
{
    public  class CreateCustomerHttpRequest
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string PhoneNumber { get; init; }
        public string Address { get; init; }
        public DateTime DateOfBirth { get; init; }
        public string IdentityNumber { get; init; }
    }
}