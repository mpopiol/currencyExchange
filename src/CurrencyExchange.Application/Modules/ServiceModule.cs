using Autofac;
using CurrencyExchange.Application.Cache;
using CurrencyExchange.Application.Cache.Interfaces;
using CurrencyExchange.Application.Configuration;
using CurrencyExchange.Application.Repositories;
using CurrencyExchange.Application.Repositories.Interfaces;
using CurrencyExchange.Application.Services;
using CurrencyExchange.Application.Services.Interfaces;
using System;

namespace CurrencyExchange.Application.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<CurrencyApiService>()
                .As<ICurrencyRateApiService>();

            builder
                .RegisterType<ApiKeyRepository>()
                .As<IApiKeyRepository>();

            builder
                .RegisterType<ApiKeyService>()
                .As<IApiKeyService>();

            builder
                .RegisterType<CurrencyRateQueryService>()
                .As<ICurrencyRateQueryService>();

            builder
                .RegisterType<MissingDataService>()
                .As<IMissingDataService>();

            builder
                .RegisterType<CurrencyRateCache>()
                .As<ICurrencyRateCache>()
                .SingleInstance();

            builder
                .RegisterType<SdmxQueryTranslationService>()
                .AsSelf();

            builder
                .Register(RegisterRequestTranslationService)
                .As<IRequestTranslationService>();

            builder
                .RegisterType<SdmxApiHandlingStrategy>()
                .AsSelf();

            builder
                .Register(RegisterApiHandlingStrategy)
                .As<IApiHandlingStrategy>();
        }

        private IApiHandlingStrategy RegisterApiHandlingStrategy(IComponentContext componentContext)
        {
            var apiConfiguration = componentContext.Resolve<IGeneralConfiguration>();

            return apiConfiguration.ApiType switch
            {
                Enums.ApiType.Sdmx => componentContext.Resolve<SdmxApiHandlingStrategy>(),
                _ => throw new NotImplementedException(),
            };
        }

        private IRequestTranslationService RegisterRequestTranslationService(IComponentContext componentContext)
        {
            var apiConfiguration = componentContext.Resolve<IGeneralConfiguration>();

            return apiConfiguration.ApiType switch
            {
                Enums.ApiType.Sdmx => componentContext.Resolve<SdmxQueryTranslationService>(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}