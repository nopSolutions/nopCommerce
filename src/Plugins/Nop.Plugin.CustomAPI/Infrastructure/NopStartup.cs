using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using MagicVilla_VillaAPI;
using Nop.Plugin.CustomAPI.Repository.IRepository;
using Nop.Plugin.CustomAPI.Repository;

namespace Nop.Plugin.CustomAPI.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring services on application startup
    /// </summary>
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.AddAutoMapper(typeof(MappingConfig));

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.UseSwagger();
            application.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 1;
    }
}