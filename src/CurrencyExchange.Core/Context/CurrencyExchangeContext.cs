using CurrencyExchange.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Core.Context
{
    public class CurrencyExchangeContext : DbContext
    {
        public DbSet<ApiKey> ApiKeys { get; set; }

        public CurrencyExchangeContext(DbContextOptions<CurrencyExchangeContext> options) : base(options)
        {
        }
    }
}