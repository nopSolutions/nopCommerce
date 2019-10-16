using Moq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Plugins;

namespace Nop.Services.Tests.FakeServices
{
    public class FakePluginService : PluginService
    {
        public FakePluginService(
            CatalogSettings catalogSettings = null,
            ICustomerService customerService = null,
            ILogger logger = null,
            INopFileProvider fileProvider = null,
            IWebHelper webHelper = null) : base(
                catalogSettings ?? new CatalogSettings(),
                customerService ?? new Mock<ICustomerService>().Object,
                logger ?? new NullLogger(),
                fileProvider ?? new Mock<INopFileProvider>().Object,
                webHelper ?? new Mock<IWebHelper>().Object)
        {
        }
    }
}
