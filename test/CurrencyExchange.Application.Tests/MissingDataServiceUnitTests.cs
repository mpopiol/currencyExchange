using CurrencyExchange.Application.Services;
using CurrencyExchange.Core.Entities;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace CurrencyExchange.Application.Tests
{
    public class MissingDataServiceUnitTests
    {
        [Fact]
        public void FillDataForQueries_NullCurrencyRates_ThrowArgumentNullException()
        {
            var service = new MissingDataService();
            CurrencyRate[] currencyRates = null;
            CurrencyRateQuery[] currencyRateQueries = null;

            Action act = () => service.FillDataForQueries(currencyRates, currencyRateQueries).ToArray();

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("currencyRates");
        }

        [Fact]
        public void FillDataForQueries_NullCurrencyRateQueries_ThrowArgumentNullException()
        {
            var service = new MissingDataService();
            var currencyRates = new CurrencyRate[] { };
            CurrencyRateQuery[] queries = null;

            Action act = () => service.FillDataForQueries(currencyRates, queries).ToArray();

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("queries");
        }

        [Fact]
        public void FillDataForQueries_NoFillNeeded_ReturnCurrencyRatesOrdered()
        {
            var service = new MissingDataService();

            var queries = new CurrencyRateQuery[]
            {
                new CurrencyRateQuery("xxx", "yyy", DateTime.UtcNow.AddDays(-1)),
                new CurrencyRateQuery("aaa", "bbb", DateTime.UtcNow.AddDays(-1)),
            };

            var currencyRates = queries.Select(query => new CurrencyRate(query, 1)).ToArray();

            var result = service.FillDataForQueries(currencyRates, queries).ToArray();

            result.Should().HaveCount(2);
            result.Should().SatisfyRespectively(
                first =>
                {
                    first.Should().Be(currencyRates[1]);
                },
                second =>
                {
                    second.Should().Be(currencyRates[0]);
                }
            );
        }

        [Fact]
        public void FillDataForQueries_OneDayMissingData_ReturnContinousDayData()
        {
            var service = new MissingDataService();

            var missingDataDate = DateTime.UtcNow.AddDays(-10);

            var queries = new CurrencyRateQuery[]
            {
                new CurrencyRateQuery("xxx", "yyy", missingDataDate.AddDays(-1)),
                new CurrencyRateQuery("xxx", "yyy", missingDataDate),
                new CurrencyRateQuery("xxx", "yyy", missingDataDate.AddDays(1)),
                new CurrencyRateQuery("aaa", "bbb", missingDataDate.AddDays(-1)),
                new CurrencyRateQuery("aaa", "bbb", missingDataDate),
                new CurrencyRateQuery("aaa", "bbb", missingDataDate.AddDays(1)),
            };

            var currencyRates = queries
                .Where(query => query.Date != missingDataDate)
                .Select((query, id) => new CurrencyRate(query, id + 1)).ToArray();

            var result = service.FillDataForQueries(currencyRates, queries).ToArray();

            result.Should().HaveCount(6);
            result.Where(currencyRate => currencyRate.Query.Date == missingDataDate).Should().HaveCount(2);
        }

        [Fact]
        public void FillDataForQueries_OneDayResultAndNextOneMissingData_ReturnBothResultsWithSameValues()
        {
            var service = new MissingDataService();

            var missingDataDate = DateTime.UtcNow.AddDays(-10);

            var queries = new CurrencyRateQuery[]
            {
                new CurrencyRateQuery("xxx", "yyy", missingDataDate.AddDays(-1)),
                new CurrencyRateQuery("xxx", "yyy", missingDataDate),
            };

            var currencyRates = queries
                .Where(query => query.Date != missingDataDate)
                .Select((query, id) => new CurrencyRate(query, id + 1)).ToArray();

            var result = service.FillDataForQueries(currencyRates, queries).ToArray();

            result.Should().HaveCount(2);
            result.Where(currencyRate => currencyRate.Query.Date == missingDataDate).Should().HaveCount(1);
            result.Select(currencyRate => currencyRate.ExchangeRate).Should().AllBeEquivalentTo(currencyRates.First().ExchangeRate);
        }

        [Fact]
        public void FillDataForQueries_OneDayResultAndPreviousOneMissingData_ReturnOneResult()
        {
            var service = new MissingDataService();

            var missingDataDate = DateTime.UtcNow.AddDays(-10);

            var queries = new CurrencyRateQuery[]
            {
                new CurrencyRateQuery("xxx", "yyy", missingDataDate.AddDays(1)),
                new CurrencyRateQuery("xxx", "yyy", missingDataDate),
            };

            var currencyRates = queries
                .Where(query => query.Date != missingDataDate)
                .Select((query, id) => new CurrencyRate(query, id + 1)).ToArray();

            var result = service.FillDataForQueries(currencyRates, queries).ToArray();

            result.Should().ContainSingle();
            result.Where(currencyRate => currencyRate.Query.Date == missingDataDate).Should().HaveCount(0);
        }
    }
}