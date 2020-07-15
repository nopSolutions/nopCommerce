using Moq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Data.Migrations;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Plugins;

namespace Nop.Tests
{
    public class FakePluginService : PluginService
    {
        public FakePluginService(
            CatalogSettings catalogSettings = null,
            ICustomerService customerService = null,
            IMigrationManager migrationManager = null,
            ILogger logger = null,
            INopFileProvider fileProvider = null,
            IWebHelper webHelper = null) : base(
            catalogSettings ?? new CatalogSettings(),
            customerService ?? new Mock<ICustomerService>().Object,
            migrationManager ?? new Mock<IMigrationManager>().Object,
            logger ?? new NullLogger(),
            fileProvider ?? CommonHelper.DefaultFileProvider,
            webHelper ?? new Mock<IWebHelper>().Object)
        {
        }
    }
}
