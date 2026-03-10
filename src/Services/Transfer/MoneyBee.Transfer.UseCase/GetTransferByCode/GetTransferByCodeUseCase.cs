using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;

namespace MoneyBee.Transfer.UseCase.GetTransferByCode
{
    public  class GetTransferByCodeUseCase : BaseUseCase<GetTransferByCodeUseCaseInput, GetTransferByCodeUseCaseOutput>
    {
        private readonly ITransferRepository _transferRepository;

        public GetTransferByCodeUseCase(ITransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException("Input is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.TransactionCode))
                throw new ArgumentNotValidException("Transaction code is required.", ErrorCodes.TransactionCodeRequired);
        }

        protected override async Task<GetTransferByCodeUseCaseOutput> ExecuteInternal(CancellationToken cancellationToken)
        {
            var transfer = await _transferRepository.GetByCodeAsync(Input.TransactionCode, cancellationToken);

            if (transfer == null)
                throw new NotFoundException("Transfer not found.", ErrorCodes.TransferNotFound);

            return new GetTransferByCodeUseCaseOutput
            {
                Id = transfer.Id,
                TransactionCode = transfer.TransactionCode,
                Amount = transfer.Amount,
                Fee = transfer.Fee,
                Currency = transfer.Currency,
                Status = transfer.Status.ToString(),
                SenderCustomerId = transfer.SenderCustomerId,
                ReceiverCustomerId = transfer.ReceiverCustomerId,
                CreatedAtUtc = transfer.CreatedAtUtc,
                CompletedAtUtc = transfer.CompletedAtUtc,
                CancelledAtUtc = transfer.CancelledAtUtc
            };
        }
    }
}