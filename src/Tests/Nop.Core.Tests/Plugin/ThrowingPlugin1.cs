using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;

namespace Nop.Core.Tests.Plugin
{
    [AutoInitialize]
    public class ThrowingPlugin1 : IPluginInitializer
    {
        public static bool WasInitialized { get; set; }
        public static bool Throw { get; set; }
        public void Initialize(IEngine engine)
        {
            WasInitialized = true;
            if (Throw)
                throw new SomeException("ThrowingPlugin1 isn't happy.");
        }
    }
}
