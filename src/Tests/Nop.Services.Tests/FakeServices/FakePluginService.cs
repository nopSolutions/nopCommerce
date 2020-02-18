using Moq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Tests;

namespace Nop.Services.Tests.FakeServices
{
    public class FakePluginService : PluginService
    {
        public FakePluginService(
            CatalogSettings catalogSettings = null,
            ICustomerService customerService = null,
            IDataProvider dataProvider = null,
            ILogger logger = null,
            INopFileProvider fileProvider = null,
            IWebHelper webHelper = null) : base(
            catalogSettings ?? new CatalogSettings(),
            customerService ?? new Mock<ICustomerService>().Object,
            dataProvider ?? new Mock<IDataProvider>().Object,
            logger ?? new NullLogger(),
            fileProvider ?? CommonHelper.DefaultFileProvider,
            webHelper ?? new Mock<IWebHelper>().Object)
        {
        }
    }
}
