using CurrencyExchange.Application.DTO;
using CurrencyExchange.Core.Entities;
using System.Threading.Tasks;

namespace CurrencyExchange.Application.Services.Interfaces
{
    public interface ICurrencyRateQueryService
    {
        Task<CurrencyRateQuery[]> GetAsync(CurrencyRateQueryDto queryDto);
    }
}