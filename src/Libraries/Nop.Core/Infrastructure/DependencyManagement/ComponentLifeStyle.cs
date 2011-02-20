using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Infrastructure.DependencyManagement
{
    public enum ComponentLifeStyle
    {
        Singleton = 0,
        Transient = 1,
        LifetimeScope
    }
}
