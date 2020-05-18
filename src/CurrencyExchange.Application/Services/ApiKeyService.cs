using CurrencyExchange.Application.Exceptions;
using CurrencyExchange.Application.Repositories.Interfaces;
using CurrencyExchange.Application.Services.Interfaces;
using CurrencyExchange.Core.Entities;
using System;
using System.Threading.Tasks;

namespace CurrencyExchange.Application.Services
{
    internal class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyRepository apiKeyRepository;

        public ApiKeyService(IApiKeyRepository apiKeyRepository)
        {
            this.apiKeyRepository = apiKeyRepository;
        }

        public async Task<string> GetNewApiKeyAsync()
        {
            var apiKey = new ApiKey(DateTime.UtcNow);

            await apiKeyRepository.AddAsync(apiKey);

            return apiKey.Id.ToString();
        }

        public Task<bool> IsApiKeyValidAsync(string id)
        {
            if (!Guid.TryParse(id, out var apiKeyGuid))
                throw new InvalidApiKeyException(id);

            return apiKeyRepository.ExistsAsync(apiKeyGuid);
        }
    }
}