namespace MoneyBee.Auth.Repository.Persistence.Seed
{
    public  class AuthSeedOptions
    {
        public bool RunOnStartup { get; set; }
        public AuthSeedEmployeeOptions DefaultEmployee { get; set; } = new();
        public AuthSeedEmployeeOptions PassiveEmployee { get; set; } = new();
    }

    public  class AuthSeedEmployeeOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}