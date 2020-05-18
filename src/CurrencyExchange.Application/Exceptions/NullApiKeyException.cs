using System;

namespace CurrencyExchange.Application.Exceptions
{
    internal class NullApiKeyException : Exception
    {
        public NullApiKeyException() : base("ApiKey is null.")
        {
        }
    }
}