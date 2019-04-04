using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the activity log model factory implementation
    /// </summary>
    public partial class ActivityLogModelFactory : IActivityLogModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public ActivityLogModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            IDateTimeHelper dateTimeHelper)
        {
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare activity log type models
        /// </summary>
        /// <returns>List of activity log type models</returns>
        protected virtual IList<ActivityLogTypeModel> PrepareActivityLogTypeModels()
        {
            //prepare available activity log types
            var availableActivityTypes = _customerActivityService.GetAllActivityTypes();
            var models = availableActivityTypes.Select(activityType => activityType.ToModel<ActivityLogTypeModel>()).ToList();

            return models;
        }

        /// <summary>
        /// Prepare activity log type datatables models
        /// </summary>
        /// <param name="searchModel">Activity log types search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareActivityLogTypeGridModel(ActivityLogTypeSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "activityLogType-grid",
                Paging = false,
                ServerSide = false,
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(ActivityLogTypeModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLogType.Fields.Name"),
                    AutoWidth = true
                },
                new ColumnProperty(nameof(ActivityLogTypeModel.Enabled))
                {
                    IsMasterCheckBox = true,
                    Title = _localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLogType.Fields.Enabled"),
                    Width =  "100",
                    Render = new RenderCheckBox("checkbox_activity_types")
                }
            };

            //prepare column definitions
            model.ColumnDefinitions = new List<ColumnDefinition>
            {
                new ColumnDefinition()
                {
                    Targets = "-1",
                    ClassName =  StyleColumn.CenterAll
                }
            };

            //prepare grid data
            model.Data = JsonConvert.SerializeObject(searchModel.ActivityLogTypeListModel.Select(typeModel => new
            {
                Id = typeModel.Id,
                Name = typeModel.Name,
                Enabled = typeModel.Enabled.ToString().ToLowerInvariant()
            }).ToList());

            return model;
        }

        /// <summary>
        /// Prepare activity log datatables models
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareActivityLogGridModel(ActivityLogSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "activityLog-grid",
                UrlRead = new DataUrl("ListLogs", "ActivityLog", null),
                UrlAction = new DataUrl("ActivityLogDelete", "ActivityLog", null),
                SearchButtonId = "search-log",
                SearchModelType = typeof(ActivityLogSearchModel),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<string>()
            {
                nameof(ActivityLogSearchModel.CreatedOnTo),
                nameof(ActivityLogSearchModel.CreatedOnFrom),
                nameof(ActivityLogSearchModel.IpAddress),
                nameof(ActivityLogSearchModel.ActivityLogTypeId)
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(ActivityLogModel.Id)),
                new ColumnProperty(nameof(ActivityLogModel.CustomerId)),
                new ColumnProperty(nameof(ActivityLogModel.ActivityLogTypeName))
                {
                    Title = _localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType"),
                    Width = "200"
                },
                new ColumnProperty(nameof(ActivityLogModel.CustomerEmail))
                {
                    Title = _localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLog.Fields.Customer"),
                    Width = "100",
                    Render = new RenderLink(new DataUrl("~/Admin/Customer/Edit", nameof(ActivityLogModel.CustomerId)))
                },
                new ColumnProperty(nameof(ActivityLogModel.IpAddress))
                {
                    Title = _localizationService.GetResource("Admin.Customers.Customers.ActivityLog.IpAddress"),
                    Width = "100"
                },
                new ColumnProperty(nameof(ActivityLogModel.Comment))
                {
                    Title = _localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLog.Fields.Comment")
                },
                new ColumnProperty(nameof(ActivityLogModel.CreatedOn))
                {
                    Title = _localizationService.GetResource("Admin.System.Log.Fields.CreatedOn"),
                    Width = "200",
                    Render = new RenderDate()
                },
                new ColumnProperty(nameof(ActivityLogModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Delete"),
                    Render = new RenderButton(_localizationService.GetResource("Admin.Common.Delete")) { Style = StyleButton.Danger }
                }
            };

            //prepare column definitions
            model.ColumnDefinitions = new List<ColumnDefinition>
            {
                new ColumnDefinition()
                {
                    Targets = "[0, 1]",
                    Visible = false
                },
                new ColumnDefinition()
                {
                    Targets = "-1",
                    ClassName =  StyleColumn.CenterAll,
                    Width = "100"
                }
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare activity log types search model
        /// </summary>
        /// <param name="searchModel">Activity log types search model</param>
        /// <returns>Activity log types search model</returns>
        public virtual ActivityLogTypeSearchModel PrepareActivityLogTypeSearchModel(ActivityLogTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.ActivityLogTypeListModel = PrepareActivityLogTypeModels();

            //prepare grid
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareActivityLogTypeGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare activity log search model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>Activity log search model</returns>
        public virtual ActivityLogSearchModel PrepareActivityLogSearchModel(ActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available activity log types
            _baseAdminModelFactory.PrepareActivityLogTypes(searchModel.ActivityLogType);

            //prepare grid
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareActivityLogGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged activity log list model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>Activity log list model</returns>
        public virtual ActivityLogListModel PrepareActivityLogListModel(ActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

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
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new ActivityLogListModel().PrepareToGrid(searchModel, activityLog, () =>
            {
                return activityLog.Select(logItem =>
                {
                    //fill in model values from the entity
                    var logItemModel = logItem.ToModel<ActivityLogModel>();
                    logItemModel.ActivityLogTypeName = logItem.ActivityLogType.Name;
                    logItemModel.CustomerEmail = logItem.Customer.Email;

                    //convert dates to the user time
                    logItemModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return logItemModel;

                });
            });

            return model;
        }

        #endregion
    }
}