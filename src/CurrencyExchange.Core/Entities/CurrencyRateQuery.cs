using CurrencyExchange.Core.Exceptions;
using CurrencyExchange.Core.Helpers;
using System;

namespace CurrencyExchange.Core.Entities
{
    public class CurrencyRateQuery
    {
        public string Hash { get; private set; }
        public string SourceCurrency { get; private set; }
        public string TargetCurrency { get; private set; }
        public DateTime Date { get; private set; }

        private CurrencyRateQuery()
        {
        }

        public CurrencyRateQuery(string sourceCurrency, string targetCurrency, DateTime date)
        {
            if (sourceCurrency.Length != 3)
            {
                throw new InvalidSourceCurrencyException(sourceCurrency);
            }
            SourceCurrency = sourceCurrency;

            if (targetCurrency.Length != 3)
            {
                throw new InvalidTargetCurrencyException(targetCurrency);
            }
            TargetCurrency = targetCurrency;

            if (date.Date > DateTime.UtcNow.Date)
            {
                throw new FutureDateQueryException();
            }
            Date = date;

            Hash = CurrencyRateHelper.GetHash(this);
        }

        public override string ToString()
            => $"{Date:yyyy-MM-dd}: {SourceCurrency} -> {TargetCurrency}";

        public bool Equals(CurrencyRateQuery currencyRateQuery)
            => Hash == currencyRateQuery.Hash;

        public override bool Equals(object obj)
            => Equals(obj as CurrencyRateQuery);

        public override int GetHashCode() => Hash.GetHashCode();
    }
}