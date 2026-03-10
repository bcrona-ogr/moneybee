namespace MoneyBee.Customer.Application.Requests.Commands.UpdateCustomer
{
    public  class UpdateCustomerResponseModel
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string PhoneNumber { get; init; }
        public string Address { get; init; }
        public DateTime DateOfBirth { get; init; }
        public string IdentityNumber { get; init; }
    }
}