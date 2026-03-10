using FluentValidation;

namespace MoneyBee.Customer.Application.Requests.Commands.CreateCustomer
{
    public  class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequestModel>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
            RuleFor(x => x.IdentityNumber).NotEmpty().MaximumLength(50);
            RuleFor(x => x.DateOfBirth).LessThan(DateTime.UtcNow.Date);
        }
    }
}