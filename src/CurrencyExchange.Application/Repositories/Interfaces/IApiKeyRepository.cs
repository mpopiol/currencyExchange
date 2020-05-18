using CurrencyExchange.Core.Entities;
using System;
using System.Threading.Tasks;

namespace CurrencyExchange.Application.Repositories.Interfaces
{
    public interface IApiKeyRepository
    {
        Task AddAsync(ApiKey apiKey);

        Task<bool> ExistsAsync(Guid id);
    }
}