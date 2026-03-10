using MoneyBee.Transfer.Domain.Entities;

namespace MoneyBee.Transfer.Abstraction.Services
{
    public interface IDailyLimitPolicy
    {
        Task EnsureAllowedAsync(
            Guid senderCustomerId,
            decimal newAmount,
            List<TransferTransaction> todaysTransfers,
            CancellationToken cancellationToken);
    }
}