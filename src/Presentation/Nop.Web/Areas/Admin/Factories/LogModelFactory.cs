using Nop.Core.Domain.Logging;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Framework.Models.Extensions;
using ILogger = Nop.Services.Logging.ILogger;
using LogLevel = Nop.Core.Domain.Logging.LogLevel;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the log model factory implementation
/// </summary>
public partial class LogModelFactory : ILogModelFactory
{
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;

    #endregion

    #region Ctor

    public LogModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        ILogger logger)
    {
        _baseAdminModelFactory = baseAdminModelFactory;
        _dateTimeHelper = dateTimeHelper;
        _customerService = customerService;
        _htmlFormatter = htmlFormatter;
        _localizationService = localizationService;
        _logger = logger;
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
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare available log levels
        await _baseAdminModelFactory.PrepareLogLevelsAsync(searchModel.AvailableLogLevels);

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
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter log
        var createdOnFromValue = searchModel.CreatedOnFrom.HasValue
            ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()) : null;
        var createdToFromValue = searchModel.CreatedOnTo.HasValue
            ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1) : null;
        var logLevel = searchModel.LogLevelId > 0 ? (LogLevel?)searchModel.LogLevelId : null;

        //get log
        var logItems = await _logger.GetAllLogsAsync(message: searchModel.Message,
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
                logModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                logModel.LogLevel = await _localizationService.GetLocalizedEnumAsync(logItem.LogLevel);
                logModel.ShortMessage = _htmlFormatter.FormatText(logItem.ShortMessage, false, true, false, false, false, false);
                logModel.FullMessage = string.Empty;
                logModel.CustomerEmail = (await _customerService.GetCustomerByIdAsync(logItem.CustomerId ?? 0))?.Email ?? string.Empty;

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

                model.LogLevel = await _localizationService.GetLocalizedEnumAsync(log.LogLevel);
                model.ShortMessage = _htmlFormatter.FormatText(log.ShortMessage, false, true, false, false, false, false);
                model.FullMessage = _htmlFormatter.FormatText(log.FullMessage, false, true, false, false, false, false);
                model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(log.CreatedOnUtc, DateTimeKind.Utc);
                model.CustomerEmail = log.CustomerId.HasValue ? (await _customerService.GetCustomerByIdAsync(log.CustomerId.Value))?.Email : string.Empty;
            }
        }
        return model;
    }

    #endregion
}