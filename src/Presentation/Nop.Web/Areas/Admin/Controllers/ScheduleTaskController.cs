using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.ScheduleTasks;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ScheduleTaskController : BaseAdminController
    {
        #region Fields

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IScheduleTaskModelFactory ScheduleTaskModelFactory { get; }
        protected IScheduleTaskService ScheduleTaskService { get; }
        protected IScheduleTaskRunner TaskRunner { get; }

        #endregion

        #region Ctor

        public ScheduleTaskController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IScheduleTaskModelFactory scheduleTaskModelFactory,
            IScheduleTaskService scheduleTaskService,
            IScheduleTaskRunner taskRunner)
        {
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            ScheduleTaskModelFactory = scheduleTaskModelFactory;
            ScheduleTaskService = scheduleTaskService;
            TaskRunner = taskRunner;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            //prepare model
            var model = await ScheduleTaskModelFactory.PrepareScheduleTaskSearchModelAsync(new ScheduleTaskSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ScheduleTaskSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ScheduleTaskModelFactory.PrepareScheduleTaskListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TaskUpdate(ScheduleTaskModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            //try to get a schedule task with the specified id
            var scheduleTask = await ScheduleTaskService.GetTaskByIdAsync(model.Id)
                               ?? throw new ArgumentException("Schedule task cannot be loaded");

            //To prevent inject the XSS payload in Schedule tasks ('Name' field), we must disable editing this field, 
            //but since it is required, we need to get its value before updating the entity.
            if (!string.IsNullOrEmpty(scheduleTask.Name))
            {
                model.Name = scheduleTask.Name;
                ModelState.Remove(nameof(model.Name));
            }

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            if(!scheduleTask.Enabled && model.Enabled)
                scheduleTask.LastEnabledUtc = DateTime.UtcNow;
            
            scheduleTask = model.ToEntity(scheduleTask);

            await ScheduleTaskService.UpdateTaskAsync(scheduleTask);

            //activity log
            await CustomerActivityService.InsertActivityAsync("EditTask",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditTask"), scheduleTask.Id), scheduleTask);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> RunNow(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            try
            {
                //try to get a schedule task with the specified id
                var scheduleTask = await ScheduleTaskService.GetTaskByIdAsync(id)
                                   ?? throw new ArgumentException("Schedule task cannot be loaded", nameof(id));

                await TaskRunner.ExecuteAsync(scheduleTask, true, true, false);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.System.ScheduleTasks.RunNow.Done"));
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("List");
        }

        #endregion
    }
}