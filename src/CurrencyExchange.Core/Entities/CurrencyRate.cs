using CurrencyExchange.Core.Exceptions;
using System;

namespace CurrencyExchange.Core.Entities
{
    public class CurrencyRate
    {
        public CurrencyRateQuery Query { get; private set; }
        public decimal ExchangeRate { get; private set; }

        private CurrencyRate()
        {
        }

        public CurrencyRate(CurrencyRateQuery query, decimal exchangeRate)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            Query = query;

            if (exchangeRate <= 0)
            {
                throw new InvalidExchangeRateException();
            }
            ExchangeRate = exchangeRate;
        }

        public override string ToString()
            => $"{Query} for {ExchangeRate}";

        public bool Equals(CurrencyRate currencyRate)
            => Query.Equals(currencyRate.Query) && ExchangeRate == currencyRate.ExchangeRate;

        public override bool Equals(object obj)
            => Equals(obj as CurrencyRate);

        public override int GetHashCode() => (Query, ExchangeRate).GetHashCode();
    }
}