using CurrencyExchange.Application.Configuration;
using CurrencyExchange.Application.Services;
using CurrencyExchange.Core.Entities;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace CurrencyExchange.Application.Tests
{
    public class SdmxQueryTranslationServiceUnitTests
    {
        private readonly ISdmxApiConfiguration apiConfigurationMock = GetApiConfigurationMock();

        [Fact]
        public void GetURIsWithQueries_NullQuery_ThrowArgumentNullException()
        {
            var service = new SdmxQueryTranslationService(apiConfigurationMock);

            Action act = () => service.GetURIsWithQueries(null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("queries");
        }

        [Fact]
        public void GetURIsWithQueries_SingleDictionaryEntry_ReturnSingleUri()
        {
            var service = new SdmxQueryTranslationService(apiConfigurationMock);
            var queries = new CurrencyRateQuery[]
            {
                new CurrencyRateQuery("USD", "EUR", DateTime.Today),
            };

            var result = service.GetURIsWithQueries(queries);

            result.Length.Should().Be(1);
        }

        [Fact]
        public void GetURIsWithQueries_TwoSourcesToSameTarget_ReturnSingleURI()
        {
            var service = new SdmxQueryTranslationService(apiConfigurationMock);
            var queries = new CurrencyRateQuery[]
            {
                new CurrencyRateQuery("USD", "EUR", DateTime.Today),
                new CurrencyRateQuery("RUB", "EUR", DateTime.Today),
            };

            var result = service.GetURIsWithQueries(queries);

            result.Length.Should().Be(1);
        }

        [Fact]
        public void GetURIsWithQueries_SameStartEndDate_ReturnFormattedDateInGeneratedURI()
        {
            var service = new SdmxQueryTranslationService(apiConfigurationMock);
            var today = DateTime.Today;
            var queries = new CurrencyRateQuery[]
            {
                new CurrencyRateQuery("USD", "EUR", today),
            };

            var result = service.GetURIsWithQueries(queries);

            result.Length.Should().Be(1);

            var todaysDateFormated = today.ToString("yyyy-MM-dd");
            result.Should().Contain(uriWithQueries => uriWithQueries.URI.Contains($"startPeriod={todaysDateFormated}&endPeriod={todaysDateFormated}"));
        }

        private static ISdmxApiConfiguration GetApiConfigurationMock()
        {
            var mock = new Mock<ISdmxApiConfiguration>();

            mock.Setup(config => config.UriTemplate)
                .Returns("https://ws-entry-point/resource/EXR/D.{0}.{1}.SP00.A?startPeriod={2}&endPeriod={3}");

            return mock.Object;
        }
    }
}