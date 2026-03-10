using FluentValidation;

namespace MoneyBee.Customer.Application.Requests.Commands.UpdateCustomer
{
    public  class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequestModel>
    {
        public UpdateCustomerRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
            RuleFor(x => x.DateOfBirth).LessThan(DateTime.UtcNow.Date);
        }
    }
}