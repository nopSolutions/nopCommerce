using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests
{
    [TestFixture]
    public abstract class ServiceTest
    {
        [SetUp]
        public void SetUp()
        {
            try
            {
                PluginManager.Initialize();
            }
            catch {}
        }
    }
}
