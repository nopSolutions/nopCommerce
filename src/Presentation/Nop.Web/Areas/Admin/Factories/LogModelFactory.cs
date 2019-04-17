using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Logging;
using Nop.Core.Html;
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
    /// Represents the log model factory implementation
    /// </summary>
    public partial class LogModelFactory : ILogModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public LogModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            ILogger logger)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _logger = logger;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare datatables model
        /// </summary>
        /// <param name="searchModel">Search model</param>
        /// <returns>Datatables model</returns>
        protected virtual DataTablesModel PrepareCategoryGridModel(LogSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "log-grid",
                UrlRead = new DataUrl("LogList", "Log", null),
                SearchButtonId = "search-log",
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = new List<FilterParameter>
            {
                new FilterParameter(nameof(searchModel.CreatedOnFrom)),
                new FilterParameter(nameof(searchModel.CreatedOnTo)),
                new FilterParameter(nameof(searchModel.Message)),
                new FilterParameter(nameof(searchModel.LogLevelId))
            };

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(LogModel.Id))
                {
                    IsMasterCheckBox = true,
                    Render = new RenderCheckBox("checkbox_log"),
                    ClassName =  StyleColumn.CenterAll,
                    Width = "50",
                },
                new ColumnProperty(nameof(LogModel.LogLevel))
                {
                    Title = _localizationService.GetResource("Admin.System.Log.Fields.LogLevel"),
                    Width = "100"
                },
                new ColumnProperty(nameof(LogModel.ShortMessage))
                {
                    Title = _localizationService.GetResource("Admin.System.Log.Fields.ShortMessage")
                },
                new ColumnProperty(nameof(LogModel.CreatedOn))
                {
                    Title = _localizationService.GetResource("Admin.System.Log.Fields.CreatedOn"),
                    Width = "200",
                    Render = new RenderDate()
                },
                new ColumnProperty(nameof(LogModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.View"),
                    Width = "100",
                    ClassName =  StyleColumn.CenterAll,
                    Render = new RenderButtonView(new DataUrl("View"))
                }
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare log search model
        /// </summary>
        /// <param name="searchModel">Log search model</param>
        /// <returns>Log search model</returns>
        public virtual LogSearchModel PrepareLogSearchModel(LogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available log levels
            _baseAdminModelFactory.PrepareLogLevels(searchModel.AvailableLogLevels);

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareCategoryGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged log list model
        /// </summary>
        /// <param name="searchModel">Log search model</param>
        /// <returns>Log list model</returns>
        public virtual LogListModel PrepareLogListModel(LogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter log
            var createdOnFromValue = searchModel.CreatedOnFrom.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone) : null;
            var createdToFromValue = searchModel.CreatedOnTo.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1) : null;
            var logLevel = searchModel.LogLevelId > 0 ? (LogLevel?)searchModel.LogLevelId : null;

            //get log
            var logItems = _logger.GetAllLogs(message: searchModel.Message,
                fromUtc: createdOnFromValue,
                toUtc: createdToFromValue,
                logLevel: logLevel,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new LogListModel().PrepareToGrid(searchModel, logItems, () =>
            {
                //fill in model values from the entity
                return logItems.Select(logItem =>
                {
                    //fill in model values from the entity
                    var logModel = logItem.ToModel<LogModel>();

                    //convert dates to the user time
                    logModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    logModel.LogLevel = _localizationService.GetLocalizedEnum(logItem.LogLevel);
                    logModel.ShortMessage = HtmlHelper.FormatText(logItem.ShortMessage, false, true, false, false, false, false);
                    logModel.FullMessage = string.Empty;
                    logModel.CustomerEmail = logItem.Customer?.Email ?? string.Empty;

                    return logModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare log model
        /// </summary>
        /// <param name="model">Log model</param>
        /// <param name="log">Log</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Log model</returns>
        public virtual LogModel PrepareLogModel(LogModel model, Log log, bool excludeProperties = false)
        {
            if (log != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = log.ToModel<LogModel>();

                    model.LogLevel = _localizationService.GetLocalizedEnum(log.LogLevel);
                    model.ShortMessage = HtmlHelper.FormatText(log.ShortMessage, false, true, false, false, false, false);
                    model.FullMessage = HtmlHelper.FormatText(log.FullMessage, false, true, false, false, false, false);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc);
                    model.FullMessage = string.Empty;
                    model.CustomerEmail = log.Customer?.Email ?? string.Empty;
                }
            }
            return model;
        }

        #endregion
    }
}