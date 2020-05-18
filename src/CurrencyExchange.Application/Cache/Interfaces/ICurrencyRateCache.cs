using CurrencyExchange.Core.Entities;

namespace CurrencyExchange.Application.Cache.Interfaces
{
    public interface ICurrencyRateCache
    {
        bool TryGet(CurrencyRateQuery query, out CurrencyRate currencyRate);

        void AddRange(CurrencyRate[] currencyRates);
    }
}