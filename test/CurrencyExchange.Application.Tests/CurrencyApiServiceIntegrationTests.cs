using AutoMapper;
using CurrencyExchange.Application.Cache;
using CurrencyExchange.Application.Cache.Interfaces;
using CurrencyExchange.Application.Configuration;
using CurrencyExchange.Application.DTO;
using CurrencyExchange.Application.MapperProfiles;
using CurrencyExchange.Application.Services;
using CurrencyExchange.Application.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CurrencyExchange.Application.Tests
{
    public class CurrencyApiServiceIntegrationTests
    {
        private readonly ILogger<CurrencyApiService> logger = new NullLogger<CurrencyApiService>();
        private readonly ICurrencyRateQueryService currencyRateQueryService = GetCurrencyRateQueryServiceMock();
        private readonly ICurrencyRateCache currencyRateCache = GetCurrencyRateCache();
        private readonly IMissingDataService missingDataService = GetMissingDataService();
        private readonly IGeneralConfiguration apiConfiguration = GetApiConfigurationMock();
        private readonly IMapper mapper;
        private readonly IApiHandlingStrategy apiHandlingStrategy;

        public CurrencyApiServiceIntegrationTests()
        {
            var apiConfiguration = GetSdmxApiConfigurationMock();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CurrencyRateMapperProfile(apiConfiguration));
            });

            mapper = new Mapper(mapperConfig);

            apiHandlingStrategy = GetApiHandlingStrategy(apiConfiguration);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetCurrencyRates_1ApiCall(bool isMemoryCacheEnabled)
        {
            var service = GetService(isMemoryCacheEnabled);
            var query = GetDefaultQuery();

            await service.GetCurrencyRatesAsync(query);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetCurrencyRates_2SameApiCalls(bool isMemoryCacheEnabled)
        {
            var service = GetService(isMemoryCacheEnabled);
            var query = GetDefaultQuery();

            var res1 = await service.GetCurrencyRatesAsync(query);
            var res2 = await service.GetCurrencyRatesAsync(query);

            res2.Should().HaveSameCount(res1);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetCurrencyRates_50SequentialSameApiCalls(bool isMemoryCacheEnabled)
        {
            var service = GetService(isMemoryCacheEnabled);
            var query = GetDefaultQuery();

            foreach (var _ in Enumerable.Range(0, 50))
            {
                await service.GetCurrencyRatesAsync(query);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetCurrencyRates_ApiCallThenExtendedPreviousCall(bool isMemoryCacheEnabled)
        {
            var service = GetService(isMemoryCacheEnabled);
            var query = GetDefaultQuery(dayRange: 100);

            var res1 = await service.GetCurrencyRatesAsync(query);

            query.StartDate = query.StartDate.AddDays(-10);
            var res2 = await service.GetCurrencyRatesAsync(query);

            res2.Length.Should().BeGreaterOrEqualTo(res1.Length);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetCurrencyRates_ApiCallThenPartialPreviousCall(bool isMemoryCacheEnabled)
        {
            var service = GetService(isMemoryCacheEnabled);
            var query = GetDefaultQuery(dayRange: 100);

            var res1 = await service.GetCurrencyRatesAsync(query);

            query.StartDate = query.StartDate.AddDays(50);
            var res2 = await service.GetCurrencyRatesAsync(query);

            res2.Length.Should().BeLessOrEqualTo(res1.Length);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetCurrencyRates_QueryForHoliday_ReturnsValueFromBeforeHoliday(bool isMemoryCacheEnabled)
        {
            var service = GetService(isMemoryCacheEnabled);
            var query = GetNewYearsQuery();

            var result = await service.GetCurrencyRatesAsync(query);

            result.Should().NotBeEmpty();
        }

        private CurrencyApiService GetService(bool isCacheEnabled = false)
            => new CurrencyApiService(logger, mapper, currencyRateQueryService, apiHandlingStrategy, GetCacheConfigMock(isCacheEnabled), apiConfiguration, missingDataService, currencyRateCache);

        private CurrencyRateQueryDto GetDefaultQuery(int dayRange = 7)
            => new CurrencyRateQueryDto
            {
                ApiKey = Guid.NewGuid().ToString(),
                StartDate = DateTime.UtcNow.AddDays(-(dayRange + 1)),
                EndDate = DateTime.UtcNow.AddDays(-1),
                CurrencyCodes = new Dictionary<string, string>
                {
                    { "USD", "EUR" },
                    { "RUB", "EUR" },
                    { "JPY", "EUR" },
                }
            };

        private CurrencyRateQueryDto GetNewYearsQuery()
        {
            var newYears = new DateTime(DateTime.UtcNow.Year, 1, 1);

            return new CurrencyRateQueryDto
            {
                ApiKey = Guid.NewGuid().ToString(),
                StartDate = newYears,
                EndDate = newYears,
                CurrencyCodes = new Dictionary<string, string>
                {
                    { "USD", "EUR" },
                    { "RUB", "EUR" },
                    { "JPY", "EUR" },
                    { "NOR", "EUR" },
                }
            };
        }

        private static ICacheConfiguration GetCacheConfigMock(bool isEnabled)
        {
            var mockConfig = new Mock<ICacheConfiguration>();

            mockConfig.Setup(config => config.IsEnabled).Returns(isEnabled);

            return mockConfig.Object;
        }

        private static ICurrencyRateCache GetCurrencyRateCache()
        {
            return new CurrencyRateCache();
        }

        private static IMissingDataService GetMissingDataService()
        {
            return new MissingDataService();
        }

        private static ICurrencyRateQueryService GetCurrencyRateQueryServiceMock()
        {
            var mockService = new Mock<IApiKeyService>();

            mockService.Setup(service => service.IsApiKeyValidAsync(It.IsAny<string>())).ReturnsAsync(true);

            return new CurrencyRateQueryService(mockService.Object);
        }

        private IApiHandlingStrategy GetApiHandlingStrategy(ISdmxApiConfiguration apiConfiguration)
        {
            var translationService = new SdmxQueryTranslationService(apiConfiguration);

            return new SdmxApiHandlingStrategy(translationService, mapper);
        }

        private static ISdmxApiConfiguration GetSdmxApiConfigurationMock()
        {
            var mock = new Mock<ISdmxApiConfiguration>();

            mock.Setup(config => config.UriTemplate)
                .Returns("https://sdw-wsrest.ecb.europa.eu/service/data/EXR/D.{0}.{1}.SP00.A?startPeriod={2:yyyy-MM-dd}&endPeriod={3:yyyy-MM-dd}");

            mock.Setup(config => config.SourceCurrencyKey).Returns("CURRENCY");
            mock.Setup(config => config.TargetCurrencyKey).Returns("CURRENCY_DENOM");

            return mock.Object;
        }

        private static IGeneralConfiguration GetApiConfigurationMock()
        {
            var mock = new Mock<IGeneralConfiguration>();

            mock.Setup(config => config.DaysGoingBackForMissingData).Returns(5);

            return mock.Object;
        }
    }
}