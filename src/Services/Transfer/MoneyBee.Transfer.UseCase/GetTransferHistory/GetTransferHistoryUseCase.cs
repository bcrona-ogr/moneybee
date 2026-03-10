using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;

namespace MoneyBee.Transfer.UseCase.GetTransferHistory
{
    public  class GetTransferHistoryUseCase : BaseUseCase<GetTransferHistoryUseCaseInput, GetTransferHistoryUseCaseOutput>
    {
        private readonly ITransferRepository _transferRepository;

        public GetTransferHistoryUseCase(ITransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException("Input is required.", ErrorCodes.InputRequired);

            if (Input.CustomerId == Guid.Empty)
                throw new ArgumentNotValidException("Customer id is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.Role))
                throw new ArgumentNotValidException("Role is required.", ErrorCodes.InputRequired);
        }

        protected override async Task<GetTransferHistoryUseCaseOutput> ExecuteInternal(CancellationToken cancellationToken)
        {
            var transfers = Input.Role.Equals("receiver", StringComparison.OrdinalIgnoreCase)
                ? await _transferRepository.GetByReceiverCustomerIdAsync(Input.CustomerId, cancellationToken)
                : await _transferRepository.GetBySenderCustomerIdAsync(Input.CustomerId, cancellationToken);

            return new GetTransferHistoryUseCaseOutput
            {
                Items = transfers.Select(x => new GetTransferHistoryUseCaseOutputItem
                {
                    Id = x.Id,
                    TransactionCode = x.TransactionCode,
                    Amount = x.Amount,
                    Fee = x.Fee,
                    Currency = x.Currency,
                    Status = x.Status.ToString(),
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