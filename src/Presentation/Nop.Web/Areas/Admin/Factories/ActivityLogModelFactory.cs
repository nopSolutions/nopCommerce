using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.DataTables;
using Nop.Services.Localization;
using Newtonsoft.Json.Linq;

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

        #region Methods

        /// <summary>
        /// Prepare activity log type models
        /// </summary>
        /// <returns>List of activity log type models</returns>
        public virtual IList<ActivityLogTypeModel> PrepareActivityLogTypeModels()
        {
            //prepare available activity log types
            var availableActivityTypes = _customerActivityService.GetAllActivityTypes();
            var models = availableActivityTypes.Select(activityType => activityType.ToModel<ActivityLogTypeModel>()).ToList();

            return models;
        }

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

            //prepare page parameters
            searchModel.SetGridPageSize();

            #region prepare grid model

            List<ColumnProperty> columns = new List<ColumnProperty>
            {
                new ColumnProperty()
                {
                    Data = "Name",
                    Title = _localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLogType.Fields.Name"),
                    AutoWidth = true
                },
                new ColumnProperty()
                {
                    Data = "Enabled",
                    Title = "<div class='checkbox'><label><input id='mastercheckbox' type='checkbox'/>" + _localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLogType.Fields.Enabled") + "</label></div>",
                    Width =  "100",
                    Render = new RenderCheckBox("checkbox_activity_types")
                }
            };

            List<ColumnDefinition> ColDef = new List<ColumnDefinition>
            {
                new ColumnDefinition()
                {
                    Targets = "-1",
                    ClassName =  "dt-head-center dt-body-center"
                }
            };

            //prepare Data
            var activityLogTypes = new JArray() as dynamic;            
            
            foreach (var altm in searchModel.ActivityLogTypeListModel)
            {
                dynamic activityLogType = new JObject();
                activityLogType.Id = altm.Id;
                activityLogType.Name = altm.Name;
                activityLogType.Enabled = altm.Enabled.ToString().ToLowerInvariant();

                activityLogTypes.Add(activityLogType);
            };

            searchModel.Grid = new DataTablesModel
            {
                Name = "activityLogType-grid",
                Data = activityLogTypes,
                Paging = false,
                LengthMenu = searchModel.AvailablePageSizes,
                ColumnCollection = columns,
                ColumnDefs = ColDef
            };

            #endregion

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

            //prepare page parameters
            searchModel.SetGridPageSize();

            #region prepare grid model

            List<string> Filters = new List<string>()
            {
                "CreatedOnTo",
                "CreatedOnFrom",
                "IpAddress",
                "ActivityLogTypeId"
            };

            List<ColumnProperty> columns = new List<ColumnProperty>
            {
                new ColumnProperty()
                {
                    Data = "Id"
                },
                new ColumnProperty()
                {
                    Data = "CustomerId"
                },
                new ColumnProperty()
                {
                    Data = "ActivityLogTypeName",
                    Title = _localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType"),
                    Width = "200"
                },
                new ColumnProperty()
                {
                    Data = "CustomerEmail",
                    Title = _localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLog.Fields.Customer"),
                    Width = "100",
                    Render = new RenderLink(new DataUrl("~/Admin/Customer/Edit/", "CustomerId"), "data")
                },
                new ColumnProperty()
                {
                    Data = "IpAddress",
                    Title = _localizationService.GetResource("Admin.Customers.Customers.ActivityLog.IpAddress"),
                    Width = "100"
                },
                new ColumnProperty()
                {
                    Data = "Comment",
                    Title = _localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLog.Fields.Comment")
                },

                new ColumnProperty()
                {
                    Data = "CreatedOn",
                    Title = _localizationService.GetResource("Admin.System.Log.Fields.CreatedOn"),
                    Width = "200",
                    Render = new RenderDate("")
                },
                new ColumnProperty()
                {
                    Data = "Id",
                    Title = _localizationService.GetResource("Admin.Common.Delete"),
                    Render = new RenderButton(_localizationService.GetResource("Admin.Common.Delete"))
                }
            };

            List<ColumnDefinition> ColDef = new List<ColumnDefinition>
            {
                new ColumnDefinition()
                {
                    Targets = "[0, 1]",
                    Visible = false,
                    Searchable = false
                },
                new ColumnDefinition()
                {
                    Targets = "-1",
                    ClassName =  "dt-head-center dt-body-center",
                    Width = "100"
                }
            };

            searchModel.Grid = new DataTablesModel
            {
                Name = "activityLog-grid",
                ServerSide = true,
                Processing = true,
                UrlRead = new DataUrl("ListLogs", "ActivityLog"),
                UrlAction = new DataUrl("AcivityLogDelete", "ActivityLog"),
                LengthMenu = searchModel.AvailablePageSizes,
                SearchButtonId = "search-log",
                Filters = Filters,
                ColumnCollection = columns,
                ColumnDefs = ColDef
            };

            #endregion

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
                pageIndex: (searchModel.Start / searchModel.Length), pageSize: searchModel.Length);

            //prepare list model
            var model = new ActivityLogListModel
            {
                Data = activityLog.Select(logItem =>
                {
                    //fill in model values from the entity
                    var logItemModel = logItem.ToModel<ActivityLogModel>();
                    logItemModel.ActivityLogTypeName = logItem.ActivityLogType.Name;
                    logItemModel.CustomerEmail = logItem.Customer.Email;

                    //convert dates to the user time
                    logItemModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return logItemModel;

                }),
                Draw = searchModel.Draw,
                RecordsTotal = activityLog.TotalCount,
                RecordsFiltered = activityLog.TotalCount
            };

            return model;
        }

        #endregion
    }
}