using Microsoft.EntityFrameworkCore;
using MoneyBee.Transfer.Abstraction.Persistence;
using MoneyBee.Transfer.Domain.Entities;
using MoneyBee.Transfer.Repository.Persistence.DbContexts;

namespace MoneyBee.Transfer.Repository.Persistence.Repositories
{
    public  class IdempotencyRecordRepository : IIdempotencyRecordRepository
    {
        private readonly TransferDbContext _dbContext;

        public IdempotencyRecordRepository(TransferDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<IdempotencyRecord> GetAsync(
            string idempotencyKey,
            string operationName,
            Guid actorId,
            CancellationToken cancellationToken)
        {
            return _dbContext.IdempotencyRecords.FirstOrDefaultAsync(
                x => x.IdempotencyKey == idempotencyKey &&
                     x.OperationName == operationName &&
                     x.ActorId == actorId,
                cancellationToken);
        }

        public async Task AddAsync(IdempotencyRecord record, CancellationToken cancellationToken)
        {
            await _dbContext.IdempotencyRecords.AddAsync(record, cancellationToken);
        }
    }
}