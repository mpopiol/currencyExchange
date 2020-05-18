using CurrencyExchange.Core.Entities;
using RestSharp;

namespace CurrencyExchange.Application.DTO
{
    public class RequestWithQueriesDto
    {
        public IRestRequest Request { get; set; }
        public CurrencyRateQuery[] Queries { get; set; }
    }
}