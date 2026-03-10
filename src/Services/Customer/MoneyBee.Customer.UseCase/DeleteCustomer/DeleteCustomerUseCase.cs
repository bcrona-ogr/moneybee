using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;
using MoneyBee.Customer.Abstraction.Persistence;

namespace MoneyBee.Customer.UseCase.DeleteCustomer
{
    public  class DeleteCustomerUseCase : BaseUseCase<DeleteCustomerUseCaseInput>
    {
        private readonly ICustomerRepository _customerRepository;

        public DeleteCustomerUseCase(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public override void Validate()
        {
            if (Input == null)
                throw new ArgumentNotValidException("Input is required.", ErrorCodes.InputRequired);

            if (Input.Id == Guid.Empty)
                throw new ArgumentNotValidException("Customer id is required.", ErrorCodes.InputRequired);
        }

        protected override async Task ExecuteInternal(CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(Input.Id, cancellationToken);

            if (customer == null)
                throw new NotFoundException("Customer not found.", ErrorCodes.CustomerNotFound);

            await _customerRepository.DeleteAsync(customer, cancellationToken);
            await _customerRepository.SaveChangesAsync(cancellationToken);

        }
    }
}