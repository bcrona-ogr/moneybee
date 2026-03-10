namespace MoneyBee.Transfer.Abstraction.Services
{
    public interface ITransferCreationLock
    {
        Task<IAsyncDisposable> AcquireAsync(string key, CancellationToken cancellationToken);
    }
}