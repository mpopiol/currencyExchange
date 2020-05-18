using CurrencyExchange.Application.Cache.Interfaces;
using CurrencyExchange.Core.Entities;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace CurrencyExchange.Application.Cache
{
    internal class CurrencyRateCache : ICurrencyRateCache
    {
        private readonly MemoryCache memoryCache;

        public CurrencyRateCache()
        {
            memoryCache = new MemoryCache(new MemoryCacheOptions
            {
                ExpirationScanFrequency = TimeSpan.FromDays(1)
            });
        }

        public void AddRange(CurrencyRate[] currencyRates)
        {
            foreach (var currencyRate in currencyRates)
                memoryCache.Set(currencyRate.Query.Hash, currencyRate);
        }

        public bool TryGet(CurrencyRateQuery query, out CurrencyRate currencyRate)
            => memoryCache.TryGetValue(query.Hash, out currencyRate);
    }
}