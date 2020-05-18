using CurrencyExchange.Application.DTO;
using CurrencyExchange.Application.Exceptions;
using CurrencyExchange.Application.Services.Interfaces;
using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchange.Application.Services
{
    internal class CurrencyRateQueryService : ICurrencyRateQueryService
    {
        private readonly IApiKeyService apiKeyService;

        public CurrencyRateQueryService(IApiKeyService apiKeyService)
        {
            this.apiKeyService = apiKeyService;
        }

        public async Task<CurrencyRateQuery[]> GetAsync(CurrencyRateQueryDto queryDto)
        {
            if (queryDto.ApiKey is null)
            {
                throw new NullApiKeyException();
            }

            if (!await apiKeyService.IsApiKeyValidAsync(queryDto.ApiKey))
            {
                throw new InvalidApiKeyException(queryDto.ApiKey);
            }

            return queryDto.CurrencyCodes
                .SelectMany(currencyPair =>
                {
                    var source = currencyPair.Key;
                    var target = currencyPair.Value;
                    var startDate = queryDto.StartDate == default ? DateTime.Today : queryDto.StartDate;
                    var endDate = queryDto.EndDate == default ? DateTime.Today : queryDto.EndDate;

                    return DateHelper.EachDay(startDate.GetRecentWorkDay(), endDate).Select(date => new CurrencyRateQuery(source, target, date));
                })
                .ToArray();
        }
    }
}