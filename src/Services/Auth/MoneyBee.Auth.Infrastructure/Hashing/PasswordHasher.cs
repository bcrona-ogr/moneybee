using System.Security.Cryptography;
using System.Text;
using MoneyBee.Auth.Abstraction.Security;

namespace MoneyBee.Auth.Infrastructure.Hashing;

public  class PasswordHasher : IPasswordHasher
{
    public (string hash, string salt) Hash(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(32);
        var hashBytes = SHA256.HashData(Combine(Encoding.UTF8.GetBytes(password), saltBytes));

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public bool Verify(string password, string passwordHash, string passwordSalt)
    {
        var saltBytes = Convert.FromBase64String(passwordSalt);
        var computedHash = SHA256.HashData(Combine(Encoding.UTF8.GetBytes(password), saltBytes));
        var computedHashBase64 = Convert.ToBase64String(computedHash);

        return computedHashBase64 == passwordHash;
    }

    private static byte[] Combine(byte[] first, byte[] second)
    {
        var result = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, result, 0, first.Length);
        Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
        return result;
    }
}