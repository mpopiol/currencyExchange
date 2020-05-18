using CurrencyExchange.Application.Services.Interfaces;
using CurrencyExchange.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyExchange.Application.Services
{
    internal class MissingDataService : IMissingDataService
    {
        public IEnumerable<CurrencyRate> FillDataForQueries(CurrencyRate[] currencyRates, CurrencyRateQuery[] queries)
        {
            if (currencyRates is null)
                throw new ArgumentNullException(nameof(currencyRates));

            if (queries is null)
                throw new ArgumentNullException(nameof(queries));

            var currencyRateGroups = currencyRates
                .GroupBy(currencyRate => (currencyRate.Query.SourceCurrency, currencyRate.Query.TargetCurrency))
                .Select(grouping => grouping.Key)
                .ToArray();

            var currencyRateOrdered = currencyRates
                .OrderBy(currencyRate => currencyRate.Query.SourceCurrency)
                .ThenBy(currencyRate => currencyRate.Query.TargetCurrency)
                .ThenBy(currencyRate => currencyRate.Query.Date)
                .ToArray();

            var currencyRateIndex = 0;

            foreach (var query in queries
                .OrderBy(query => query.SourceCurrency)
                .ThenBy(query => query.TargetCurrency)
                .ThenBy(query => query.Date))
            {
                if (!currencyRateGroups.Contains((query.SourceCurrency, query.TargetCurrency)))
                    continue;

                if (currencyRateIndex < (currencyRateOrdered.Length - 1) &&
                    (query.Date == currencyRateOrdered[currencyRateIndex + 1].Query.Date ||
                    query.TargetCurrency != currencyRateOrdered[currencyRateIndex].Query.TargetCurrency ||
                    query.SourceCurrency != currencyRateOrdered[currencyRateIndex].Query.SourceCurrency))
                {
                    currencyRateIndex++;
                }

                if (query.Date < currencyRateOrdered[currencyRateIndex].Query.Date)
                    continue;

                yield return new CurrencyRate(query, currencyRateOrdered[currencyRateIndex].ExchangeRate);
            }
        }
    }
}