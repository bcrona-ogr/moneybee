using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Transfer.Abstraction.Services;
using MoneyBee.Transfer.Domain.Entities;

namespace MoneyBee.Transfer.Infrastructure.Services
{
    public  class DailyLimitPolicy : IDailyLimitPolicy
    {
        private const decimal DailyLimit = 100000m;

        public Task EnsureAllowedAsync(Guid senderCustomerId, decimal newAmount, List<TransferTransaction> todaysTransfers, CancellationToken cancellationToken)
        {
            var todayTotal = todaysTransfers.Sum(x => x.Amount);

            if (todayTotal + newAmount > DailyLimit)
                throw new BusinessException("Daily transfer limit exceeded.", ErrorCodes.DailyLimitExceeded);

            return Task.CompletedTask;
        }
    }
}