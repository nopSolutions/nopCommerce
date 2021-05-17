using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Messages;
using Nop.Core.Infrastructure;
using Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using System.Threading.Tasks;
using Nop.Web.Framework.Menu;
using System;
using System.Linq;

namespace Nop.Plugin.Misc.BuyAmScraper
{
    public class BuyAmScraperPlugin : BasePlugin
    {
        #region Fields

        private const string SCRAPE_TASK_NAME = "Nop.Plugin.BuyAmScraper.Service.BuyAmScraperTask";
        private readonly Services.Tasks.IScheduleTaskService _scheduleTaskService;

        #endregion

        #region Ctor

        public BuyAmScraperPlugin(
            Services.Tasks.IScheduleTaskService scheduleTaskService)
        {
            _scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            if (await _scheduleTaskService.GetTaskByTypeAsync(SCRAPE_TASK_NAME) == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new Core.Domain.Tasks.ScheduleTask
                {
                    Enabled = false,
                    Name = "Scrape from buy.am",
                    Type = SCRAPE_TASK_NAME,
                    Seconds = 5 * 60
                });
            }

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        #endregion
    }
}
