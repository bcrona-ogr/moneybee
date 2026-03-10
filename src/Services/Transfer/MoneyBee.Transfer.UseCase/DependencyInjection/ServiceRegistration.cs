using Microsoft.Extensions.DependencyInjection;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Transfer.UseCase.CancelTransfer;
using MoneyBee.Transfer.UseCase.CompleteTransfer;
using MoneyBee.Transfer.UseCase.CreateTransfer;
using MoneyBee.Transfer.UseCase.GetTransferByCode;
using MoneyBee.Transfer.UseCase.GetTransferHistory;

namespace MoneyBee.Transfer.UseCase.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddTransferUseCases(this IServiceCollection services)
        {
            services.AddScoped<CreateTransferUseCase>();
            services.AddScoped<IUseCase<CreateTransferUseCaseInput, CreateTransferUseCaseOutput>, CreateTransferUseCase>();

            services.AddScoped<GetTransferByCodeUseCase>();
            services.AddScoped<IUseCase<GetTransferByCodeUseCaseInput, GetTransferByCodeUseCaseOutput>, GetTransferByCodeUseCase>();

            services.AddScoped<CompleteTransferUseCase>();
            services.AddScoped<IUseCase<CompleteTransferUseCaseInput, CompleteTransferUseCaseOutput>, CompleteTransferUseCase>();

            services.AddScoped<CancelTransferUseCase>();
            services.AddScoped<IUseCase<CancelTransferUseCaseInput, CancelTransferUseCaseOutput>, CancelTransferUseCase>();

            services.AddScoped<GetTransferHistoryUseCase>();
            services.AddScoped<IUseCase<GetTransferHistoryUseCaseInput, GetTransferHistoryUseCaseOutput>, GetTransferHistoryUseCase>();

            return services;
        }
    }
}