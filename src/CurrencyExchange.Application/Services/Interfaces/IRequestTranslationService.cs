using CurrencyExchange.Application.DTO;
using CurrencyExchange.Core.Entities;

namespace CurrencyExchange.Application.Services.Interfaces
{
    public interface IRequestTranslationService
    {
        UriWithQueriesDto[] GetURIsWithQueries(CurrencyRateQuery[] queries);
    }
}