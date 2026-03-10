using MoneyBee.Shared.Core.Entities;
using MoneyBee.Shared.Core.Exceptions;

namespace MoneyBee.Customer.Domain.Entities
{
    public  class Customer : BaseEntity
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Address { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public string IdentityNumber { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        private Customer()
        {
        }

        public Customer(Guid id, string firstName, string lastName, string phoneNumber, string address, DateTime dateOfBirth, string identityNumber, DateTime createdAtUtc)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new BusinessException("First name is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(lastName))
                throw new BusinessException("Last name is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new BusinessException("Phone number is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(address))
                throw new BusinessException("Address is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(identityNumber))
                throw new BusinessException("Identity number is required.", ErrorCodes.InputRequired);

            if (dateOfBirth.Date >= DateTime.UtcNow.Date)
                throw new BusinessException("Date of birth must be in the past.", ErrorCodes.ValidationError);

            Id = id;
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            PhoneNumber = phoneNumber.Trim();
            Address = address.Trim();
            DateOfBirth = dateOfBirth.Date;
            IdentityNumber = identityNumber.Trim();
            CreatedAtUtc = createdAtUtc;
            UpdatedAtUtc = createdAtUtc;
        }

        public void Update(string firstName, string lastName, string phoneNumber, string address, DateTime dateOfBirth, DateTime updatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new BusinessException("First name is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(lastName))
                throw new BusinessException("Last name is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new BusinessException("Phone number is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(address))
                throw new BusinessException("Address is required.", ErrorCodes.InputRequired);

            if (dateOfBirth.Date >= DateTime.UtcNow.Date)
                throw new BusinessException("Date of birth must be in the past.", ErrorCodes.ValidationError);

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            PhoneNumber = phoneNumber.Trim();
            Address = address.Trim();
            DateOfBirth = dateOfBirth.Date;
            UpdatedAtUtc = updatedAtUtc;
        }
    }
}