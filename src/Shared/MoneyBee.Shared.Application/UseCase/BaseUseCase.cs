namespace MoneyBee.Shared.Application.UseCase
{
    public abstract class BaseUseCase<TIn, TOut> : IUseCase<TIn, TOut>
    {
        public TIn Input { get; private set; }

        public void SetInput(TIn input)
        {
            Input = input;
        }

        public abstract void Validate();

        public virtual Task<TOut> Execute(CancellationToken cancellationToken = default)
        {
            Validate();
            return ExecuteInternal(cancellationToken);
        }

        protected abstract Task<TOut> ExecuteInternal(CancellationToken cancellationToken);
    }

    public abstract class BaseUseCase<TIn> : IUseCase<TIn>
    {
        public TIn Input { get; private set; }

        public void SetInput(TIn input)
        {
            Input = input;
        }

        public abstract void Validate();

        public virtual Task Execute(CancellationToken cancellationToken = default)
        {
            Validate();
            return ExecuteInternal(cancellationToken);
        }

        protected abstract Task ExecuteInternal(CancellationToken cancellationToken);
    }

}