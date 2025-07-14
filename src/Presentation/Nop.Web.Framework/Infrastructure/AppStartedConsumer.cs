﻿using System.Reflection;
using Nop.Core.Events;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Security;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Web.Framework.Infrastructure;

/// <summary>
/// Represents AppStarted event consumer
/// </summary>
public partial class AppStartedConsumer : IConsumer<AppStartedEvent>
{
    #region Fields

    protected readonly ILogger _logger;
    protected readonly IMigrationManager _migrationManager;
    protected readonly IPermissionService _permissionService;
    protected readonly IPluginService _pluginService;
    protected readonly ITaskScheduler _taskScheduler;

    #endregion

    #region Ctor

    public AppStartedConsumer(ILogger logger,
        IMigrationManager migrationManager,
        IPermissionService permissionService,
        IPluginService pluginService,
        ITaskScheduler taskScheduler)
    {
        _logger = logger;
        _migrationManager = migrationManager;
        _permissionService = permissionService;
        _pluginService = pluginService;
        _taskScheduler = taskScheduler;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(AppStartedEvent eventMessage)
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        //log application start
        await _logger.InformationAsync("Application started");

        //install and update plugins
        await _pluginService.InstallPluginsAsync();
        await _pluginService.UpdatePluginsAsync();

        //insert new ACL permission if exists
        await _permissionService.InsertPermissionsAsync();

        //update nopCommerce core and db
        var assembly = Assembly.GetAssembly(typeof(ApplicationBuilderExtensions));
        _migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);
        assembly = Assembly.GetAssembly(typeof(IMigrationManager));
        _migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);

        //run scheduler
        await _taskScheduler.InitializeAsync();
        await _taskScheduler.StartSchedulerAsync();
    }

    #endregion
}
