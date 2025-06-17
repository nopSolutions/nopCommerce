using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure.DependencyManagement;
using AbcWarehouse.Plugin.Misc.SearchSpring.Services;

namespace AbcWarehouse.Plugin.Misc.SearchSpring.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<ISearchSpringService, SearchSpringService>();
        }

        public int Order => 10;
    }
}
