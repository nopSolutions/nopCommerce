using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the activity log model factory implementation
    /// </summary>
    public partial class ActivityLogModelFactory : IActivityLogModelFactory
    {
        #region Fields

        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }

        #endregion

        #region Ctor

        public ActivityLogModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper)
        {
            BaseAdminModelFactory = baseAdminModelFactory;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare activity log type models
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of activity log type models
        /// </returns>
        protected virtual async Task<IList<ActivityLogTypeModel>> PrepareActivityLogTypeModelsAsync()
        {
            //prepare available activity log types
            var availableActivityTypes = await CustomerActivityService.GetAllActivityTypesAsync();
            var models = availableActivityTypes.Select(activityType => activityType.ToModel<ActivityLogTypeModel>()).ToList();

            return models;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare activity log types search model
        /// </summary>
        /// <param name="searchModel">Activity log types search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log types search model
        /// </returns>
        public virtual async Task<ActivityLogTypeSearchModel> PrepareActivityLogTypeSearchModelAsync(ActivityLogTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.ActivityLogTypeListModel = await PrepareActivityLogTypeModelsAsync();

            //prepare grid
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare activity log search model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log search model
        /// </returns>
        public virtual async Task<ActivityLogSearchModel> PrepareActivityLogSearchModelAsync(ActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available activity log types
            await BaseAdminModelFactory.PrepareActivityLogTypesAsync(searchModel.ActivityLogType);

            //prepare grid
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged activity log list model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the activity log list model
        /// </returns>
        public virtual async Task<ActivityLogListModel> PrepareActivityLogListModelAsync(ActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter log
            var startDateValue = searchModel.CreatedOnFrom == null ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = searchModel.CreatedOnTo == null ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //get log
            var activityLog = await CustomerActivityService.GetAllActivitiesAsync(createdOnFrom: startDateValue,
                createdOnTo: endDateValue,
                activityLogTypeId: searchModel.ActivityLogTypeId,
                ipAddress: searchModel.IpAddress,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            if (activityLog is null)
                return new ActivityLogListModel();

            //prepare list model
            var customerIds = activityLog.GroupBy(logItem => logItem.CustomerId).Select(logItem => logItem.Key);
            var activityLogCustomers = await CustomerService.GetCustomersByIdsAsync(customerIds.ToArray());
            var model = await new ActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
            {
                return activityLog.SelectAwait(async logItem =>
                {
                    //fill in model values from the entity
                    var logItemModel = logItem.ToModel<ActivityLogModel>();
                    logItemModel.ActivityLogTypeName = (await CustomerActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;

                    logItemModel.CustomerEmail = activityLogCustomers?.FirstOrDefault(x => x.Id == logItem.CustomerId)?.Email;

                    //convert dates to the user time
                    logItemModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return logItemModel;
                });
            });

            return model;
        }

        #endregion
    }
}