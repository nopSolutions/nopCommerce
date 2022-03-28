using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Autofac;
using Nop.Plugin.Misc.AbcCore.Mattresses;
using Nop.Web.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Plugin.Misc.AbcMattresses.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(
               IServiceCollection services,
               ITypeFinder typeFinder,
               AppSettings appSettings
        ) {
            services.AddScoped<IAbcMattressModelGiftMappingService, AbcMattressModelGiftMappingService>();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
