using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.UseCase.GetTransferByCode;

namespace MoneyBee.Transfer.Application.Requests.Queries.GetTransferByCode
{
    public  class GetTransferByCodeRequestHandler : BaseRequestHandler<GetTransferByCodeRequestModel, GetTransferByCodeResponseModel, GetTransferByCodeRequestValidator>
    {
        private readonly IUseCase<GetTransferByCodeUseCaseInput, GetTransferByCodeUseCaseOutput> _useCase;

        public GetTransferByCodeRequestHandler(IUseCase<GetTransferByCodeUseCaseInput, GetTransferByCodeUseCaseOutput> useCase)
        {
            _useCase = useCase;
        }

        protected override async Task<GetTransferByCodeResponseModel> HandleInternal(GetTransferByCodeRequestModel request, CancellationToken cancellationToken)
        {
            _useCase.SetInput(new GetTransferByCodeUseCaseInput
            {
                TransactionCode = request.TransactionCode,
            });

            var result = await _useCase.Execute(cancellationToken);

            return new GetTransferByCodeResponseModel
            {
                Id = result.Id,
                TransactionCode = result.TransactionCode,
                Amount = result.Amount,
                Fee = result.Fee,
                Currency = result.Currency,
                Status = result.Status,
                SenderCustomerId = result.SenderCustomerId,
                ReceiverCustomerId = result.ReceiverCustomerId,
                CreatedAtUtc = result.CreatedAtUtc,
                CompletedAtUtc = result.CompletedAtUtc,
                CancelledAtUtc = result.CancelledAtUtc
            };
        }
    }
}