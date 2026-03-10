using MoneyBee.Transfer.Domain.Entities;

namespace MoneyBee.Transfer.Abstraction.Persistence
{
    public interface ITransferRepository
    {
        Task AddAsync(TransferTransaction transfer, CancellationToken cancellationToken);
        Task<TransferTransaction> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<TransferTransaction> GetByCodeAsync(string transactionCode, CancellationToken cancellationToken);
        Task<List<TransferTransaction>> GetBySenderCustomerIdAsync(Guid senderCustomerId, CancellationToken cancellationToken);
        Task<List<TransferTransaction>> GetByReceiverCustomerIdAsync(Guid receiverCustomerId, CancellationToken cancellationToken);
        Task<List<TransferTransaction>> GetSenderTransfersForDateAsync(Guid senderCustomerId, DateTime utcDate, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}