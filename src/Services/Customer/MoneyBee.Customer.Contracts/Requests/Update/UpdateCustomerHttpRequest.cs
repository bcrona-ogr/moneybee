namespace MoneyBee.Customer.Contracts.Requests.Update
{
    public  class UpdateCustomerHttpRequest
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string PhoneNumber { get; init; }
        public string Address { get; init; }
        public DateTime DateOfBirth { get; init; }
    }
}