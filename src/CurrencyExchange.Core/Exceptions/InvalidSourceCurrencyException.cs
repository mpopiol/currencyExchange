namespace CurrencyExchange.Core.Exceptions
{
    internal class InvalidSourceCurrencyException : RequestException
    {
        public InvalidSourceCurrencyException(string currency) : base($"'{currency}' is invalid currency code.")
        {
        }
    }
}