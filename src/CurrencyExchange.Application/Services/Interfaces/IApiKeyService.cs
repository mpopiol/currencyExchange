using System.Threading.Tasks;

namespace CurrencyExchange.Application.Services.Interfaces
{
    public interface IApiKeyService
    {
        Task<string> GetNewApiKeyAsync();

        Task<bool> IsApiKeyValidAsync(string id);
    }
}