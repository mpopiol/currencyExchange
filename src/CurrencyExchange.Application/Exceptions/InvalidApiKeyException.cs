using CurrencyExchange.Core.Exceptions;

namespace CurrencyExchange.Application.Exceptions
{
    internal class InvalidApiKeyException : RequestException
    {
        public InvalidApiKeyException(string apiKey) : base($"Invalid API key ({apiKey})")
        {
        }
    }
}