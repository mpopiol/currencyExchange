using CurrencyExchange.Application.Configuration;
using CurrencyExchange.Application.DTO;
using CurrencyExchange.Application.Services.Interfaces;
using CurrencyExchange.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CurrencyExchange.Application.Services
{
    internal class SdmxQueryTranslationService : IRequestTranslationService
    {
        private readonly ISdmxApiConfiguration apiConfiguration;

        public SdmxQueryTranslationService(ISdmxApiConfiguration apiConfiguration)
        {
            this.apiConfiguration = apiConfiguration;
        }

        public UriWithQueriesDto[] GetURIsWithQueries(CurrencyRateQuery[] queries)
        {
            if (queries == null)
                throw new ArgumentNullException(nameof(queries));

            if (!queries.Any())
                return Array.Empty<UriWithQueriesDto>();

            var startDate = queries.Min(query => query.Date);
            var endDate = queries.Max(query => query.Date);

            return queries.GroupBy(query => query.TargetCurrency)
                .Select(grouping =>
                {
                    var target = grouping.Key;
                    var sources = string.Join('+', grouping.Select(sourceTarget => sourceTarget.SourceCurrency).Distinct());

                    var uri = string.Format(apiConfiguration.UriTemplate, sources, target, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

                    return new UriWithQueriesDto
                    {
                        URI = uri,
                        Queries = grouping.ToArray()
                    };
                })
                .ToArray();
        }
    }
}