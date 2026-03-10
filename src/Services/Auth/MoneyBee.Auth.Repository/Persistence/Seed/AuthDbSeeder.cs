using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoneyBee.Auth.Abstraction.Security;
using MoneyBee.Auth.Domain.Entities;
using MoneyBee.Auth.Domain.Enums;
using MoneyBee.Auth.Repository.Persistence.DbContexts;

namespace MoneyBee.Auth.Repository.Persistence.Seed
{
    public  class AuthDbSeeder
    {
        private readonly AuthDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IOptions<AuthSeedOptions> _options;
        private readonly ILogger<AuthDbSeeder> _logger;

        public AuthDbSeeder(
            AuthDbContext dbContext,
            IPasswordHasher passwordHasher,
            IOptions<AuthSeedOptions> options,
            ILogger<AuthDbSeeder> logger)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _options = options;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var options = _options.Value;

            if (!options.RunOnStartup)
            {
                _logger.LogInformation("Auth seed is disabled.");
                return;
            }

            await _dbContext.Database.MigrateAsync(cancellationToken);

            await SeedEmployeeIfNotExistsAsync(
                username: options.DefaultEmployee?.Username,
                password: options.DefaultEmployee?.Password,
                status: EmployeeStatus.Active,
                logLabel: "default employee",
                cancellationToken: cancellationToken);

            await SeedEmployeeIfNotExistsAsync(
                username: options.PassiveEmployee?.Username,
                password: options.PassiveEmployee?.Password,
                status: EmployeeStatus.Passive,
                logLabel: "passive employee",
                cancellationToken: cancellationToken);
        }

        private async Task SeedEmployeeIfNotExistsAsync(
            string username,
            string password,
            EmployeeStatus status,
            string logLabel,
            CancellationToken cancellationToken)
        {
            username = username?.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("{LogLabel} username is empty. Seed skipped", logLabel);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("{LogLabel} password is empty. Seed skipped for {Username}", logLabel, username);
                return;
            }

            var existingEmployee = await _dbContext.Employees
                .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);

            if (existingEmployee != null)
            {
                _logger.LogInformation("{LogLabel} already exists: {Username}", logLabel, username);
                return;
            }

            var hashResult = _passwordHasher.Hash(password);

            var employee = new Employee(
                Guid.NewGuid(),
                username,
                hashResult.hash,
                hashResult.salt,
                status);

            await _dbContext.Employees.AddAsync(employee, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("{LogLabel} seeded successfully: {Username}", logLabel, username);
        }
    }
}