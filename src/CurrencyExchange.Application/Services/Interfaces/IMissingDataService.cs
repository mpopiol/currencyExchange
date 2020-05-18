using CurrencyExchange.Core.Entities;
using System.Collections.Generic;

namespace CurrencyExchange.Application.Services.Interfaces
{
    public interface IMissingDataService
    {
        IEnumerable<CurrencyRate> FillDataForQueries(CurrencyRate[] currencyRates, CurrencyRateQuery[] queries);
    }
}