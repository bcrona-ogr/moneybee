using MoneyBee.Shared.Application.Caching.Attributes;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Customer.Abstraction.Persistence;

namespace MoneyBee.Customer.UseCase.GetCustomerById
{
    public class GetCustomerByIdUseCase : BaseUseCase<GetCustomerByIdUseCaseInput, GetCustomerByIdUseCaseOutput>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByIdUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [Caching(60, Prefix = "customer-by-id", VaryByProperties = [nameof(GetCustomerByIdUseCaseInput.Id)])]
        public override Task<GetCustomerByIdUseCaseOutput> Execute(CancellationToken cancellationToken = default)
        {
            return base.Execute(cancellationToken);
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException("Input is required.");

            if (Input.Id == Guid.Empty)
                throw new ArgumentNotValidException("Customer id is required.");
        }

        protected override async Task<GetCustomerByIdUseCaseOutput> ExecuteInternal(CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(Input.Id, cancellationToken);

            if (customer == null)
                throw new NotFoundException("Customer not found.", ErrorCodes.CustomerNotFound);

            return new GetCustomerByIdUseCaseOutput
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                DateOfBirth = customer.DateOfBirth,
                IdentityNumber = customer.IdentityNumber
            };
        }
    }
}