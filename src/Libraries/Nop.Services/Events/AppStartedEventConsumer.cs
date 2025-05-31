using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Data.Migrations;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Services.ScheduleTasks;


namespace Nop.Services.Events
{
    /// <summary>
    /// Consumer that handles AppStartedEvent and performs startup tasks.
    /// </summary>
    public class AppStartedEventConsumer : IConsumer<AppStartedEvent>
    {
        private readonly ILogger _logger;
        private readonly IPluginService _pluginService;
        private readonly IPermissionService _permissionService;
        private readonly IMigrationManager _migrationManager;
        private readonly ITaskScheduler _taskScheduler;
        private readonly IGenericAttributeService _genericAttributeService;

        public AppStartedEventConsumer(
            ILogger logger,
            IPluginService pluginService,
            IPermissionService permissionService,
            IMigrationManager migrationManager,
            ITaskScheduler taskScheduler,
            IGenericAttributeService genericAttributeService)
        {
            _logger = logger;
            _pluginService = pluginService;
            _permissionService = permissionService;
            _migrationManager = migrationManager;
            _taskScheduler = taskScheduler;
            _genericAttributeService = genericAttributeService;
        }

        public async Task HandleEventAsync(AppStartedEvent eventMessage)
        {
            await _logger.InformationAsync("Application started");

            await _pluginService.InstallPluginsAsync();
            await _pluginService.UpdatePluginsAsync();

            await _permissionService.InsertPermissionsAsync();

            var assembly1 = Assembly.GetAssembly(typeof(ApplicationBuilderExtensions));
            _migrationManager.ApplyUpMigrations(assembly1, MigrationProcessType.Update);

            var assembly2 = Assembly.GetAssembly(typeof(IMigrationManager));
            _migrationManager.ApplyUpMigrations(assembly2, MigrationProcessType.Update);

            await _taskScheduler.InitializeAsync();
            await _taskScheduler.StartSchedulerAsync();

            await _genericAttributeService.DeleteAttributesAsync<Customer>(NopCustomerDefaults.ProcessPaymentRequestAttribute);
        }
    }
}
