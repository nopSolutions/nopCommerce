using Nop.Core.Infrastructure.DependencyManagement;
using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.FreshAddressNewsletterIntegration.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Plugin.Misc.FreshAddressNewsletterIntegration.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return int.MaxValue;
            }
        }

        public void Register(
               IServiceCollection services,
               ITypeFinder typeFinder,
               AppSettings appSettings
        ) {
            services.AddScoped<IFreshAddressService, FreshAddressService>();
        }
    }
}
