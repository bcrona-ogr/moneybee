using MediatR;
using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Customer.UseCase.DeleteCustomer;

namespace MoneyBee.Customer.Application.Requests.Commands.DeleteCustomer
{
    public  class DeleteCustomerRequestHandler : BaseRequestHandler<DeleteCustomerRequestModel, Unit, DeleteCustomerRequestValidator>
    {
        private readonly IUseCase<DeleteCustomerUseCaseInput> _useCase;

        public DeleteCustomerRequestHandler(IUseCase<DeleteCustomerUseCaseInput> useCase)
        {
            _useCase = useCase;
        }

        protected override async Task<Unit> HandleInternal(DeleteCustomerRequestModel request, CancellationToken cancellationToken)
        {
            _useCase.SetInput(new DeleteCustomerUseCaseInput
            {
                Id = request.Id,
                
            });

            await _useCase.Execute(cancellationToken);
            return Unit.Value;
        }
    }
}