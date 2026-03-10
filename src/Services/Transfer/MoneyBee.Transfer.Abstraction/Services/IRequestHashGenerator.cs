namespace MoneyBee.Transfer.Abstraction.Services
{
    public interface IRequestHashGenerator
    {
        string Generate(object request);
    }
}