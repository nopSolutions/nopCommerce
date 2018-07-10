using System;
using System.Linq;
using Nop.Core.Domain.Logging;
using Nop.Core.Html;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Models.Logging;

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
            this._baseAdminModelFactory = baseAdminModelFactory;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._logger = logger;
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
            var model = new LogListModel
            {
                //fill in model values from the entity
                Data = logItems.Select(logItem =>
                {
                    //fill in model values from the entity
                    var logModel = new LogModel
                    {
                        Id = logItem.Id,
                        IpAddress = logItem.IpAddress,
                        CustomerId = logItem.CustomerId,
                        PageUrl = logItem.PageUrl,
                        ReferrerUrl = logItem.ReferrerUrl
                    };

                    //little performance optimization: ensure that "FullMessage" is not returned
                    logModel.FullMessage = string.Empty;

                    //convert dates to the user time
                    logModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    logModel.CustomerEmail = logItem.Customer?.Email;
                    logModel.LogLevel = _localizationService.GetLocalizedEnum(logItem.LogLevel);
                    logModel.ShortMessage = HtmlHelper.FormatText(logItem.ShortMessage, false, true, false, false, false, false);

                    return logModel;
                }),
                Total = logItems.TotalCount
            };

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
                model = model ?? new LogModel
                {
                    Id = log.Id,
                    LogLevel = _localizationService.GetLocalizedEnum(log.LogLevel),
                    ShortMessage = HtmlHelper.FormatText(log.ShortMessage, false, true, false, false, false, false),
                    FullMessage = HtmlHelper.FormatText(log.FullMessage, false, true, false, false, false, false),
                    IpAddress = log.IpAddress,
                    CustomerId = log.CustomerId,
                    CustomerEmail = log.Customer?.Email,
                    PageUrl = log.PageUrl,
                    ReferrerUrl = log.ReferrerUrl,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc)
                };
            }

            return model;
        }

        #endregion
    }
}