namespace CurrencyExchange.Core.Exceptions
{
    internal class InvalidTargetCurrencyException : RequestException
    {
        public InvalidTargetCurrencyException(string currency) : base($"'{currency}' is invalid currency code.")
        {
        }
    }
}