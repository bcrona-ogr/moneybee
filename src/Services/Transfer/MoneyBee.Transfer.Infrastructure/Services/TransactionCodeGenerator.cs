using MoneyBee.Transfer.Abstraction.Services;

namespace MoneyBee.Transfer.Infrastructure.Services
{
    public  class TransactionCodeGenerator : ITransactionCodeGenerator
    {
        public string Generate()
        {
            return $"TRX-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
        }
    }
}