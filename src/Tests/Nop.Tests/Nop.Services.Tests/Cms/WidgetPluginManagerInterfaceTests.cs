using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Nop.Core.Domain.Customers;
using Nop.Services.Plugins;
using Nop.Services.Cms;

namespace Nop.Tests.Services.Cms
{
    [TestFixture]
    public class WidgetPluginManagerInterfaceTests
    {
        [Test]
        public async Task LoadActivePluginsAsync_ShouldReturnActiveWidgets_WhenValidInput()
        {
            // Arrange
            var widgetPluginManager = CreateWidgetPluginManager();

            var mockWidget1 = new Mock<IWidgetPlugin>();
            var mockWidget2 = new Mock<IWidgetPlugin>();

            var activeWidgets = new List<IWidgetPlugin> { mockWidget1.Object, mockWidget2.Object };
            SetupLoadActivePluginsAsync(widgetPluginManager, activeWidgets);

            // Act
            var result = await widgetPluginManager.LoadActivePluginsAsync();

            // Assert
            CollectionAssert.AreEquivalent(activeWidgets, result);
        }

        [Test]
        public void IsPluginActive_ShouldReturnTrue_WhenPluginIsActive()
        {
            // Arrange
            var widgetPluginManager = CreateWidgetPluginManager();
            var mockWidget = new Mock<IWidgetPlugin>();

            // Act
            var result = widgetPluginManager.IsPluginActive(mockWidget.Object);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsPluginActiveAsync_ShouldReturnTrue_WhenPluginIsActive()
        {
            // Arrange
            var widgetPluginManager = CreateWidgetPluginManager();
            var mockWidget = new Mock<IWidgetPlugin>();
            SetupIsPluginActiveAsync(widgetPluginManager, mockWidget.Object, active: true);

            // Act
            var result = await widgetPluginManager.IsPluginActiveAsync("MockWidget");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsPluginActive_ShouldReturnFalse_WhenPluginIsNotActive()
        {
            // Arrange
            var widgetPluginManager = CreateWidgetPluginManager();
            var mockWidget = new Mock<IWidgetPlugin>();
            SetupIsPluginActive(widgetPluginManager, mockWidget.Object, active: false);

            // Act
            var result = widgetPluginManager.IsPluginActive(mockWidget.Object);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsPluginActiveAsync_ShouldReturnFalse_WhenPluginIsNotActive()
        {
            // Arrange
            var widgetPluginManager = CreateWidgetPluginManager();
            var mockWidget = new Mock<IWidgetPlugin>();
            SetupIsPluginActiveAsync(widgetPluginManager, mockWidget.Object, active: false);

            // Act
            var result = await widgetPluginManager.IsPluginActiveAsync("MockWidget");

            // Assert
            Assert.IsFalse(result);
        }

        private static IWidgetPluginManager CreateWidgetPluginManager()
        {
            return new Mock<IWidgetPluginManager>().Object;
        }

        private static void SetupLoadActivePluginsAsync(IWidgetPluginManager widgetPluginManager, List<IWidgetPlugin> activeWidgets)
        {
            var mockPluginManager = Mock.Get(widgetPluginManager);
            mockPluginManager.Setup(manager => manager.LoadActivePluginsAsync(It.IsAny<Customer>(), It.IsAny<int>(), It.IsAny<string>()))
                             .ReturnsAsync(activeWidgets);
        }

        private static void SetupIsPluginActive(IWidgetPluginManager widgetPluginManager, IWidgetPlugin widget, bool active)
        {
            var mockPluginManager = Mock.Get(widgetPluginManager);
            mockPluginManager.Setup(manager => manager.IsPluginActive(widget))
                             .Returns(active);
        }

        private static void SetupIsPluginActiveAsync(IWidgetPluginManager widgetPluginManager, IWidgetPlugin widget, bool active)
        {
            var mockPluginManager = Mock.Get(widgetPluginManager);
            mockPluginManager.Setup(manager => manager.IsPluginActiveAsync(It.IsAny<string>(), It.IsAny<Customer>(), It.IsAny<int>()))
                             .ReturnsAsync(active);
        }
    }
}
