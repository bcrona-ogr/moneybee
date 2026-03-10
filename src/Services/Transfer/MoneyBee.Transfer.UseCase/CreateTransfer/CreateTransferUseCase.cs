using System.Text.Json;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Abstraction.Services;
using MoneyBee.Transfer.Domain.Entities;

namespace MoneyBee.Transfer.UseCase.CreateTransfer
{
    public  class CreateTransferUseCase : BaseUseCase<CreateTransferUseCaseInput, CreateTransferUseCaseOutput>
    {
        private readonly ITransferRepository _transferRepository;
        private readonly ICustomerQueryService _customerQueryService;
        private readonly ITransactionCodeGenerator _transactionCodeGenerator;
        private readonly IFeeCalculator _feeCalculator;
        private readonly IDailyLimitPolicy _dailyLimitPolicy;
        private readonly ITransferCreationLock _transferCreationLock;
        private readonly IIdempotencyRecordRepository _idempotencyRecordRepository;
        private readonly IRequestHashGenerator _requestHashGenerator;

        public CreateTransferUseCase(ITransferRepository transferRepository, ICustomerQueryService customerQueryService, ITransactionCodeGenerator transactionCodeGenerator, IFeeCalculator feeCalculator,
            IDailyLimitPolicy dailyLimitPolicy, ITransferCreationLock transferCreationLock, IIdempotencyRecordRepository idempotencyRecordRepository, IRequestHashGenerator requestHashGenerator)
        {
            _transferRepository = transferRepository;
            _customerQueryService = customerQueryService;
            _transactionCodeGenerator = transactionCodeGenerator;
            _feeCalculator = feeCalculator;
            _dailyLimitPolicy = dailyLimitPolicy;
            _transferCreationLock = transferCreationLock;
            _idempotencyRecordRepository = idempotencyRecordRepository;
            _requestHashGenerator = requestHashGenerator;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException("Input is required.", ErrorCodes.InputRequired);

            if (Input.SenderCustomerId == Guid.Empty)
                throw new ArgumentNotValidException("Sender customer id is required.", ErrorCodes.SenderCustomerIdRequired);

            if (Input.ReceiverCustomerId == Guid.Empty)
                throw new ArgumentNotValidException("Receiver customer id is required.", ErrorCodes.ReceiverCustomerIdRequired);

            if (Input.EmployeeId == Guid.Empty)
                throw new ArgumentNotValidException("Employee id is required.", ErrorCodes.EmployeeIdRequired);

            if (Input.Amount <= 0)
                throw new ArgumentNotValidException("Amount must be greater than zero.", ErrorCodes.AmountMustBePositive);

            if (string.IsNullOrWhiteSpace(Input.IdempotencyKey))
                throw new ArgumentNotValidException("Idempotency key is required.", ErrorCodes.IdempotencyKeyRequired);
        }

        protected override async Task<CreateTransferUseCaseOutput> ExecuteInternal(CancellationToken cancellationToken)
        {
            var requestHash = _requestHashGenerator.Generate(new
            {
                Input.SenderCustomerId,
                Input.ReceiverCustomerId,
                Input.Amount,
                Input.EmployeeId,
            });

            var existingRecord = await _idempotencyRecordRepository.GetAsync(Input.IdempotencyKey, "CreateTransfer", Input.EmployeeId, cancellationToken);

            if (existingRecord != null)
            {
                if (!string.Equals(existingRecord.RequestHash, requestHash, StringComparison.Ordinal))
                    throw new IdempotencyConflictException("Idempotency key was already used with a different request.");
                var existingResponse = JsonSerializer.Deserialize<CreateTransferUseCaseOutput>(existingRecord.ResponsePayload);
                return existingResponse;
            }

            var sender = await _customerQueryService.GetByIdAsync(Input.SenderCustomerId, cancellationToken);
            if (sender == null)
                throw new NotFoundException("Sender customer not found.", ErrorCodes.SenderCustomerNotFound);

            var receiver = await _customerQueryService.GetByIdAsync(Input.ReceiverCustomerId, cancellationToken);
            if (receiver == null)
                throw new NotFoundException("Receiver customer not found.", ErrorCodes.ReceiverCustomerNotFound);

            var businessDate = DateTime.UtcNow.Date;
            var lockKey = $"transfer:create:sender:{Input.SenderCustomerId}:day:{businessDate:yyyyMMdd}";

            await using var acquiredLock = await _transferCreationLock.AcquireAsync(lockKey, cancellationToken);

            var todaysTransfers = await _transferRepository.GetSenderTransfersForDateAsync(Input.SenderCustomerId, businessDate, cancellationToken);

            await _dailyLimitPolicy.EnsureAllowedAsync(Input.SenderCustomerId, Input.Amount, todaysTransfers, cancellationToken);

            var fee = _feeCalculator.Calculate(Input.Amount);
            var transactionCode = _transactionCodeGenerator.Generate();

            var entity = new TransferTransaction(Guid.NewGuid(), Input.SenderCustomerId, Input.ReceiverCustomerId, Input.Amount, fee, 949, transactionCode, Input.EmployeeId, DateTime.UtcNow);


            var response = new CreateTransferUseCaseOutput
            {
                Id = entity.Id,
                TransactionCode = entity.TransactionCode,
                Amount = entity.Amount,
                Fee = entity.Fee,
                Currency = entity.Currency,
                Status = entity.Status.ToString(),
                SenderCustomerId = entity.SenderCustomerId,
                ReceiverCustomerId = entity.ReceiverCustomerId,
                CreatedAtUtc = entity.CreatedAtUtc
            };
            var record = new IdempotencyRecord(Guid.NewGuid(), Input.IdempotencyKey, "CreateTransfer", Input.EmployeeId, requestHash, JsonSerializer.Serialize(response), 200, DateTime.UtcNow, DateTime.UtcNow.AddHours(24));

            await _transferRepository.AddAsync(entity, cancellationToken);
            await _idempotencyRecordRepository.AddAsync(record, cancellationToken);
            await _transferRepository.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}