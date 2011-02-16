using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;

namespace Nop.Core.Infrastructure.AutoFac
{
    public interface IAutoFacDependencyRegistar
    {
        void Register(ContainerBuilder builder, TypeFinder typeFinder);
    }
}
