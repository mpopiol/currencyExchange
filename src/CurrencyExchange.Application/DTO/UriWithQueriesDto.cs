using CurrencyExchange.Core.Entities;

namespace CurrencyExchange.Application.DTO
{
    public class UriWithQueriesDto
    {
        public string URI { get; set; }
        public CurrencyRateQuery[] Queries { get; set; }
    }
}