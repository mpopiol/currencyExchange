using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace CurrencyExchange.Core.Tests
{
    public class CurrencyRateQueryUnitTests
    {
        [Fact]
        public void CurrencyRateQuery_SourceCurrencyNot3Chars_ThrowInvalidSourceCurrencyException()
        {
            var sourceCurrency = "";
            var targetCurrency = "xxx";
            var date = new DateTime();

            Action act = () => new CurrencyRateQuery(sourceCurrency, targetCurrency, date);

            act.Should().Throw<InvalidSourceCurrencyException>();
        }

        [Fact]
        public void CurrencyRateQuery_TargetCurrencyNot3Chars_ThrowInvalidTargetCurrencyException()
        {
            var sourceCurrency = "xxx";
            var targetCurrency = "";
            var date = new DateTime();

            Action act = () => new CurrencyRateQuery(sourceCurrency, targetCurrency, date);

            act.Should().Throw<InvalidTargetCurrencyException>();
        }

        [Fact]
        public void CurrencyRateQuery_FutureDate_ThrowFutureDateQueryException()
        {
            var sourceCurrency = "xxx";
            var targetCurrency = "yyy";
            var date = DateTime.UtcNow.AddDays(1);

            Action act = () => new CurrencyRateQuery(sourceCurrency, targetCurrency, date);

            act.Should().Throw<FutureDateQueryException>();
        }

        [Fact]
        public void CurrencyRateQuery_PastDate_InstanceCreatedProperly()
        {
            var sourceCurrency = "xxx";
            var targetCurrency = "yyy";
            var date = DateTime.UtcNow.AddDays(-1);

            var result = new CurrencyRateQuery(sourceCurrency, targetCurrency, date);

            result.SourceCurrency.Should().Be(sourceCurrency);
            result.TargetCurrency.Should().Be(targetCurrency);
            result.Date.Should().Be(date);
        }
    }
}