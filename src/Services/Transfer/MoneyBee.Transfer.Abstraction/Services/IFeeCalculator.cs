namespace MoneyBee.Transfer.Abstraction.Services
{
    public interface IFeeCalculator
    {
        decimal Calculate(decimal amount);
    }
}