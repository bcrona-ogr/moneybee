using FluentValidation;

namespace MoneyBee.Transfer.Application.Requests.Commands.CreateTransfer
{
    public  class CreateTransferRequestValidator : AbstractValidator<CreateTransferRequestModel>
    {
        public CreateTransferRequestValidator()
        {
            RuleFor(x => x.SenderCustomerId).NotEmpty();
            RuleFor(x => x.ReceiverCustomerId).NotEmpty();
            RuleFor(x => x.EmployeeId).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}