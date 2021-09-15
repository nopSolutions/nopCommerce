using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.PolyCommerce.Factories;
using Nop.Web.Areas.Admin.Factories;

namespace Nop.Plugin.Misc.PolyCommerce.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 2;

        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddTransient<IProductModelFactory, PolyCommerceProductModelFactory>();
        }
    }
}
