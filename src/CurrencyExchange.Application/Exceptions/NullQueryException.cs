using System;

namespace CurrencyExchange.Application.Exceptions
{
    internal class NullQueryException : Exception
    {
        public NullQueryException() : base("Query is null.")
        {
        }
    }
}