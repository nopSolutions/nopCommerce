using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Widgets.AbcBonusBundle.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Plugin.Widgets.AbcBonusBundle.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => int.MaxValue;

        public void Register(
               IServiceCollection services,
               ITypeFinder typeFinder,
               AppSettings appSettings
        ) {
            services.AddScoped<IProductAbcBundleService, ProductAbcBundleService>();
        }
    }
}
