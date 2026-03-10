using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;

namespace MoneyBee.Transfer.UseCase.CompleteTransfer
{
    public  class CompleteTransferUseCase : BaseUseCase<CompleteTransferUseCaseInput, CompleteTransferUseCaseOutput>
    {
        private readonly ITransferRepository _transferRepository;

        public CompleteTransferUseCase(ITransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException("Input is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.TransactionCode))
                throw new ArgumentNotValidException("Transaction code is required.", ErrorCodes.TransactionCodeRequired);

            if (Input.ReceiverCustomerId == Guid.Empty)
                throw new ArgumentNotValidException("Receiver customer id is required.", ErrorCodes.ReceiverCustomerIdRequired);
        }

        protected override async Task<CompleteTransferUseCaseOutput> ExecuteInternal(CancellationToken cancellationToken)
        {
            var transfer = await _transferRepository.GetByCodeAsync(Input.TransactionCode, cancellationToken);

            if (transfer == null)
                throw new NotFoundException("Transfer not found.", ErrorCodes.TransferNotFound);

            if (transfer.ReceiverCustomerId != Input.ReceiverCustomerId)
                throw new BusinessException("Receiver customer does not match transfer.", ErrorCodes.ReceiverCustomerMismatch);

            transfer.Complete(DateTime.UtcNow);
            await _transferRepository.SaveChangesAsync(cancellationToken);

            return new CompleteTransferUseCaseOutput
            {
                Id = transfer.Id,
                TransactionCode = transfer.TransactionCode,
                Status = transfer.Status.ToString(),
                CompletedAtUtc = transfer.CompletedAtUtc
            };
        }
    }
}