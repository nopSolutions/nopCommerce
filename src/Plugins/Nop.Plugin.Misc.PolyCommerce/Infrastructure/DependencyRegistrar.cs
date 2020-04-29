using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
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

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<PolyCommerceProductModelFactory>().As<IProductModelFactory>().InstancePerLifetimeScope();
        }
    }
}
