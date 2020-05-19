using CurrencyExchange.Application.Cache.Interfaces;
using CurrencyExchange.Application.Configuration;
using CurrencyExchange.Core.Entities;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace CurrencyExchange.Application.Cache
{
    internal class CurrencyRateCache : ICurrencyRateCache
    {
        private const uint entrySize = 1;

        private readonly MemoryCache memoryCache;
        private readonly ICacheConfiguration cacheConfiguration;

        public CurrencyRateCache(ICacheConfiguration cacheConfiguration)
        {
            memoryCache = new MemoryCache(new MemoryCacheOptions
            {
                ExpirationScanFrequency = cacheConfiguration.ExpirationScanFrequency,
                SizeLimit = cacheConfiguration.MaxCachedQueries
            });
            this.cacheConfiguration = cacheConfiguration;
        }

        public void AddRange(CurrencyRate[] currencyRates)
        {
            foreach (var currencyRate in currencyRates)
                memoryCache.Set(currencyRate.Query.Hash, currencyRate, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = cacheConfiguration.SlidingEntryExpiration,
                    Size = entrySize
                });
        }

        public bool TryGet(CurrencyRateQuery query, out CurrencyRate currencyRate)
            => memoryCache.TryGetValue(query.Hash, out currencyRate);
    }
}