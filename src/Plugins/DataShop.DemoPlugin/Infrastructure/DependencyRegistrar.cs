using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Web.Framework.Mvc;
using DataShop.DemoPlugin.Data;
using DataShop.DemoPlugin.Domain;
using DataShop.DemoPlugin.Services;

namespace DataShop.DemoPlugin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string ContextName = "nop_object_context_datashop_demo_item";

        public int Order
        {
            get { return 1; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            // Register Services
            builder.RegisterType<DemoService>().As<IDemoService>().InstancePerLifetimeScope();

            //Register Data Context
            this.RegisterPluginDataContext<DataContext>(builder, ContextName);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<DemoItem>>()
                .As<IRepository<DemoItem>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(ContextName))
                .InstancePerLifetimeScope();
        }
    }
}
