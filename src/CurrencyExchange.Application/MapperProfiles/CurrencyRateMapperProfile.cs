using AutoMapper;
using CurrencyExchange.Application.Configuration;
using CurrencyExchange.Application.DTO;
using CurrencyExchange.Core.Entities;
using System;
using System.Linq;

namespace CurrencyExchange.Application.MapperProfiles
{
    internal class CurrencyRateMapperProfile : Profile
    {
        public CurrencyRateMapperProfile(ISdmxApiConfiguration apiConfiguration)
        {
            CreateMap<CurrencyRate, CurrencyRateDto>()
                .ForMember(currencyRateDto => currencyRateDto.SourceCurrency, m => m.MapFrom(currencyRate => currencyRate.Query.SourceCurrency))
                .ForMember(currencyRateDto => currencyRateDto.TargetCurrency, m => m.MapFrom(currencyRate => currencyRate.Query.TargetCurrency))
                .ForMember(currencyRateDto => currencyRateDto.Date, m => m.MapFrom(currencyRate => currencyRate.Query.Date));

            CreateMap<SDMXGenericData, CurrencyRate[]>()
                .ConvertUsing(genericData => genericData.DataSet.Series
                    .Where(serie =>
                        serie.SeriesKey.Any(key => key.Id == apiConfiguration.SourceCurrencyKey) &&
                        serie.SeriesKey.Any(key => key.Id == apiConfiguration.TargetCurrencyKey))
                    .SelectMany(serie => serie.Obs.Select(obs => GetCurrencyRate(obs, serie, apiConfiguration))).ToArray());
        }

        private CurrencyRate GetCurrencyRate(Obs obs, SDMXSeries serie, ISdmxApiConfiguration apiConfiguration)
        {
            var sourceCurrency = serie.SeriesKey.Single(key => key.Id == apiConfiguration.SourceCurrencyKey).Value;
            var targetCurrency = serie.SeriesKey.Single(key => key.Id == apiConfiguration.TargetCurrencyKey).Value;
            var date = DateTime.Parse(obs.ObsDimension.Value);
            var exchangeRate = Convert.ToDecimal(obs.ObsValue.Value);

            var query = new CurrencyRateQuery(sourceCurrency, targetCurrency, date);

            return new CurrencyRate(query, exchangeRate);
        }
    }
}