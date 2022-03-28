using System;
using System.Linq;
using EllaSoftware.Plugin.Misc.CronTasks.Models;
using EllaSoftware.Plugin.Misc.CronTasks.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Tasks;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EllaSoftware.Plugin.Misc.CronTasks.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class CronTasksController : BasePluginController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;
        private readonly ICronTaskService _cronTaskService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public CronTasksController(
            ISettingService settingService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            INotificationService notificationService,
            ICronTaskService cronTaskService,
            IScheduleTaskService scheduleTaskService,
            IDateTimeHelper dateTimeHelper)
        {
            _settingService = settingService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _notificationService = notificationService;
            _cronTaskService = cronTaskService;
            _scheduleTaskService = scheduleTaskService;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            var settings = await _settingService.LoadSettingAsync<CronTasksSettings>(
                await _storeContext.GetActiveStoreScopeConfigurationAsync()
            );

            var model = new CronTasksSettingsModel
            {
                ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
            };

            model.CronTaskSearchModel.SetGridPageSize();

            foreach (var scheduleTask in await _scheduleTaskService.GetAllTasksAsync(true))
                model.CronTaskSearchModel.AddCronTaskModel.AvailableScheduleTasks.Add(new SelectListItem()
                {
                    Text = scheduleTask.Name,
                    Value = scheduleTask.Id.ToString()
                });

            return View("~/Plugins/EllaSoftware.CronTasks/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(CronTasksSettingsModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var settings = await _settingService.LoadSettingAsync<CronTasksSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #region Cron Tasks

        [HttpPost]
        public async Task<IActionResult> CronTaskList(CronTaskSearchModel searchModel)
        {
            var cronTasks = await _cronTaskService.GetCronTasksAsync();

            var model = new CronTaskListModel();

            model.Data = await GetCronTaskModelsAsync(cronTasks);
            model.Draw = searchModel.Draw;
            model.RecordsFiltered = model.Data.Count();
            model.RecordsTotal = model.Data.Count();

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertCronTask([Validate]CronTaskModel model)
        {
            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(model.ScheduleTaskId)
                ?? throw new ArgumentException("No schedule task found with the specified id");

            await _cronTaskService.InsertCronTaskAsync(scheduleTask.Id, model.CronExpression);

            return Json(new { Result = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCronTask([Validate]CronTaskModel model)
        {
            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(model.ScheduleTaskId)
                ?? throw new ArgumentException("No schedule task found with the specified id");

            await _cronTaskService.UpdateCronTaskAsync(scheduleTask.Id, model.CronExpression);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCronTask(int id)
        {
            var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(id)
                ?? throw new ArgumentException("No schedule task found with the specified id");

            await _cronTaskService.DeleteCronTaskAsync(id);

            return new NullJsonResult();
        }

        #endregion

        #endregion

        private async Task<IEnumerable<CronTaskModel>> GetCronTaskModelsAsync(IDictionary<int, string> cronTasks)
        {
            var result = new List<CronTaskModel>();

            foreach (var ct in cronTasks)
            {
                var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(ct.Key);
                if (scheduleTask == null)
                    continue;

                var scheduleTaskModel = scheduleTask.ToModel<ScheduleTaskModel>();

                //convert dates to the user time
                if (scheduleTask.LastStartUtc.HasValue)
                {
                    scheduleTaskModel.LastStartUtc = (await _dateTimeHelper
                        .ConvertToUserTimeAsync(scheduleTask.LastStartUtc.Value, DateTimeKind.Utc)).ToString("G");
                }

                if (scheduleTask.LastEndUtc.HasValue)
                {
                    scheduleTaskModel.LastEndUtc = (await _dateTimeHelper
                        .ConvertToUserTimeAsync(scheduleTask.LastEndUtc.Value, DateTimeKind.Utc)).ToString("G");
                }
                
                if (scheduleTask.LastSuccessUtc.HasValue)
                {
                    scheduleTaskModel.LastSuccessUtc = (await _dateTimeHelper
                        .ConvertToUserTimeAsync(scheduleTask.LastSuccessUtc.Value, DateTimeKind.Utc)).ToString("G");
                }

                var cronNextOccurence = _cronTaskService.GetQuartzJobNextOccurrence(scheduleTask.Id);

                var model = new CronTaskModel()
                {
                    ScheduleTaskId = scheduleTask.Id,
                    ScheduleTaskModel = scheduleTaskModel,
                    CronExpression = ct.Value,
                    ExecutionStatus = _cronTaskService.GetQuartzJobExecutionStatus(scheduleTask.Id),
                    CronNextOccurrence = cronNextOccurence.HasValue ?
                        (await _dateTimeHelper.ConvertToUserTimeAsync(cronNextOccurence.Value, DateTimeKind.Utc)).ToString("G") :
                        string.Empty
                };

                result.Add(model);
            }

            return result;
        }
    }
}
