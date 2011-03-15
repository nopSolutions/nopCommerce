using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Plugins;
using Nop.Core.Tests.Plugin;

[assembly: TestPlugin]

namespace Nop.Core.Tests.Plugin
{

    public interface ITestPlugin1 :IPlugin
    {
    }

    public class TestPluginImplementation : BasePlugin, ITestPlugin1
    {
        public TestPluginImplementation()
            : base("TestPluginImplementation", "TestPluginImplementation") { }
    }

    public class TestPluginAttribute : BasePluginAttribute, ITestPlugin1
    {
        public TestPluginAttribute()
            :base("TestPluginAttribute", "TestPluginAttribute", 5){}
    }

    
}

