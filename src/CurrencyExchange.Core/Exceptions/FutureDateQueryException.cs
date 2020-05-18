namespace CurrencyExchange.Core.Exceptions
{
    internal class FutureDateQueryException : RequestException
    {
        public FutureDateQueryException() : base("Date in query is set to the future.")
        {
        }
    }
}