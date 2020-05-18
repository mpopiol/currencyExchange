using Autofac;
using AutoMapper;
using CurrencyExchange.Core.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CurrencyExchange.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddControllersAsServices();

            services.AddResponseCaching();

            var connection = Configuration.GetConnectionString("SqlServer");
            services.AddDbContext<CurrencyExchangeContext>(options => options.UseSqlServer(connection));
            services.AddDbContextPool<CurrencyExchangeContext>(options => options.UseSqlServer(connection));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var assembliesTypes = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                .Where(a => a.Name.StartsWith("CurrencyExchange.", StringComparison.OrdinalIgnoreCase))
                .SelectMany(an => Assembly.Load(an).GetTypes())
                .ToArray();

            foreach (var module in GetDerivedTypes<Autofac.Module>(assembliesTypes))
            {
                builder.RegisterModule(module);
            }

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>().InstancePerLifetimeScope();
        }

        private static IEnumerable<TBase> GetDerivedTypes<TBase>(Type[] assembliesTypes)
        {
            return assembliesTypes
                .Where(p => typeof(TBase).IsAssignableFrom(p) && p.IsPublic && !p.IsAbstract)
                .Distinct()
                .Select(p => (TBase)Activator.CreateInstance(p));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddSerilog();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("log.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}