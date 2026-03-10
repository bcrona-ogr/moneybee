using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Customer.UseCase.UpdateCustomer;

namespace MoneyBee.Customer.Application.Requests.Commands.UpdateCustomer
{
    public  class UpdateCustomerRequestHandler : BaseRequestHandler<UpdateCustomerRequestModel, UpdateCustomerResponseModel, UpdateCustomerRequestValidator>
    {
        private readonly IUseCase<UpdateCustomerUseCaseInput, UpdateCustomerUseCaseOutput> _useCase;

        public UpdateCustomerRequestHandler(IUseCase<UpdateCustomerUseCaseInput, UpdateCustomerUseCaseOutput> useCase)
        {
            _useCase = useCase;
        }

        protected override async Task<UpdateCustomerResponseModel> HandleInternal(UpdateCustomerRequestModel request, CancellationToken cancellationToken)
        {
            _useCase.SetInput(new UpdateCustomerUseCaseInput
            {
                Id = request.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                
            });

            var result = await _useCase.Execute(cancellationToken);

            return new UpdateCustomerResponseModel
            {
                Id = result.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                PhoneNumber = result.PhoneNumber,
                Address = result.Address,
                DateOfBirth = result.DateOfBirth,
                IdentityNumber = result.IdentityNumber
            };
        }
    }
}