using Autofac.Features.OwnedInstances;
using CurrencyExchange.Application.Repositories.Interfaces;
using CurrencyExchange.Core.Context;
using CurrencyExchange.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CurrencyExchange.Application.Repositories
{
    internal class ApiKeyRepository : IApiKeyRepository
    {
        private readonly Func<Owned<CurrencyExchangeContext>> dbContextFactory;

        public ApiKeyRepository(Func<Owned<CurrencyExchangeContext>> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task AddAsync(ApiKey apiKey)
        {
            using var dbContext = dbContextFactory();

            dbContext.Value.Add(apiKey);

            await dbContext.Value.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            using var dbContext = dbContextFactory();

            return await dbContext.Value.ApiKeys.AnyAsync(apiKey => apiKey.Id == id);
        }
    }
}