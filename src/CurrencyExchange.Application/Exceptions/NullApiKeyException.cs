using CurrencyExchange.Core.Exceptions;
using System;

namespace CurrencyExchange.Application.Exceptions
{
    internal class NullApiKeyException : RequestException
    {
        public NullApiKeyException() : base("ApiKey is null.")
        {
        }
    }
}