using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Customer.UseCase.CreateCustomer;

namespace MoneyBee.Customer.Application.Requests.Commands.CreateCustomer
{
    public  class CreateCustomerRequestHandler
        : BaseRequestHandler<CreateCustomerRequestModel, CreateCustomerResponseModel, CreateCustomerRequestValidator>
    {
        private readonly IUseCase<CreateCustomerUseCaseInput, CreateCustomerUseCaseOutput> _useCase;

        public CreateCustomerRequestHandler(IUseCase<CreateCustomerUseCaseInput, CreateCustomerUseCaseOutput> useCase)
        {
            _useCase = useCase;
        }

        protected override async Task<CreateCustomerResponseModel> HandleInternal(CreateCustomerRequestModel request, CancellationToken cancellationToken)
        {
            _useCase.SetInput(new CreateCustomerUseCaseInput
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                IdentityNumber = request.IdentityNumber,
                
            });

            var result = await _useCase.Execute(cancellationToken);

            return new CreateCustomerResponseModel
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