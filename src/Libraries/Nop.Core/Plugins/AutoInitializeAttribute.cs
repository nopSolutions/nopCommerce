using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Marks a plugin initializer as eligible for auto initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoInitializeAttribute : InitializerCreatingAttribute
    {
    }
}
