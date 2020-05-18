using CurrencyExchange.Core.Exceptions;
using System;

namespace CurrencyExchange.Core.Entities
{
    public class ApiKey
    {
        public Guid Id { get; private set; }
        public DateTime CreationTime { get; private set; }

        private ApiKey()
        {
        }

        public ApiKey(DateTime creationTime)
        {
            if (creationTime > DateTime.UtcNow)
            {
                throw new FutureDateCreationException();
            }

            CreationTime = creationTime;

            Id = Guid.NewGuid();
        }
    }
}