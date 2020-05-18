using System;

namespace CurrencyExchange.Core.Exceptions
{
    internal class InvalidExchangeRateException : Exception
    {
        public InvalidExchangeRateException() : base("Invalid exchange rate.")
        {
        }
    }
}