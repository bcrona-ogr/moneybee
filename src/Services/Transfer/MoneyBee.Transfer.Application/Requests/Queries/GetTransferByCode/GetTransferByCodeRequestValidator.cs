using FluentValidation;

namespace MoneyBee.Transfer.Application.Requests.Queries.GetTransferByCode
{
    public  class GetTransferByCodeRequestValidator : AbstractValidator<GetTransferByCodeRequestModel>
    {
        public GetTransferByCodeRequestValidator()
        {
            RuleFor(x => x.TransactionCode).NotEmpty();
        }
    }
}