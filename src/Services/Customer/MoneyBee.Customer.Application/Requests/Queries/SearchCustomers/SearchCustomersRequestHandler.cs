using MoneyBee.Shared.Application.Request;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Customer.UseCase.SearchCustomers;

namespace MoneyBee.Customer.Application.Requests.Queries.SearchCustomers
{
    public  class SearchCustomersRequestHandler
        : BaseRequestHandler<SearchCustomersRequestModel, SearchCustomersResponseModel>
    {
        private readonly IUseCase<SearchCustomersUseCaseInput, SearchCustomersUseCaseOutput> _useCase;

        public SearchCustomersRequestHandler(IUseCase<SearchCustomersUseCaseInput, SearchCustomersUseCaseOutput> useCase)
        {
            _useCase = useCase;
        }

        protected override async Task<SearchCustomersResponseModel> HandleInternal(SearchCustomersRequestModel request, CancellationToken cancellationToken)
        {
            _useCase.SetInput(new SearchCustomersUseCaseInput
            {
                Query = request.Query,
                
            });

            var result = await _useCase.Execute(cancellationToken);

            return new SearchCustomersResponseModel
            {
                Items = result.Items.Select(x => new SearchCustomersResponseModelItem
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Address = x.Address,
                    DateOfBirth = x.DateOfBirth,
                    IdentityNumber = x.IdentityNumber
                }).ToList()
            };
        }
    }
}