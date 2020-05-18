using CurrencyExchange.Core.Entities;
using System;
using System.Text;

namespace CurrencyExchange.Core.Helpers
{
    public static class CurrencyRateHelper
    {
        public static string GetHash(CurrencyRateQuery currencyExchange)
        {
            var plainTextBuilder = new StringBuilder();

            plainTextBuilder.Append(currencyExchange.SourceCurrency);
            plainTextBuilder.Append(currencyExchange.TargetCurrency);
            plainTextBuilder.Append(currencyExchange.Date);

            var plainTextBytes = Encoding.UTF8.GetBytes(plainTextBuilder.ToString());

            return Convert.ToBase64String(plainTextBytes);
        }
    }
}