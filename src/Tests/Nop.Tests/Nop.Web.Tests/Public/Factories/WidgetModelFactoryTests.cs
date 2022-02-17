using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Routing;
using Nop.Core.Domain.Cms;
using Nop.Services.Configuration;
using Nop.Web.Factories;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Factories
{
    [TestFixture]
    public class WidgetModelFactoryTests : WebTest
    {
        private ISettingService _settingsService;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _settingsService = GetService<ISettingService>();

            var widgetSettings = GetService<WidgetSettings>();

            widgetSettings.ActiveWidgetSystemNames.Add("TestWidgetPlugin");

            await _settingsService.SaveSettingAsync(widgetSettings);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            var widgetSettings = GetService<WidgetSettings>();

            widgetSettings.ActiveWidgetSystemNames.Remove("TestWidgetPlugin");

            await _settingsService.SaveSettingAsync(widgetSettings);
        }

        [Test]
        public async Task CanPrepareRenderWidgetModel()
        {
            var models = await GetService<IWidgetModelFactory>().PrepareRenderWidgetModelAsync("test widget zone");

            models.Any().Should().BeTrue();

            var model = models[0];

            var args = model.WidgetViewComponentArguments as RouteValueDictionary;
            args.Should().NotBeNull();
            args.Count.Should().Be(2);
            args["widgetZone"].Should().Be("test widget zone");
            model.WidgetViewComponent.Should().Be(typeof(TestWidgetPlugin));
        }
    }
}
