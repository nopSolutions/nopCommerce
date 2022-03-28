using Nop.Services.Common;
using Nop.Services.Tasks;
using Nop.Core.Domain.Tasks;
using Nop.Services.Configuration;
using System.Text.RegularExpressions;
using Nop.Services.Plugins;
using Nop.Core;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcSliExport
{
    public class SliExport : BasePlugin, IMiscPlugin
    {
        public static class LocaleKey
        {
            public const string XMLFilename = "Plugins.Misc.AbcSliExport.Fields.XMLFilename";
            public const string ExportABCWarehouse = "Plugins.Misc.AbcSliExport.Fields.ExportAbcWarehouse";
            public const string ExportHawthorne = "Plugins.Misc.AbcSliExport.Fields.ExportHawthorne";
            public const string FTPUsername = "Plugins.Misc.AbcSliExport.FTPUsername";
            public const string FTPPassword = "Plugins.Misc.AbcSliExport.Fields.FTPPassword";
            public const string FTPServer = "Plugins.Misc.AbcSliExport.Fields.FTPServer";
            public const string FTPPath = "Plugins.Misc.AbcSliExport.Fields.FTPPath";
        }

        private readonly ISettingService _settingService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IWebHelper _webHelper;

        public SliExport(
            ISettingService settingService,
            IScheduleTaskService scheduleTaskService,
            IWebHelper webHelper
        )
        {
            _settingService = settingService;
            _scheduleTaskService = scheduleTaskService;
            _webHelper = webHelper;
        }

        /// <summary>
        ///		Install the plugin.
        ///		Add all necessary tasks.
        ///		Add all the default settings.
        /// </summary>
        public override async System.Threading.Tasks.Task InstallAsync()
        {
            await AddTaskAsync("SliExport", 86400);

            await _settingService.SaveSettingAsync(SliExportSettings.Default());

            await base.InstallAsync();
        }

        /// <summary>
        ///		Uninstall the plugin.
        ///		Remove all tasks owned by this plugin.
        ///		Remove all settings distinct to this plugin.
        /// </summary>
        public override async System.Threading.Tasks.Task UninstallAsync()
        {
            await RemoveTaskAsync("SliExport");

            await _settingService.DeleteSettingAsync<SliExportSettings>();

            await base.UninstallAsync();
        }

        #region Task Service Helper Methods

        /// <summary>
        ///		Add the specified task using the appropriate service.
        ///		The class must be "[name]Task" and exist in this namespace.
        /// </summary>
        private async System.Threading.Tasks.Task AddTaskAsync(string name, int seconds)
        {
            ScheduleTask task = new ScheduleTask();
            task.Name = "AbcWarehouse: " + name;
            task.Seconds = seconds;
            task.Type = GetTaskType(name);
            task.Enabled = false;
            task.StopOnError = false;

            await _scheduleTaskService.InsertTaskAsync(task);
            return;
        }

        /// <summary>
        ///		Delete the specified task from the database.
        /// </summary>
        private async System.Threading.Tasks.Task RemoveTaskAsync(string name)
        {
            ScheduleTask task = await _scheduleTaskService.GetTaskByTypeAsync(GetTaskType(name));

            if (task != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(task);
            }
            return;
        }

        private static string GetTaskType(string name)
        {
            return typeof(SliExport).Namespace + "." + name + "Task" +
                ", " + typeof(SliExport).Namespace;
        }

        #endregion

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/SliExport/Configure";
        }
    }
}