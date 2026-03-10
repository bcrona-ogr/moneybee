using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.UseCase.GetTransferHistory;

namespace MoneyBee.Transfer.Application.Requests.Queries.GetTransferHistory
{
    public  class GetTransferHistoryRequestHandler : BaseRequestHandler<GetTransferHistoryRequestModel, GetTransferHistoryResponseModel>
    {
        private readonly IUseCase<GetTransferHistoryUseCaseInput, GetTransferHistoryUseCaseOutput> _useCase;

        public GetTransferHistoryRequestHandler(IUseCase<GetTransferHistoryUseCaseInput, GetTransferHistoryUseCaseOutput> useCase)
        {
            _useCase = useCase;
        }

        protected override async Task<GetTransferHistoryResponseModel> HandleInternal(GetTransferHistoryRequestModel request, CancellationToken cancellationToken)
        {
            _useCase.SetInput(new GetTransferHistoryUseCaseInput
            {
                CustomerId = request.CustomerId,
                Role = request.Role,
            });

            var result = await _useCase.Execute(cancellationToken);

            return new GetTransferHistoryResponseModel
            {
                Items = result.Items.Select(x => new GetTransferHistoryResponseModelItem
                {
                    Id = x.Id,
                    TransactionCode = x.TransactionCode,
                    Amount = x.Amount,
                    Fee = x.Fee,
                    Currency = x.Currency,
                    Status = x.Status,
                    SenderCustomerId = x.SenderCustomerId,
                    ReceiverCustomerId = x.ReceiverCustomerId,
                    CreatedAtUtc = x.CreatedAtUtc,
                    CompletedAtUtc = x.CompletedAtUtc,
                    CancelledAtUtc = x.CancelledAtUtc
                }).ToList()
            };
        }
    }
}