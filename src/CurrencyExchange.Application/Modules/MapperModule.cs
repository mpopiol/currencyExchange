using Autofac;
using AutoMapper;
using CurrencyExchange.Application.Configuration;
using CurrencyExchange.Application.MapperProfiles;

namespace CurrencyExchange.Application.Modules
{
    public class MapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CurrencyRateMapperProfile(ctx.Resolve<ISdmxApiConfiguration>()));
            }));
        }
    }
}