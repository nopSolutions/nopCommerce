using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Helpers;
using Nop.Services.ScheduleTasks;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Models.Extensions;
using Task = System.Threading.Tasks.Task;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the schedule task model factory implementation
    /// </summary>
    public partial class ScheduleTaskModelFactory : IScheduleTaskModelFactory
    {
        #region Fields

        protected IDateTimeHelper DateTimeHelper { get; }
        protected IScheduleTaskService ScheduleTaskService { get; }

        #endregion

        #region Ctor

        public ScheduleTaskModelFactory(IDateTimeHelper dateTimeHelper,
            IScheduleTaskService scheduleTaskService)
        {
            DateTimeHelper = dateTimeHelper;
            ScheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare schedule task search model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the schedule task search model
        /// </returns>
        public virtual Task<ScheduleTaskSearchModel> PrepareScheduleTaskSearchModelAsync(ScheduleTaskSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged schedule task list model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the schedule task list model
        /// </returns>
        public virtual async Task<ScheduleTaskListModel> PrepareScheduleTaskListModelAsync(ScheduleTaskSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get schedule tasks
            var scheduleTasks = (await ScheduleTaskService.GetAllTasksAsync(true)).ToPagedList(searchModel);

            //prepare list model
            var model = await new ScheduleTaskListModel().PrepareToGridAsync(searchModel, scheduleTasks, () =>
            {
                return scheduleTasks.SelectAwait(async scheduleTask =>
                {
                    //fill in model values from the entity
                    var scheduleTaskModel = scheduleTask.ToModel<ScheduleTaskModel>();

                    //convert dates to the user time
                    if (scheduleTask.LastStartUtc.HasValue)
                    {
                        scheduleTaskModel.LastStartUtc = (await DateTimeHelper
                            .ConvertToUserTimeAsync(scheduleTask.LastStartUtc.Value, DateTimeKind.Utc)).ToString("G");
                    }

                    if (scheduleTask.LastEndUtc.HasValue)
                    {
                        scheduleTaskModel.LastEndUtc = (await DateTimeHelper
                            .ConvertToUserTimeAsync(scheduleTask.LastEndUtc.Value, DateTimeKind.Utc)).ToString("G");
                    }

                    if (scheduleTask.LastSuccessUtc.HasValue)
                    {
                        scheduleTaskModel.LastSuccessUtc = (await DateTimeHelper
                            .ConvertToUserTimeAsync(scheduleTask.LastSuccessUtc.Value, DateTimeKind.Utc)).ToString("G");
                    }

                    return scheduleTaskModel;
                });
            });
            return model;
        }

        #endregion
    }
}