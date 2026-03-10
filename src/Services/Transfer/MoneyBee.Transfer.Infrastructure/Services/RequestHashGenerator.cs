using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MoneyBee.Transfer.Abstraction.Services;

namespace MoneyBee.Transfer.Infrastructure.Services
{
    public  class RequestHashGenerator : IRequestHashGenerator
    {
        public string Generate(object request)
        {
            var json = JsonSerializer.Serialize(request);
            var bytes = Encoding.UTF8.GetBytes(json);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }
    }
}