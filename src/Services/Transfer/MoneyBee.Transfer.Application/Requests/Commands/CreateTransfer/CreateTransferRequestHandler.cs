using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.UseCase.CreateTransfer;

namespace MoneyBee.Transfer.Application.Requests.Commands.CreateTransfer
{
    public  class CreateTransferRequestHandler : BaseRequestHandler<CreateTransferRequestModel, CreateTransferResponseModel, CreateTransferRequestValidator>
    {
        private readonly IUseCase<CreateTransferUseCaseInput, CreateTransferUseCaseOutput> _useCase;

        public CreateTransferRequestHandler(IUseCase<CreateTransferUseCaseInput, CreateTransferUseCaseOutput> useCase)
        {
            _useCase = useCase;
        }

        protected override async Task<CreateTransferResponseModel> HandleInternal(CreateTransferRequestModel request, CancellationToken cancellationToken)
        {
            _useCase.SetInput(new CreateTransferUseCaseInput
            {
                SenderCustomerId = request.SenderCustomerId,
                ReceiverCustomerId = request.ReceiverCustomerId,
                Amount = request.Amount,
                EmployeeId = request.EmployeeId,
                IdempotencyKey = request.IdempotencyKey
            });

            var result = await _useCase.Execute(cancellationToken);

            return new CreateTransferResponseModel
            {
                Id = result.Id,
                TransactionCode = result.TransactionCode,
                Amount = result.Amount,
                Fee = result.Fee,
                Currency = result.Currency,
                Status = result.Status,
                SenderCustomerId = result.SenderCustomerId,
                ReceiverCustomerId = result.ReceiverCustomerId,
                CreatedAtUtc = result.CreatedAtUtc
            };
        }
    }
}