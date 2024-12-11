using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data;

namespace Nop.Services.Logging;

/// <summary>
/// Default logger
/// </summary>
public partial class DefaultLogger : ILogger
{
    #region Fields

    protected readonly CommonSettings _commonSettings;
    protected readonly CustomerSettings _customerSettings;

    protected readonly IRepository<Log> _logRepository;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public DefaultLogger(CommonSettings commonSettings,
        CustomerSettings customerSettings,
        IRepository<Log> logRepository,
        IWebHelper webHelper)
    {
        _commonSettings = commonSettings;
        _customerSettings = customerSettings;
        _logRepository = logRepository;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Gets a value indicating whether this message should not be logged
    /// </summary>
    /// <param name="message">Message</param>
    /// <returns>Result</returns>
    protected virtual bool IgnoreLog(string message)
    {
        if (!_commonSettings.IgnoreLogWordlist.Any())
            return false;

        if (string.IsNullOrWhiteSpace(message))
            return false;

        return _commonSettings
            .IgnoreLogWordlist
            .Any(x => message.Contains(x, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Prepare log item
    /// </summary>
    /// <param name="logLevel">Log level</param>
    /// <param name="shortMessage">The short message</param>
    /// <param name="fullMessage">The full message</param>
    /// <param name="customer">The customer to associate log record with</param>
    /// <returns>Log item</returns>
    protected virtual Log PrepareLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
    {
        return new Log
        {
            LogLevel = logLevel,
            ShortMessage = shortMessage,
            FullMessage = fullMessage,
            IpAddress = _customerSettings.StoreIpAddresses ? _webHelper.GetCurrentIpAddress() : string.Empty,
            CustomerId = customer?.Id,
            PageUrl = _webHelper.GetThisPageUrl(true),
            ReferrerUrl = _webHelper.GetUrlReferrer(),
            CreatedOnUtc = DateTime.UtcNow
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether a log level is enabled
    /// </summary>
    /// <param name="level">Log level</param>
    /// <returns>Result</returns>
    public virtual bool IsEnabled(LogLevel level)
    {
        return level switch
        {
            LogLevel.Debug => false,
            _ => true,
        };
    }

    /// <summary>
    /// Deletes a log item
    /// </summary>
    /// <param name="log">Log item</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteLogAsync(Log log)
    {
        ArgumentNullException.ThrowIfNull(log);

        await _logRepository.DeleteAsync(log, false);
    }

    /// <summary>
    /// Deletes a log items
    /// </summary>
    /// <param name="logs">Log items</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteLogsAsync(IList<Log> logs)
    {
        await _logRepository.DeleteAsync(logs, false);
    }

    /// <summary>
    /// Clears a log
    /// </summary>
    /// <param name="olderThan">The date that sets the restriction on deleting records. Leave null to remove all records</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ClearLogAsync(DateTime? olderThan = null)
    {
        if (olderThan == null)
            await _logRepository.TruncateAsync();
        else
            await _logRepository.DeleteAsync(p => p.CreatedOnUtc < olderThan.Value);
    }

    /// <summary>
    /// Gets all log items
    /// </summary>
    /// <param name="fromUtc">Log item creation from; null to load all records</param>
    /// <param name="toUtc">Log item creation to; null to load all records</param>
    /// <param name="message">Message</param>
    /// <param name="logLevel">Log level; null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the log item items
    /// </returns>
    public virtual async Task<IPagedList<Log>> GetAllLogsAsync(DateTime? fromUtc = null, DateTime? toUtc = null,
        string message = "", LogLevel? logLevel = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var logs = await _logRepository.GetAllPagedAsync(query =>
        {
            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOnUtc);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOnUtc);
            if (logLevel.HasValue)
            {
                var logLevelId = (int)logLevel.Value;
                query = query.Where(l => logLevelId == l.LogLevelId);
            }

            if (!string.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
            query = query.OrderByDescending(l => l.CreatedOnUtc);

            return query;
        }, pageIndex, pageSize);

        return logs;
    }

    /// <summary>
    /// Gets a log item
    /// </summary>
    /// <param name="logId">Log item identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the log item
    /// </returns>
    public virtual async Task<Log> GetLogByIdAsync(int logId)
    {
        return await _logRepository.GetByIdAsync(logId);
    }

    /// <summary>
    /// Get log items by identifiers
    /// </summary>
    /// <param name="logIds">Log item identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the log items
    /// </returns>
    public virtual async Task<IList<Log>> GetLogByIdsAsync(int[] logIds)
    {
        return await _logRepository.GetByIdsAsync(logIds);
    }

    /// <summary>
    /// Inserts a log item
    /// </summary>
    /// <param name="logLevel">Log level</param>
    /// <param name="shortMessage">The short message</param>
    /// <param name="fullMessage">The full message</param>
    /// <param name="customer">The customer to associate log record with</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// </returns>
    public virtual async Task InsertLogAsync(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
    {
        //check ignore word/phrase list?
        if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
            return;

        await _logRepository.InsertAsync(PrepareLog(logLevel, shortMessage, fullMessage, customer), false);
    }

    /// <summary>
    /// Inserts a log item
    /// </summary>
    /// <param name="logLevel">Log level</param>
    /// <param name="shortMessage">The short message</param>
    /// <param name="fullMessage">The full message</param>
    /// <param name="customer">The customer to associate log record with</param>
    public virtual void InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "", Customer customer = null)
    {
        //check ignore word/phrase list?
        if (IgnoreLog(shortMessage) || IgnoreLog(fullMessage))
            return;
        
        _logRepository.Insert(PrepareLog(logLevel, shortMessage, fullMessage, customer), false);
    }

    /// <summary>
    /// Information
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InformationAsync(string message, Exception exception = null, Customer customer = null)
    {
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Information))
            await InsertLogAsync(LogLevel.Information, message, exception?.ToString() ?? string.Empty, customer);
    }

    /// <summary>
    /// Information
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    public virtual void Information(string message, Exception exception = null, Customer customer = null)
    {
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Information))
            InsertLog(LogLevel.Information, message, exception?.ToString() ?? string.Empty, customer);
    }

    /// <summary>
    /// Warning
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task WarningAsync(string message, Exception exception = null, Customer customer = null)
    {
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Warning))
            await InsertLogAsync(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, customer);
    }

    /// <summary>
    /// Warning
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    public virtual void Warning(string message, Exception exception = null, Customer customer = null)
    {
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Warning))
            InsertLog(LogLevel.Warning, message, exception?.ToString() ?? string.Empty, customer);
    }

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ErrorAsync(string message, Exception exception = null, Customer customer = null)
    {
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Error))
            await InsertLogAsync(LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
    }

    /// <summary>
    /// Error
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="exception">Exception</param>
    /// <param name="customer">Customer</param>
    public virtual void Error(string message, Exception exception = null, Customer customer = null)
    {
        //don't log thread abort exception
        if (exception is ThreadAbortException)
            return;

        if (IsEnabled(LogLevel.Error))
            InsertLog(LogLevel.Error, message, exception?.ToString() ?? string.Empty, customer);
    }

    #endregion
}