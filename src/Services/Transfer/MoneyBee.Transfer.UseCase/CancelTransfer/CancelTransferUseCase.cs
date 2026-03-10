using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;

namespace MoneyBee.Transfer.UseCase.CancelTransfer
{
    public  class CancelTransferUseCase : BaseUseCase<CancelTransferUseCaseInput, CancelTransferUseCaseOutput>
    {
        private readonly ITransferRepository _transferRepository;

        public CancelTransferUseCase(ITransferRepository transferRepository)
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

        protected override async Task<CancelTransferUseCaseOutput> ExecuteInternal(CancellationToken cancellationToken)
        {
            var transfer = await _transferRepository.GetByCodeAsync(Input.TransactionCode, cancellationToken);

            if (transfer == null)
                throw new NotFoundException("Transfer not found.", ErrorCodes.TransferNotFound);

            transfer.Cancel(DateTime.UtcNow);
            await _transferRepository.SaveChangesAsync(cancellationToken);

            return new CancelTransferUseCaseOutput
            {
                Id = transfer.Id,
                TransactionCode = transfer.TransactionCode,
                Status = transfer.Status.ToString(),
                CancelledAtUtc = transfer.CancelledAtUtc
            };
        }
    }
}