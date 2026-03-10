using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.UseCase.CancelTransfer;

namespace MoneyBee.Transfer.Application.Requests.Commands.CancelTransfer
{
    public  class CancelTransferRequestHandler
        : BaseRequestHandler<CancelTransferRequestModel, CancelTransferResponseModel>
    {
        private readonly IUseCase<CancelTransferUseCaseInput, CancelTransferUseCaseOutput> _useCase;

        public CancelTransferRequestHandler(IUseCase<CancelTransferUseCaseInput, CancelTransferUseCaseOutput> useCase)
        {
            _useCase = useCase;
        }

        protected override async Task<CancelTransferResponseModel> HandleInternal(CancelTransferRequestModel request, CancellationToken cancellationToken)
        {
            _useCase.SetInput(new CancelTransferUseCaseInput
            {
                TransactionCode = request.TransactionCode,
            });

            var result = await _useCase.Execute(cancellationToken);

            return new CancelTransferResponseModel
            {
                Id = result.Id,
                TransactionCode = result.TransactionCode,
                Status = result.Status,
                CancelledAtUtc = result.CancelledAtUtc
            };
        }
    }
}