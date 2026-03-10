using FluentValidation;

namespace MoneyBee.Customer.Application.Requests.Commands.DeleteCustomer
{
    public  class DeleteCustomerRequestValidator : AbstractValidator<DeleteCustomerRequestModel>
    {
        public DeleteCustomerRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}