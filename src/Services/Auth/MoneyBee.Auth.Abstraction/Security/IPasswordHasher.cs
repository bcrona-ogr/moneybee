namespace MoneyBee.Auth.Abstraction.Security
{
    public interface IPasswordHasher
    {
        bool Verify(string password, string passwordHash, string passwordSalt);
        (string hash, string salt) Hash(string password);
    }
}