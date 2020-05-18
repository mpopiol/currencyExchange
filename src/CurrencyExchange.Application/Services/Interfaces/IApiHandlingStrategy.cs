using CurrencyExchange.Application.DTO;
using CurrencyExchange.Core.Entities;
using RestSharp;

namespace CurrencyExchange.Application.Services.Interfaces
{
    public interface IApiHandlingStrategy
    {
        RequestWithQueriesDto[] GetPreparedRequests(CurrencyRateQuery[] queries);

        CurrencyRate[] GetCurrencyRates(IRestResponse response);
    }
}