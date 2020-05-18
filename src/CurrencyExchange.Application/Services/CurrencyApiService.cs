using AutoMapper;
using CurrencyExchange.Application.Cache.Interfaces;
using CurrencyExchange.Application.Configuration;
using CurrencyExchange.Application.DTO;
using CurrencyExchange.Application.Exceptions;
using CurrencyExchange.Application.Services.Interfaces;
using CurrencyExchange.Core.Entities;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CurrencyExchange.Application.Services
{
    internal class CurrencyApiService : ICurrencyRateApiService
    {
        private readonly RestClient restClient = new RestClient();

        private readonly ILogger<CurrencyApiService> logger;
        private readonly IMapper mapper;
        private readonly ICurrencyRateQueryService currencyRateQueryService;
        private readonly IApiHandlingStrategy apiHandlingStrategy;
        private readonly ICacheConfiguration cacheConfiguration;
        private readonly IGeneralConfiguration apiConfiguration;
        private readonly IMissingDataService missingDataService;
        private readonly ICurrencyRateCache currencyRateCache;

        public CurrencyApiService(
            ILogger<CurrencyApiService> logger,
            IMapper mapper,
            ICurrencyRateQueryService currencyRateQueryService,
            IApiHandlingStrategy apiHandlingStrategy,
            ICacheConfiguration cacheConfiguration,
            IGeneralConfiguration apiConfiguration,
            IMissingDataService missingDataService,
            ICurrencyRateCache currencyRateCache)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.currencyRateQueryService = currencyRateQueryService;
            this.apiHandlingStrategy = apiHandlingStrategy;
            this.cacheConfiguration = cacheConfiguration;
            this.apiConfiguration = apiConfiguration;
            this.missingDataService = missingDataService;
            this.currencyRateCache = currencyRateCache;
        }

        public async Task<CurrencyRateDto[]> GetCurrencyRatesAsync(CurrencyRateQueryDto queryDto)
        {
            if (queryDto is null)
                throw new NullQueryException();

            var queries = await currencyRateQueryService.GetAsync(queryDto);

            if (!queries.Any())
                return Array.Empty<CurrencyRateDto>();

            var currencyRates = new List<CurrencyRate>();

            if (cacheConfiguration.IsEnabled)
            {
                FilterCachedQueries(ref queries, currencyRates);
            }

            var requestsWithQueries = apiHandlingStrategy.GetPreparedRequests(queries);

            var currencyRateTasks = requestsWithQueries.Select(ExecuteRequestAsync);
            var currencyRateResults = await Task.WhenAll(currencyRateTasks);

            currencyRates.AddRange(currencyRateResults.SelectMany(currencyRates => currencyRates.Where(currencyRate => currencyRate != null)));

            if (currencyRates.Any(currencyRate => currencyRate.Query.Date <= queryDto.StartDate))
                return mapper.Map<CurrencyRateDto[]>(currencyRates);

            var originalDate = queryDto.StartDate;
            queryDto.StartDate = originalDate.AddDays(-apiConfiguration.DaysGoingBackForMissingData);

            var extendedResults = await GetCurrencyRatesAsync(queryDto);

            return extendedResults
                .Where(currencyRate => currencyRate.Date >= originalDate)
                .ToArray();
        }

        private void FilterCachedQueries(ref CurrencyRateQuery[] queries, List<CurrencyRate> currencyRates)
        {
            queries = queries.Where(query =>
                {
                    var isInCache = currencyRateCache.TryGet(query, out var currencyRate);

                    if (isInCache)
                        currencyRates.Add(currencyRate);

                    return !isInCache;
                })
                .ToArray();
        }

        private async Task<CurrencyRate[]> ExecuteRequestAsync(RequestWithQueriesDto requestWithQueries)
        {
            var response = await GetResponseAsync(requestWithQueries.Request);

            var currencyRates = apiHandlingStrategy.GetCurrencyRates(response);

            if (currencyRates.Any() && currencyRates.Length < requestWithQueries.Queries.Length)
            {
                currencyRates = missingDataService.FillDataForQueries(currencyRates, requestWithQueries.Queries).ToArray();
            }

            if (cacheConfiguration.IsEnabled)
                currencyRateCache.AddRange(currencyRates);

            return currencyRates;
        }

        private async Task<IRestResponse> GetResponseAsync(IRestRequest request)
        {
            logger.LogInformation($"Executing request to API {request.Resource}");

            var response = await restClient.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedApiRequestException(request.Resource, response.StatusCode);
            }

            return response;
        }
    }
}