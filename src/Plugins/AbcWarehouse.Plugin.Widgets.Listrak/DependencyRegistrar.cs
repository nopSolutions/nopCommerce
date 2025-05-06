using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using AbcWarehouse.Plugin.Widgets.Listrak;

namespace AbcWarehouse.Plugin.Widgets.Listrak
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings settings)
        {
            services.AddScoped<IListrakService, ListrakService>();
        }

        public int Order => 1;
    }
}
