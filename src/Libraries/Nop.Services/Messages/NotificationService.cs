using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Logging;

namespace Nop.Services.Messages;

/// <summary>
/// Notification service
/// </summary>
public partial class NotificationService : INotificationService
{
    #region Fields

    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILogger _logger;
    protected readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public NotificationService(IHttpContextAccessor httpContextAccessor,
        ILogger logger,
        ITempDataDictionaryFactory tempDataDictionaryFactory,
        IWorkContext workContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _tempDataDictionaryFactory = tempDataDictionaryFactory;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Save message into TempData
    /// </summary>
    /// <param name="type">Notification type</param>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time (in milliseconds), on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    protected virtual void PrepareTempData(NotifyType type, string message, bool encode = true, int timeout = 0)
    {
        var context = _httpContextAccessor.HttpContext;
        var tempData = _tempDataDictionaryFactory.GetTempData(context);

        //Messages have stored in a serialized list
        var messages = tempData.TryGetValue(NopMessageDefaults.NotificationListKey, out var value) 
            ? JsonConvert.DeserializeObject<IList<NotifyData>>(value.ToString())
            : new List<NotifyData>();

        messages.Add(new NotifyData
        {
            Message = message,
            Type = type,
            Encode = encode,
            Timeout = timeout
        });

        tempData[NopMessageDefaults.NotificationListKey] = JsonConvert.SerializeObject(messages);
    }

    /// <summary>
    /// Log exception
    /// </summary>
    /// <param name="exception">Exception</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task LogExceptionAsync(Exception exception)
    {
        if (exception == null)
            return;
        var customer = await _workContext.GetCurrentCustomerAsync();
        await _logger.ErrorAsync(exception.Message, exception, customer);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Display notification
    /// </summary>
    /// <param name="type">Notification type</param>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time (in milliseconds) on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    public virtual void Notification(NotifyType type, string message, bool encode = true, int timeout = 0)
    {
        PrepareTempData(type, message, encode, timeout);
    }

    /// <summary>
    /// Display success notification
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time (in milliseconds) on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    public virtual void SuccessNotification(string message, bool encode = true, int timeout = 0)
    {
        PrepareTempData(NotifyType.Success, message, encode, timeout);
    }

    /// <summary>
    /// Display warning notification
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time (in milliseconds) on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    public virtual void WarningNotification(string message, bool encode = true, int timeout = 0)
    {
        PrepareTempData(NotifyType.Warning, message, encode, timeout);
    }

    /// <summary>
    /// Display error notification
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="encode">A value indicating whether the message should not be encoded</param>
    /// <param name="timeout">The time (in milliseconds) on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    public virtual void ErrorNotification(string message, bool encode = true, int timeout = 0)
    {
        PrepareTempData(NotifyType.Error, message, encode, timeout);
    }

    /// <summary>
    /// Display error notification
    /// </summary>
    /// <param name="exception">Exception</param>
    /// <param name="logException">A value indicating whether exception should be logged</param>
    /// <param name="timeout">The time (in milliseconds) on which notification will close automatically; leave 0 if notification shouldn't close automatically</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task ErrorNotificationAsync(Exception exception, bool logException = true, int timeout = 0)
    {
        if (exception == null)
            return;

        if (logException)
            await LogExceptionAsync(exception);

        ErrorNotification(exception.Message, timeout: timeout);
    }

    #endregion
}