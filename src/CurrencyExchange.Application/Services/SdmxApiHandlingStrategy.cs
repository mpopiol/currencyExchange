using AutoMapper;
using CurrencyExchange.Application.DTO;
using CurrencyExchange.Application.Services.Interfaces;
using CurrencyExchange.Core.Entities;
using RestSharp;
using RestSharp.Deserializers;
using System.Linq;

namespace CurrencyExchange.Application.Services
{
    internal class SdmxApiHandlingStrategy : IApiHandlingStrategy
    {
        private readonly DotNetXmlDeserializer xmlDeserializer = new DotNetXmlDeserializer();
        private readonly IRequestTranslationService translationService;
        private readonly IMapper mapper;

        public SdmxApiHandlingStrategy(
            IRequestTranslationService translationService,
            IMapper mapper)
        {
            this.translationService = translationService;
            this.mapper = mapper;
        }

        public CurrencyRate[] GetCurrencyRates(IRestResponse response)
        {
            var genericData = xmlDeserializer.Deserialize<SDMXGenericData>(response);

            return mapper.Map<CurrencyRate[]>(genericData);
        }

        public RequestWithQueriesDto[] GetPreparedRequests(CurrencyRateQuery[] currencyRateQuery)
        {
            var URIsWithQueries = translationService.GetURIsWithQueries(currencyRateQuery);

            return URIsWithQueries.Select(uriWithQueries => new RequestWithQueriesDto
            {
                Request = new RestRequest(uriWithQueries.URI, Method.GET),
                Queries = uriWithQueries.Queries
            }).ToArray();
        }
    }
}