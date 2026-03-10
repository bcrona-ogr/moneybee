using MoneyBee.Shared.Core.Entities;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Domain.Enums;

namespace MoneyBee.Transfer.Domain.Entities
{
    public  class TransferTransaction : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid SenderCustomerId { get; private set; }
        public Guid ReceiverCustomerId { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
        public int Currency { get; private set; } = 949; // Default code for Turkish Lira
        public string TransactionCode { get; private set; }
        public TransferStatus Status { get; private set; }
        public Guid CreatedByEmployeeId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? CompletedAtUtc { get; private set; }
        public DateTime? CancelledAtUtc { get; private set; }
        public uint Version { get; private set; }

        private TransferTransaction()
        {
        }

        public TransferTransaction(Guid id, Guid senderCustomerId, Guid receiverCustomerId, decimal amount, decimal fee, int currency, string transactionCode, Guid createdByEmployeeId, DateTime createdAtUtc)
        {
            if (senderCustomerId == Guid.Empty)
                throw new BusinessException("Sender customer id is required.", ErrorCodes.SenderCustomerIdRequired);

            if (receiverCustomerId == Guid.Empty)
                throw new BusinessException("Receiver customer id is required.", ErrorCodes.ReceiverCustomerIdRequired);

            if (senderCustomerId == receiverCustomerId)
                throw new BusinessException("Sender and receiver cannot be the same.", ErrorCodes.ValidationError);

            if (amount <= 0)
                throw new BusinessException("Amount must be greater than zero.", ErrorCodes.AmountMustBePositive);

            if (fee < 0)
                throw new BusinessException("Fee cannot be negative.", ErrorCodes.ValidationError);

            if (Currency <= 0)
                throw new BusinessException("Currency is required.", ErrorCodes.ValidationError);

            if (string.IsNullOrWhiteSpace(transactionCode))
                throw new BusinessException("Transaction code is required.", ErrorCodes.TransactionCodeRequired);

            if (createdByEmployeeId == Guid.Empty)
                throw new BusinessException("Employee id is required.", ErrorCodes.EmployeeIdRequired);

            Id = id;
            SenderCustomerId = senderCustomerId;
            ReceiverCustomerId = receiverCustomerId;
            Amount = amount;
            Fee = fee;
            Currency = currency;
            TransactionCode = transactionCode.Trim();
            CreatedByEmployeeId = createdByEmployeeId;
            CreatedAtUtc = createdAtUtc;
            Status = TransferStatus.ReadyForPickup;
            Version = 1;

        }

        public void Complete(DateTime completedAtUtc)
        {
            switch (Status)
            {
                case TransferStatus.Cancelled:
                    throw new BusinessException("Cancelled transfer cannot be completed.", ErrorCodes.CancelledTransferCannotBeCompleted);

                case TransferStatus.Completed:
                    throw new BusinessException("Transfer is already completed.", ErrorCodes.TransferAlreadyCompleted);
            }

            Status = TransferStatus.Completed;
            CompletedAtUtc = completedAtUtc;
            Version++;
        }

        public void Cancel(DateTime cancelledAtUtc)
        {
            switch (Status)
            {
                case TransferStatus.Completed:
                    throw new BusinessException("Completed transfer cannot be cancelled.", ErrorCodes.CompletedTransferCannotBeCancelled);

                case TransferStatus.Cancelled:
                    throw new BusinessException("Transfer is already cancelled.", ErrorCodes.TransferAlreadyCancelled);
            }

            Status = TransferStatus.Cancelled;
            CancelledAtUtc = cancelledAtUtc;
            Version++;
        }
    }
}