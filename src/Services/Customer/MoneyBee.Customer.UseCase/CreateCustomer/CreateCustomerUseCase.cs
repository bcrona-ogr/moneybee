using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Customer.Abstraction.Persistence;

namespace MoneyBee.Customer.UseCase.CreateCustomer
{
    public  class CreateCustomerUseCase : BaseUseCase<CreateCustomerUseCaseInput, CreateCustomerUseCaseOutput>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateCustomerUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException("Input is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.FirstName))
                throw new ArgumentNotValidException("First name is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.LastName))
                throw new ArgumentNotValidException("Last name is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.PhoneNumber))
                throw new ArgumentNotValidException("Phone number is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.Address))
                throw new ArgumentNotValidException("Address is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.IdentityNumber))
                throw new ArgumentNotValidException("Identity number is required.", ErrorCodes.InputRequired);
        }

        protected override async Task<CreateCustomerUseCaseOutput> ExecuteInternal(CancellationToken cancellationToken)
        {
            var existingCustomer = await _customerRepository.GetByIdentityNumberAsync(
                Input.IdentityNumber,
                cancellationToken);

            if (existingCustomer != null)
                throw new BusinessException("Customer with the same identity number already exists.", ErrorCodes.ValidationError);
            var entity = new Domain.Entities.Customer(
                Guid.NewGuid(),
                Input.FirstName,
                Input.LastName,
                Input.PhoneNumber,
                Input.Address,
                Input.DateOfBirth,
                Input.IdentityNumber,
                DateTime.UtcNow);

            await _customerRepository.AddAsync(entity, cancellationToken);
            await _customerRepository.SaveChangesAsync(cancellationToken);

            return new CreateCustomerUseCaseOutput
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PhoneNumber = entity.PhoneNumber,
                Address = entity.Address,
                DateOfBirth = entity.DateOfBirth,
                IdentityNumber = entity.IdentityNumber
            };
        }
    }
}