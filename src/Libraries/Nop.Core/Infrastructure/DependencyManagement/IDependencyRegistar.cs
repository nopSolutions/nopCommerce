using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace Nop.Core.Infrastructure.DependencyManagement
{
    public interface IDependencyRegistar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);
    }
}
