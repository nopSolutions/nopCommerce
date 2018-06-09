using System;
using System.Linq;
using Nop.Services.Helpers;
using Nop.Services.Tasks;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the schedule task model factory implementation
    /// </summary>
    public partial class ScheduleTaskModelFactory : IScheduleTaskModelFactory
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IScheduleTaskService _scheduleTaskService;

        #endregion

        #region Ctor

        public ScheduleTaskModelFactory(IDateTimeHelper dateTimeHelper,
            IScheduleTaskService scheduleTaskService)
        {
            this._dateTimeHelper = dateTimeHelper;
            this._scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare schedule task search model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>Schedule task search model</returns>
        public virtual ScheduleTaskSearchModel PrepareScheduleTaskSearchModel(ScheduleTaskSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged schedule task list model
        /// </summary>
        /// <param name="searchModel">Schedule task search model</param>
        /// <returns>Schedule task list model</returns>
        public virtual ScheduleTaskListModel PrepareScheduleTaskListModel(ScheduleTaskSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get schedule tasks
            var scheduleTasks = _scheduleTaskService.GetAllTasks(true);

            //prepare list model
            var model = new ScheduleTaskListModel
            {
                Data = scheduleTasks.PaginationByRequestModel(searchModel).Select(scheduleTask =>
                {
                    //fill in model values from the entity
                    var scheduleTaskModel = scheduleTask.ToModel<ScheduleTaskModel>();

                    //convert dates to the user time
                    if (scheduleTask.LastStartUtc.HasValue)
                    {
                        scheduleTaskModel.LastStartUtc = _dateTimeHelper
                            .ConvertToUserTime(scheduleTask.LastStartUtc.Value, DateTimeKind.Utc).ToString("G");
                    }

                    if (scheduleTask.LastEndUtc.HasValue)
                    {
                        scheduleTaskModel.LastEndUtc = _dateTimeHelper
                            .ConvertToUserTime(scheduleTask.LastEndUtc.Value, DateTimeKind.Utc).ToString("G");
                    }

                    if (scheduleTask.LastSuccessUtc.HasValue)
                    {
                        scheduleTaskModel.LastSuccessUtc = _dateTimeHelper
                            .ConvertToUserTime(scheduleTask.LastSuccessUtc.Value, DateTimeKind.Utc).ToString("G");
                    }

                    return scheduleTaskModel;
                }),
                Total = scheduleTasks.Count
            };

            return model;
        }

        #endregion
    }
}