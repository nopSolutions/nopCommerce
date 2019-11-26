using FluentMigrator.Runner;
using Moq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
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
            IMigrationRunner migrationRunner = null,
            INopFileProvider fileProvider = null,
            IRepository<MigrationVersionInfo> migrationVersionInfoRepository= null,
            IWebHelper webHelper = null) : base(
            catalogSettings ?? new CatalogSettings(),
            customerService ?? new Mock<ICustomerService>().Object,
            logger ?? new NullLogger(),
            migrationRunner ?? new Mock<IMigrationRunner>().Object,
            fileProvider ?? new Mock<INopFileProvider>().Object,
            migrationVersionInfoRepository ?? new Mock<IRepository<MigrationVersionInfo>>().Object,
            webHelper ?? new Mock<IWebHelper>().Object)
        {
        }
    }
}
