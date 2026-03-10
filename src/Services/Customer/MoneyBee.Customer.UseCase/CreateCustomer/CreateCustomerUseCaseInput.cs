namespace MoneyBee.Customer.UseCase.CreateCustomer
{
    public  class CreateCustomerUseCaseInput
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string PhoneNumber { get; init; }
        public string Address { get; init; }
        public DateTime DateOfBirth { get; init; }
        public string IdentityNumber { get; init; }
    }
}

