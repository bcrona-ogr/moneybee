namespace MoneyBee.Shared.Application.UseCase
{
    public interface IUseCase
    {
    }

    public interface IUseCase<TIn, TOut> : IUseCase
    {
        TIn Input { get; }

        void SetInput(TIn input);

        Task<TOut> Execute(CancellationToken cancellationToken = default);
    }

    public interface IUseCase<TIn> : IUseCase
    {
        TIn Input { get; }

        void SetInput(TIn input);

        Task Execute(CancellationToken cancellationToken = default);
    }

}