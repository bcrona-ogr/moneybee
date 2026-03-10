using MoneyBee.Auth.Domain.Enums;
using MoneyBee.Shared.Core.Entities;
using MoneyBee.Shared.Core.Exceptions;

namespace MoneyBee.Auth.Domain.Entities
{
    public  class Employee : BaseEntity
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; } = null;
        public string PasswordHash { get; private set; } = null;
        public string PasswordSalt { get; private set; } = null;
        public EmployeeStatus Status { get; private set; }

        private Employee()
        {
        }

        public Employee(Guid id, string username, string passwordHash, string passwordSalt, EmployeeStatus status)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new BusinessException("Username is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new BusinessException("Password hash is required.", ErrorCodes.InputRequired);

            if (string.IsNullOrWhiteSpace(passwordSalt))
                throw new BusinessException("Password salt is required.", ErrorCodes.InputRequired);

            Id = id;
            Username = username.Trim();
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            Status = status;
        }

        public bool IsActive() => Status == EmployeeStatus.Active;
    }
}