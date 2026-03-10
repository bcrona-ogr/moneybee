using Microsoft.EntityFrameworkCore;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Domain.Entities;
using MoneyBee.Transfer.Repository.Persistence.DbContexts;

namespace MoneyBee.Transfer.Repository.Persistence.Repositories
{
    public  class TransferRepository : ITransferRepository
    {
        private readonly TransferDbContext _dbContext;

        public TransferRepository(TransferDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TransferTransaction transfer, CancellationToken cancellationToken)
        {
            await _dbContext.Transfers.AddAsync(transfer, cancellationToken);
        }

        public async Task<TransferTransaction> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Transfers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TransferTransaction> GetByCodeAsync(string transactionCode, CancellationToken cancellationToken)
        {
            return await _dbContext.Transfers.FirstOrDefaultAsync(x => x.TransactionCode == transactionCode, cancellationToken);
        }

        public async Task<List<TransferTransaction>> GetBySenderCustomerIdAsync(Guid senderCustomerId, CancellationToken cancellationToken)
        {
            return await _dbContext.Transfers
                .Where(x => x.SenderCustomerId == senderCustomerId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TransferTransaction>> GetByReceiverCustomerIdAsync(Guid receiverCustomerId, CancellationToken cancellationToken)
        {
            return await _dbContext.Transfers
                .Where(x => x.ReceiverCustomerId == receiverCustomerId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TransferTransaction>> GetSenderTransfersForDateAsync(Guid senderCustomerId, DateTime utcDate, CancellationToken cancellationToken)
        {
            var start = utcDate.Date;
            var end = start.AddDays(1);

            return await _dbContext.Transfers
                .Where(x =>
                    x.SenderCustomerId == senderCustomerId &&
                    x.CreatedAtUtc >= start &&
                    x.CreatedAtUtc < end &&
                    x.Status != Domain.Enums.TransferStatus.Cancelled)
                .ToListAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}