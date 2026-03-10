using MoneyBee.Transfer.Abstraction.Services;

namespace MoneyBee.Transfer.Infrastructure.Services
{
    public  class FlatFeeCalculator : IFeeCalculator
    {
        public decimal Calculate(decimal amount)
        {
            return 20m;
        }
    }
}