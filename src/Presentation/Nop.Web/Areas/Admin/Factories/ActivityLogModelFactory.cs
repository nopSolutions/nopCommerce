using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the activity log model factory implementation
    /// </summary>
    public partial class ActivityLogModelFactory : IActivityLogModelFactory
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public ActivityLogModelFactory(ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService)
        {
            this._customerActivityService = customerActivityService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare activity log type models
        /// </summary>
        /// <returns>List of activity log type models</returns>
        public virtual IList<ActivityLogTypeModel> PrepareActivityLogTypeModels()
        {
            //prepare available activity log types
            var availableActivityTypes = _customerActivityService.GetAllActivityTypes();
            var model = availableActivityTypes.Select(activityType => activityType.ToModel()).ToList();

            return model;
        }

        /// <summary>
        /// Prepare activity log search model
        /// </summary>
        /// <param name="model">Activity log search model</param>
        /// <returns>Activity log search model</returns>
        public virtual ActivityLogSearchModel PrepareActivityLogSearchModel(ActivityLogSearchModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available activity log types
            var availableActivityTypes = _customerActivityService.GetAllActivityTypes();
            model.ActivityLogType = availableActivityTypes.Select(activityType => new SelectListItem
            {
                Value = activityType.Id.ToString(),
                Text = activityType.Name
            }).ToList();

            //insert special type item for the "all" value
            model.ActivityLogType.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return model;
        }

        /// <summary>
        /// Prepare paged activity log list model for the grid
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <param name="command">Pagination parameters</param>
        /// <returns>Grid model</returns>
        public virtual DataSourceResult PrepareActivityLogListGridModel(ActivityLogSearchModel searchModel, DataSourceRequest command)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            //get parameters to filter log
            var startDateValue = searchModel.CreatedOnFrom == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = searchModel.CreatedOnTo == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //get log
            var activityLog = _customerActivityService.GetAllActivities(createdOnFrom: startDateValue,
                createdOnTo: endDateValue,
                activityLogTypeId: searchModel.ActivityLogTypeId,
                ipAddress: searchModel.IpAddress,
                pageIndex: command.Page - 1, pageSize: command.PageSize);

            //prepare grid model
            var model = new DataSourceResult
            {
                Data = activityLog.Select(logItem =>
                {
                    //fill in model values from the entity
                    var logItemModel = logItem.ToModel();

                    //convert dates to the user time
                    logItemModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return logItemModel;

                }),
                Total = activityLog.TotalCount
            };

            return model;
        }

        #endregion
    }
}