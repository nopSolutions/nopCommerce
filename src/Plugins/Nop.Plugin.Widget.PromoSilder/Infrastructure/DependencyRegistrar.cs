using Nop.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Widgets.PromoSilder.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(global::Autofac.ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            throw new NotImplementedException();
        }
    }
}
