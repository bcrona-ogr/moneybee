using MoneyBee.Auth.Domain.Entities;

namespace MoneyBee.Auth.Abstraction.Security;

public interface IJwtTokenGenerator
{
    TokenResult  Generate(Employee employee);
}