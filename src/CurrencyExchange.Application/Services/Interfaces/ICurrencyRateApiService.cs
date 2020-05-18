using CurrencyExchange.Application.DTO;
using System.Threading.Tasks;

namespace CurrencyExchange.Application.Services.Interfaces
{
    public interface ICurrencyRateApiService
    {
        Task<CurrencyRateDto[]> GetCurrencyRatesAsync(CurrencyRateQueryDto queries);
    }
}