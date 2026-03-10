using MoneyBee.Transfer.Domain.Entities;

namespace MoneyBee.Transfer.Abstraction.Persistence
{
    public interface IIdempotencyRecordRepository
    {
        Task<IdempotencyRecord> GetAsync(
            string idempotencyKey,
            string operationName,
            Guid actorId,
            CancellationToken cancellationToken);

        Task AddAsync(IdempotencyRecord record, CancellationToken cancellationToken);
    }
}