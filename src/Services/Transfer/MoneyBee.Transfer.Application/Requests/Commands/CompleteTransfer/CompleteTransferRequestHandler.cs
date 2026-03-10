using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.UseCase.CompleteTransfer;

namespace MoneyBee.Transfer.Application.Requests.Commands.CompleteTransfer
{
    public  class CompleteTransferRequestHandler : BaseRequestHandler<CompleteTransferRequestModel, CompleteTransferResponseModel>
    {
        private readonly IUseCase<CompleteTransferUseCaseInput, CompleteTransferUseCaseOutput> _useCase;

        public CompleteTransferRequestHandler(IUseCase<CompleteTransferUseCaseInput, CompleteTransferUseCaseOutput> useCase)
        {
            _useCase = useCase;
        }

        protected override async Task<CompleteTransferResponseModel> HandleInternal(CompleteTransferRequestModel request, CancellationToken cancellationToken)
        {
            _useCase.SetInput(new CompleteTransferUseCaseInput
            {
                TransactionCode = request.TransactionCode,
                ReceiverCustomerId = request.ReceiverCustomerId
            });

            var result = await _useCase.Execute(cancellationToken);

            return new CompleteTransferResponseModel
            {
                Id = result.Id,
                TransactionCode = result.TransactionCode,
                Status = result.Status,
                CompletedAtUtc = result.CompletedAtUtc
            };
        }
    }
}