using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.AbcCore;
using Microsoft.Extensions.DependencyInjection;

namespace Nop.Plugin.Misc.AbcSliExport.Infrastructure
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
            services.AddScoped<ISliExportTask, SliExportTask>();
        }
    }
}
