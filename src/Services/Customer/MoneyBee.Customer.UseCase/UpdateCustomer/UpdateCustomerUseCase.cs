using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Customer.Abstraction.Persistence;

namespace MoneyBee.Customer.UseCase.UpdateCustomer
{
    public  class UpdateCustomerUseCase : BaseUseCase<UpdateCustomerUseCaseInput, UpdateCustomerUseCaseOutput>
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException("Input is required.", ErrorCodes.InputRequired);

            if (Input.Id == Guid.Empty)
                throw new ArgumentNotValidException("Customer id is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.FirstName))
                throw new ArgumentNotValidException("First name is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.LastName))
                throw new ArgumentNotValidException("Last name is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.PhoneNumber))
                throw new ArgumentNotValidException("Phone number is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(Input.Address))
                throw new ArgumentNotValidException("Address is required.", ErrorCodes.InputRequired);
        }

        protected override async Task<UpdateCustomerUseCaseOutput> ExecuteInternal(CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(Input.Id, cancellationToken);

            if (customer == null)
                throw new NotFoundException("Customer not found.", ErrorCodes.CustomerNotFound);

            customer.Update(Input.FirstName, Input.LastName, Input.PhoneNumber, Input.Address, Input.DateOfBirth, DateTime.UtcNow);

            await _customerRepository.SaveChangesAsync(cancellationToken);

            return new UpdateCustomerUseCaseOutput
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