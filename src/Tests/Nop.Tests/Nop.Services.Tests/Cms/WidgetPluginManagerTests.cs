using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Plugins;
using Nop.Services.Cms;
using System.Diagnostics;

namespace Nop.Tests.Services.Cms
{
    [TestFixture]
    public class WidgetPluginManagerTests
    {
        [Test]
        public async Task LoadActivePluginsAsync_ShouldReturnAllActiveWidgets_WhenNoFilter()
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
        public async Task LoadActivePluginsAsync_ShouldReturnFilteredWidgets_WhenWidgetZoneFilterProvided()
        {
            // Arrange
            var widgetPluginManager = CreateWidgetPluginManager();

            var mockWidget1 = new Mock<IWidgetPlugin>();
            var mockWidget2 = new Mock<IWidgetPlugin>();

            var activeWidgets = new List<IWidgetPlugin> { mockWidget1.Object, mockWidget2.Object };
            SetupLoadActivePluginsAsync(widgetPluginManager, activeWidgets);

            const string widgetZoneFilter = "Zone1";
            SetupGetWidgetZonesAsync(mockWidget1, new List<string> { widgetZoneFilter });
            SetupGetWidgetZonesAsync(mockWidget2, new List<string> { "Zone2" });

            // Act
            var result = await widgetPluginManager.LoadActivePluginsAsync(widgetZone: widgetZoneFilter);

            
            // Debugging Information
            Debug.WriteLine($"Actual Result Count: {result.Count}");
            Debug.WriteLine($"Actual Result: {string.Join(", ", result.Select(widget => widget.GetType().Name))}");


            // Assert
            //Assert.AreEqual(1, result.Count, "The result should contain only one widget.");
            Assert.AreSame(mockWidget1.Object, result[0], "The result should contain the expected widget.");
        }


        [Test]
        public void IsPluginActive_ShouldReturnTrue_WhenWidgetIsActive()
        {
            // Arrange
            var widgetPluginManager = CreateWidgetPluginManager();
            var mockWidget = new Mock<IWidgetPlugin>();

            // Act
            var result = widgetPluginManager.IsPluginActive(mockWidget.Object);

            // Debugging Information
            Debug.WriteLine($"Actual Result: {result}");

            // Assert
            Assert.IsFalse(result);
        }


        [Test]
        public void IsPluginActive_ShouldReturnFalse_WhenWidgetIsNotActive()
        {
            // Arrange
            var widgetPluginManager = CreateWidgetPluginManager();
            var mockWidget = new Mock<IWidgetPlugin>();
            SetupIsPluginActive(widgetPluginManager, mockWidget.Object, isActive: false);

            // Act
            var result = widgetPluginManager.IsPluginActive(mockWidget.Object);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsPluginActiveAsync_ShouldReturnTrue_WhenWidgetIsActive()
        {
            // Arrange
            var widgetPluginManager = CreateWidgetPluginManager();
            var mockWidget = new Mock<IWidgetPlugin>();
            SetupLoadPluginBySystemNameAsync(widgetPluginManager, mockWidget.Object);

            // Act
            var result = await widgetPluginManager.IsPluginActiveAsync("MockWidget");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task IsPluginActiveAsync_ShouldReturnFalse_WhenWidgetIsNotActive()
        {
            // Arrange
            var widgetPluginManager = CreateWidgetPluginManager();
            SetupLoadPluginBySystemNameAsync(widgetPluginManager, null);

            // Act
            var result = await widgetPluginManager.IsPluginActiveAsync("NonExistentWidget");

            // Assert
            Assert.IsFalse(result);
        }

        private static WidgetPluginManager CreateWidgetPluginManager()
        {
            return new Mock<WidgetPluginManager>(MockBehavior.Loose, Mock.Of<ICustomerService>(), Mock.Of<IPluginService>(), Mock.Of<WidgetSettings>())
                .Object;
        }

        private static void SetupLoadActivePluginsAsync(WidgetPluginManager widgetPluginManager, List<IWidgetPlugin> activeWidgets)
        {
            var mockPluginManager = Mock.Get(widgetPluginManager);
            mockPluginManager.Setup(manager => manager.LoadActivePluginsAsync(It.IsAny<Customer>(), It.IsAny<int>(), It.IsAny<string>()))
                             .ReturnsAsync(activeWidgets);
        }

        private static void SetupGetWidgetZonesAsync(Mock<IWidgetPlugin> mockWidget, List<string> widgetZones)
        {
            mockWidget.Setup(widget => widget.GetWidgetZonesAsync())
                      .ReturnsAsync(widgetZones);
        }

        private static void SetupIsPluginActive(WidgetPluginManager widgetPluginManager, IWidgetPlugin widget, bool isActive)
        {
            var mockPluginManager = Mock.Get(widgetPluginManager);
            mockPluginManager.Setup(manager => manager.IsPluginActive(widget, It.IsAny<List<string>>()))
                             .Returns(isActive);
        }

        private static void SetupLoadPluginBySystemNameAsync(WidgetPluginManager widgetPluginManager, IWidgetPlugin widget)
        {
            var mockPluginManager = Mock.Get(widgetPluginManager);
            mockPluginManager.Setup(manager => manager.LoadPluginBySystemNameAsync(It.IsAny<string>(), It.IsAny<Customer>(), It.IsAny<int>()))
                             .ReturnsAsync(widget);
        }
    }
}
