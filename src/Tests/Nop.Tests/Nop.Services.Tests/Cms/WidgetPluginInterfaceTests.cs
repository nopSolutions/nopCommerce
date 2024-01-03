using Moq;
using NUnit.Framework;
using Nop.Services.Cms;

namespace Nop.Tests.Services.Cms
{
    [TestFixture]
    public class WidgetPluginInterfaceTests
    {
        [Test]
        public void HideInWidgetList_ShouldReturnTrue_WhenHideInWidgetListIsTrue()
        {
            // Arrange
            var widgetPlugin = CreateWidgetPlugin();
            SetupHideInWidgetList(widgetPlugin, hideInWidgetList: true);

            // Act
            var result = widgetPlugin.HideInWidgetList;

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void HideInWidgetList_ShouldReturnFalse_WhenHideInWidgetListIsFalse()
        {
            // Arrange
            var widgetPlugin = CreateWidgetPlugin();
            SetupHideInWidgetList(widgetPlugin, hideInWidgetList: false);

            // Act
            var result = widgetPlugin.HideInWidgetList;

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetWidgetZonesAsync_ShouldReturnWidgetZones_WhenValidInput()
        {
            // Arrange
            var widgetPlugin = CreateWidgetPlugin();

            var mockWidgetZones = new List<string> { "Zone1", "Zone2" };
            SetupGetWidgetZonesAsync(widgetPlugin, mockWidgetZones);

            // Act
            var result = await widgetPlugin.GetWidgetZonesAsync();

            // Assert
            CollectionAssert.AreEquivalent(mockWidgetZones, result);
        }

        [Test]
        public void GetWidgetViewComponent_ShouldReturnViewComponentType_WhenValidWidgetZone()
        {
            // Arrange
            var widgetPlugin = CreateWidgetPlugin();
            const string widgetZone = "Zone1";

            SetupGetWidgetViewComponent(widgetPlugin, widgetZone, typeof(MockViewComponent));

            // Act
            var result = widgetPlugin.GetWidgetViewComponent(widgetZone);

            // Assert
            Assert.AreEqual(typeof(MockViewComponent), result);
        }

        [Test]
        public void GetWidgetViewComponent_ShouldReturnNull_WhenInvalidWidgetZone()
        {
            // Arrange
            var widgetPlugin = CreateWidgetPlugin();
            const string invalidWidgetZone = "InvalidZone";

            SetupGetWidgetViewComponent(widgetPlugin, invalidWidgetZone, null);

            // Act
            var result = widgetPlugin.GetWidgetViewComponent(invalidWidgetZone);

            // Assert
            Assert.IsNull(result);
        }

        private static IWidgetPlugin CreateWidgetPlugin()
        {
            return new Mock<IWidgetPlugin>().Object;
        }

        private static void SetupHideInWidgetList(IWidgetPlugin widgetPlugin, bool hideInWidgetList)
        {
            var mockWidgetPlugin = Mock.Get(widgetPlugin);
            mockWidgetPlugin.SetupGet(plugin => plugin.HideInWidgetList)
                            .Returns(hideInWidgetList);
        }

        private static void SetupGetWidgetZonesAsync(IWidgetPlugin widgetPlugin, List<string> widgetZones)
        {
            var mockWidgetPlugin = Mock.Get(widgetPlugin);
            mockWidgetPlugin.Setup(plugin => plugin.GetWidgetZonesAsync())
                            .ReturnsAsync(widgetZones);
        }

        private static void SetupGetWidgetViewComponent(IWidgetPlugin widgetPlugin, string widgetZone, Type viewComponentType)
        {
            var mockWidgetPlugin = Mock.Get(widgetPlugin);
            mockWidgetPlugin.Setup(plugin => plugin.GetWidgetViewComponent(widgetZone))
                            .Returns(viewComponentType);
        }

        // Mock ViewComponent for testing purposes
        private class MockViewComponent
        {
        }
    }
}
