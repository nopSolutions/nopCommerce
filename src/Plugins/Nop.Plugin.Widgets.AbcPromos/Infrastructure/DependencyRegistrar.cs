

using Autofac;
using Autofac.Features.ResolveAnything;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Microsoft.Extensions.DependencyInjection;
using Nop.Services.Caching;
using Nop.Plugin.Widgets.AbcPromos.Tasks;
using Nop.Plugin.Widgets.AbcPromos.Tasks.LegacyTasks;

namespace Nop.Plugin.Widgets.AbcPromos
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
            services.AddScoped<UpdatePromosTask, UpdatePromosTask>();
            services.AddScoped<GenerateRebatePromoPageTask, GenerateRebatePromoPageTask>();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1; }
        }
    }
}
