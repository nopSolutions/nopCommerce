using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Logging;
using Nop.Core.Html;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the log model factory implementation
    /// </summary>
    public partial class LogModelFactory : ILogModelFactory
    {
        #region Fields

        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILogger Logger { get; }

        #endregion

        #region Ctor

        public LogModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            ILogger logger)
        {
            BaseAdminModelFactory = baseAdminModelFactory;
            DateTimeHelper = dateTimeHelper;
            CustomerService = customerService;
            LocalizationService = localizationService;
            Logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare log search model
        /// </summary>
        /// <param name="searchModel">Log search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the log search model
        /// </returns>
        public virtual async Task<LogSearchModel> PrepareLogSearchModelAsync(LogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available log levels
            await BaseAdminModelFactory.PrepareLogLevelsAsync(searchModel.AvailableLogLevels);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged log list model
        /// </summary>
        /// <param name="searchModel">Log search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the log list model
        /// </returns>
        public virtual async Task<LogListModel> PrepareLogListModelAsync(LogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter log
            var createdOnFromValue = searchModel.CreatedOnFrom.HasValue
                ? (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()) : null;
            var createdToFromValue = searchModel.CreatedOnTo.HasValue
                ? (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1) : null;
            var logLevel = searchModel.LogLevelId > 0 ? (LogLevel?)searchModel.LogLevelId : null;

            //get log
            var logItems = await Logger.GetAllLogsAsync(message: searchModel.Message,
                fromUtc: createdOnFromValue,
                toUtc: createdToFromValue,
                logLevel: logLevel,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new LogListModel().PrepareToGridAsync(searchModel, logItems, () =>
            {
                //fill in model values from the entity
                return logItems.SelectAwait(async logItem =>
                {
                    //fill in model values from the entity
                    var logModel = logItem.ToModel<LogModel>();

                    //convert dates to the user time
                    logModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    logModel.LogLevel = await LocalizationService.GetLocalizedEnumAsync(logItem.LogLevel);
                    logModel.ShortMessage = HtmlHelper.FormatText(logItem.ShortMessage, false, true, false, false, false, false);
                    logModel.FullMessage = string.Empty;
                    logModel.CustomerEmail = (await CustomerService.GetCustomerByIdAsync(logItem.CustomerId ?? 0))?.Email ?? string.Empty;

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the log model
        /// </returns>
        public virtual async Task<LogModel> PrepareLogModelAsync(LogModel model, Log log, bool excludeProperties = false)
        {
            if (log != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = log.ToModel<LogModel>();

                    model.LogLevel = await LocalizationService.GetLocalizedEnumAsync(log.LogLevel);
                    model.ShortMessage = HtmlHelper.FormatText(log.ShortMessage, false, true, false, false, false, false);
                    model.FullMessage = HtmlHelper.FormatText(log.FullMessage, false, true, false, false, false, false);
                    model.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc);
                    model.CustomerEmail = log.CustomerId.HasValue ? (await CustomerService.GetCustomerByIdAsync(log.CustomerId.Value))?.Email : string.Empty;
                }
            }
            return model;
        }

        #endregion
    }
}