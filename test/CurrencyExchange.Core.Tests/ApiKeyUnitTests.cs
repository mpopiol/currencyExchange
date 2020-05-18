using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace CurrencyExchange.Core.Tests
{
    public class ApiKeyUnitTests
    {
        [Fact]
        public void ApiKey_CreationTimeFromFuture_ThrowFutureDateCreationException()
        {
            var creationDate = DateTime.UtcNow.AddDays(1);

            Action act = () => new ApiKey(creationDate);

            act.Should().Throw<FutureDateCreationException>();
        }

        [Fact]
        public void ApiKey_CreationTimeFromPast_CreationTimeSetProperly()
        {
            var creationDate = DateTime.UtcNow.AddDays(-1);

            var result = new ApiKey(creationDate);

            result.CreationTime.Should().Be(creationDate);
        }
    }
}