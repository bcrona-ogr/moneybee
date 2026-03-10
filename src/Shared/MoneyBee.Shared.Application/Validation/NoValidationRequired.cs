using FluentValidation;

namespace MoneyBee.Shared.Application.Validation;

public  class NoValidationRequired<T> : AbstractValidator<T>
{
}