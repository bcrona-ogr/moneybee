using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Customer.UseCase.GetCustomerById;

namespace MoneyBee.Customer.Application.Requests.Queries.GetCustomerById
{
    public  class GetCustomerByIdRequestHandler : BaseRequestHandler<GetCustomerByIdRequestModel, GetCustomerByIdResponseModel>
    {
        private readonly IUseCase<GetCustomerByIdUseCaseInput, GetCustomerByIdUseCaseOutput> _useCase;

        public GetCustomerByIdRequestHandler(IUseCase<GetCustomerByIdUseCaseInput, GetCustomerByIdUseCaseOutput> useCase)
        {
            _useCase = useCase;
        }

        protected override async Task<GetCustomerByIdResponseModel> HandleInternal(GetCustomerByIdRequestModel request, CancellationToken cancellationToken)
        {
            _useCase.SetInput(new GetCustomerByIdUseCaseInput
            {
                Id = request.Id,
                
            });

            var result = await _useCase.Execute(cancellationToken);

            return new GetCustomerByIdResponseModel
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