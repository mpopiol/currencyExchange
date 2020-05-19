using System;

namespace CurrencyExchange.Application.Configuration
{
    public interface ICacheConfiguration
    {
        public bool IsEnabled { get; }
        TimeSpan SlidingEntryExpiration { get; }
        TimeSpan ExpirationScanFrequency { get; }
        long? MaxCachedQueries { get; set; }
    }
}