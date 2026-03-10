using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Customer.Abstraction.Persistence;

namespace MoneyBee.Customer.UseCase.SearchCustomers
{
    public  class SearchCustomersUseCase : BaseUseCase<SearchCustomersUseCaseInput, SearchCustomersUseCaseOutput>
    {
        private readonly ICustomerRepository _customerRepository;

        public SearchCustomersUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public override void Validate()
        {
        }

        protected override async Task<SearchCustomersUseCaseOutput> ExecuteInternal(CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.SearchAsync(Input?.Query, cancellationToken);

            return new SearchCustomersUseCaseOutput
            {
                Items = customers.Select(x => new SearchCustomersUseCaseOutputItem
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