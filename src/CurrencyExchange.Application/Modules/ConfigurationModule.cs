using Autofac;
using CurrencyExchange.Application.Configuration;
using CurrencyExchange.Application.Enums;
using Microsoft.Extensions.Configuration;

namespace CurrencyExchange.Application.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(RegisterGeneralConfiguration)
                .As<IGeneralConfiguration>();

            builder
                .Register(RegisterSdmxConfiguration)
                .As<ISdmxApiConfiguration>();

            builder
                .Register(RegisterCacheConfiguration)
                .As<ICacheConfiguration>();
        }

        private IGeneralConfiguration RegisterGeneralConfiguration(IComponentContext componentContext)
        {
            var configuration = componentContext.Resolve<IConfiguration>();

            return configuration.GetSection("General").Get<GeneralConfiguration>();
        }

        private ICacheConfiguration RegisterCacheConfiguration(IComponentContext componentContext)
        {
            var configuration = componentContext.Resolve<IConfiguration>();

            return configuration.GetSection("Cache").Get<CacheConfiguration>();
        }

        private ISdmxApiConfiguration RegisterSdmxConfiguration(IComponentContext componentContext)
        {
            var configuration = componentContext.Resolve<IConfiguration>();

            return configuration.GetSection("SdmxApi").Get<SdmxApiConfiguration>();
        }

        private class GeneralConfiguration : IGeneralConfiguration
        {
            public ApiType ApiType { get; set; }

            public int DaysGoingBackForMissingData { get; set; }
        }

        private class CacheConfiguration : ICacheConfiguration
        {
            public bool IsEnabled { get; set; }
        }

        private class SdmxApiConfiguration : ISdmxApiConfiguration
        {
            public string UriTemplate { get; set; }

            public string SourceCurrencyKey { get; set; }

            public string TargetCurrencyKey { get; set; }
        }
    }
}