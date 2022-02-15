using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Plugins;
using Nop.Tests.Nop.Web.Tests.Public;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests
{
    [TestFixture]
    public abstract class WebTest : BaseNopTest
    {
        protected WebTest()
        {
            //init plugins
            InitPlugins();
        }

        private void InitPlugins()
        {
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
            webHostEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            CommonHelper.DefaultFileProvider = new NopFileProvider(webHostEnvironment.Object);

            Singleton<IPluginsInfo>.Instance = new PluginsInfo(CommonHelper.DefaultFileProvider)
            {
                PluginDescriptors = new List<(PluginDescriptor, bool)>
                {
                    (new PluginDescriptor
                    {
                        PluginType = typeof(TestWidgetPlugin),
                        SystemName = "TestWidgetPlugin",
                        FriendlyName = "Test widget plugin",
                        Installed = true,
                        ReferencedAssembly = typeof(TestWidgetPlugin).Assembly
                    }, true)
                }
            };
        }
    }
}
