using System;

namespace CurrencyExchange.Application.Exceptions
{
    internal class EmptyCurrencyCodesException : Exception
    {
        public EmptyCurrencyCodesException() : base("Empty currency codes dictionary in the request.")
        {
        }
    }
}