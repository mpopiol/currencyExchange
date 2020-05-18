using System;

namespace CurrencyExchange.Core.Exceptions
{
    public class RequestException : Exception
    {
        public RequestException(string message) : base(message)
        {
        }
    }
}