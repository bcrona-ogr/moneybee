using FluentAssertions;
using MoneyBee.Transfer.Application.Requests.Commands.CreateTransfer;

namespace MoneyBee.Transfer.UnitTests.Requests.CreateTransfer
{
    public  class CreateTransferRequestValidatorTests
    {
        private readonly CreateTransferRequestValidator _validator;

        public CreateTransferRequestValidatorTests()
        {
            _validator = new CreateTransferRequestValidator();
        }

        [Fact]
        public async Task ValidateAsync_Should_BeValid_When_RequestValid()
        {
            var request = new CreateTransferRequestModel
            {
                SenderCustomerId = Guid.NewGuid(),
                ReceiverCustomerId = Guid.NewGuid(),
                Amount = 500m,
                EmployeeId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateAsync_Should_BeInvalid_When_SenderCustomerIdEmpty()
        {
            var request = new CreateTransferRequestModel
            {
                SenderCustomerId = Guid.Empty,
                ReceiverCustomerId = Guid.NewGuid(),
                Amount = 500m,
                EmployeeId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "SenderCustomerId");
        }

        [Fact]
        public async Task ValidateAsync_Should_BeInvalid_When_AmountNotPositive()
        {
            var request = new CreateTransferRequestModel
            {
                SenderCustomerId = Guid.NewGuid(),
                ReceiverCustomerId = Guid.NewGuid(),
                Amount = 0m,
                EmployeeId = Guid.NewGuid()
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Amount");
        }
    }
}