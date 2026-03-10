using MoneyBee.Auth.Abstraction.Persistence;
using MoneyBee.Auth.Abstraction.Security;
using MoneyBee.Shared.Application.UseCase;
using MoneyBee.Shared.Core.Exceptions;

namespace MoneyBee.Auth.UseCase.Login;

public class LoginUseCase : BaseUseCase<LoginUseCaseInput, LoginUseCaseOutput>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUseCase(IEmployeeRepository employeeRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _employeeRepository = employeeRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public override void Validate()
    {
        if (Input is null)
            throw new ArgumentNotValidException("Input is required.", ErrorCodes.InputRequired);

        if (string.IsNullOrWhiteSpace(Input.Username))
            throw new ArgumentNotValidException("Username is required.", ErrorCodes.InputRequired);

        if (string.IsNullOrWhiteSpace(Input.Password))
            throw new ArgumentNotValidException("Password is required.", ErrorCodes.InputRequired);
    }

    protected override async Task<LoginUseCaseOutput> ExecuteInternal(CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByUsernameAsync(Input.Username, cancellationToken);

        if (employee is null)
            throw new BusinessException("Invalid username or password.", ErrorCodes.ValidationError);

        if (!employee.IsActive())
            throw new BusinessException("Employee is passive.", ErrorCodes.ValidationError);

        var isValid = _passwordHasher.Verify(Input.Password, employee.PasswordHash, employee.PasswordSalt);

        if (!isValid)
            throw new BusinessException("Invalid username or password.", ErrorCodes.ValidationError);

        var tokenResult = _jwtTokenGenerator.Generate(employee);

        return new LoginUseCaseOutput
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAtUtc = tokenResult.ExpiresAtUtc,
            TokenType = tokenResult.TokenType
        };
    }
}