using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Web.Factories;
using Nop.Services.Media.RoxyFileman;
using AbcWarehouse.Plugin.Payments.UniFi.Services;

namespace AbcWarehouse.Plugin.Payments.UniFi
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
            services.AddScoped<ITransactionLookupService, TransactionLookupService>();
        }
    }
}
