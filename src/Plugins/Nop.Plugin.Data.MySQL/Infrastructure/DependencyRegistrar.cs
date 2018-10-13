using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Data.MySQL.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using ProductService = Nop.Plugin.Data.MySQL.Services.Catalog.ProductService;
using ProductTagService = Nop.Plugin.Data.MySQL.Services.Catalog.ProductTagService;
using FulltextService = Nop.Plugin.Data.MySQL.Services.Common.FulltextService;
using LocalizationService = Nop.Plugin.Data.MySQL.Services.Localization.LocalizationService;
using CategoryService = Nop.Plugin.Data.MySQL.Services.Catalog.CategoryService;

namespace Nop.Plugin.Data.MySQL.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //data
            builder.RegisterType<DataProviderManager>().As<IDataProviderManager>().InstancePerDependency();

            //services
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductTagService>().As<IProductTagService>().InstancePerLifetimeScope();
            builder.RegisterType<FulltextService>().As<IFulltextService>().InstancePerLifetimeScope();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// After the default dependency registrar.
        /// </summary>
        public int Order => 1;
    }
}
