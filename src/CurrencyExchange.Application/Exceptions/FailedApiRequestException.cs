using System;
using System.Net;

namespace CurrencyExchange.Application.Exceptions
{
    internal class FailedApiRequestException : Exception
    {
        public FailedApiRequestException(string requestResource, HttpStatusCode statusCode) : base($"Request to api ({requestResource}) failed. Response status code: {statusCode}")
        {
        }
    }
}