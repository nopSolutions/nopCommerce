using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the activity log model factory implementation
/// </summary>
public partial class ActivityLogModelFactory : IActivityLogModelFactory
{
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;

    #endregion

    #region Ctor

    public ActivityLogModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper)
    {
        _baseAdminModelFactory = baseAdminModelFactory;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
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
        var availableActivityTypes = await _customerActivityService.GetAllActivityTypesAsync();
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
        ArgumentNullException.ThrowIfNull(searchModel);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available activity log types
        await _baseAdminModelFactory.PrepareActivityLogTypesAsync(searchModel.ActivityLogType);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter log
        var startDateValue = searchModel.CreatedOnFrom == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var endDateValue = searchModel.CreatedOnTo == null ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //get log
        var activityLog = await _customerActivityService.GetAllActivitiesAsync(createdOnFrom: startDateValue,
            createdOnTo: endDateValue,
            activityLogTypeId: searchModel.ActivityLogTypeId,
            ipAddress: searchModel.IpAddress,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        if (activityLog is null)
            return new ActivityLogListModel();

        //prepare list model
        var customerIds = activityLog.GroupBy(logItem => logItem.CustomerId).Select(logItem => logItem.Key);
        var activityLogCustomers = await _customerService.GetCustomersByIdsAsync(customerIds.ToArray());
        var model = await new ActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
        {
            return activityLog.SelectAwait(async logItem =>
            {
                //fill in model values from the entity
                var logItemModel = logItem.ToModel<ActivityLogModel>();
                logItemModel.ActivityLogTypeName = (await _customerActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;

                logItemModel.CustomerEmail = activityLogCustomers?.FirstOrDefault(x => x.Id == logItem.CustomerId)?.Email;

                //convert dates to the user time
                logItemModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                return logItemModel;
            });
        });

        return model;
    }

    #endregion
}