using System.Collections.Concurrent;
using MoneyBee.Transfer.Abstraction.Services;

namespace MoneyBee.Transfer.Infrastructure.Services
{
    public  class InMemoryTransferCreationLock : ITransferCreationLock
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();

        public async Task<IAsyncDisposable> AcquireAsync(string key, CancellationToken cancellationToken)
        {
            var semaphore = Locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync(cancellationToken);

            return new Releaser(semaphore);
        }

        private sealed class Releaser : IAsyncDisposable
        {
            private readonly SemaphoreSlim _semaphore;

            public Releaser(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public ValueTask DisposeAsync()
            {
                _semaphore.Release();
                return ValueTask.CompletedTask;
            }
        }
    }
}