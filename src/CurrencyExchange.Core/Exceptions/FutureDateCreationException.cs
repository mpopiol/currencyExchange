using System;

namespace CurrencyExchange.Core.Exceptions
{
    internal class FutureDateCreationException : Exception
    {
        public FutureDateCreationException() : base("Creation date is set to the future.")
        {
        }
    }
}