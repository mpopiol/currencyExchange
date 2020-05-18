using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace CurrencyExchange.Core.Tests
{
    public class CurrencyRateUnitTests
    {
        [Fact]
        public void CurrencyRate_NullQuery_ThrowArgumentNullException()
        {
            CurrencyRateQuery query = null;
            var exchangeRate = 0;

            Action act = () => new CurrencyRate(query, exchangeRate);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CurrencyRate_NegativeExchangeRate_ThrowInvalidExchangeRateException()
        {
            CurrencyRateQuery query = new CurrencyRateQuery("xxx", "yyy", DateTime.UtcNow);
            var exchangeRate = 0;

            Action act = () => new CurrencyRate(query, exchangeRate);

            act.Should().Throw<InvalidExchangeRateException>();
        }

        [Fact]
        public void CurrencyRate_PositiveExchangeRate_InstanceCreatedProperly()
        {
            CurrencyRateQuery query = new CurrencyRateQuery("xxx", "yyy", DateTime.UtcNow);
            var exchangeRate = 1;

            var result = new CurrencyRate(query, exchangeRate);

            result.ExchangeRate.Should().Be(exchangeRate);
            result.Query.Should().BeSameAs(query);
        }
    }
}