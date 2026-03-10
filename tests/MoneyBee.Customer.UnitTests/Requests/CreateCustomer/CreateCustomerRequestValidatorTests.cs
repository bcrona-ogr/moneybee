using FluentAssertions;
using MoneyBee.Customer.Application.Requests.Commands.CreateCustomer;

namespace MoneyBee.Customer.UnitTests.Requests.CreateCustomer
{
    public  class CreateCustomerRequestValidatorTests
    {
        private readonly CreateCustomerRequestValidator _validator;

        public CreateCustomerRequestValidatorTests()
        {
            _validator = new CreateCustomerRequestValidator();
        }

        [Fact]
        public async Task ValidateAsync_Should_BeValid_When_RequestValid()
        {
            var request = new CreateCustomerRequestModel
            {
                FirstName = "Ali",
                LastName = "Veli",
                PhoneNumber = "5551112233",
                Address = "Istanbul",
                DateOfBirth = new DateTime(1990, 1, 1),
                IdentityNumber = "12345678901"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateAsync_Should_BeInvalid_When_FirstNameEmpty()
        {
            var request = new CreateCustomerRequestModel
            {
                FirstName = "",
                LastName = "Veli",
                PhoneNumber = "5551112233",
                Address = "Istanbul",
                DateOfBirth = new DateTime(1990, 1, 1),
                IdentityNumber = "12345678901"
            };

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "FirstName");
        }
    }
}