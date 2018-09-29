using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Logging;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Notification service
    /// </summary>
    public partial class NotificationService : INotificationService
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private readonly IWorkContext _workContext;

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
        /// <param name="context">HttpContext</param>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        protected virtual void PrepareTempData(HttpContext context, NotifyType type, string message)
        {
            var tempData = _tempDataDictionaryFactory.GetTempData(context);

            //Messages have stored in a serialized list
            var messageList = tempData.ContainsKey(NopMessageDefaults.NotificationListKey)
                ? JsonConvert.DeserializeObject<IList<NotifyData>>(tempData[NopMessageDefaults.NotificationListKey].ToString())
                : new List<NotifyData>();

            messageList.Add(new NotifyData
            {
                Message = message,
                Type = type
            });

            tempData[NopMessageDefaults.NotificationListKey] = JsonConvert.SerializeObject(messageList);
        }

        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="exception">Exception</param>
        protected virtual void LogException(Exception exception)
        {
            if (exception == null)
                return;
            var customer = _workContext.CurrentCustomer;
            _logger.Error(exception.Message, exception, customer);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="context">HttpContext</param>
        public virtual void SuccessNotification(string message, HttpContext context = null)
        {
            PrepareTempData(context ?? _httpContextAccessor.HttpContext, NotifyType.Success, message);
        }

        /// <summary>
        /// Display warning notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="context">HttpContext</param>
        public virtual void WarningNotification(string message, HttpContext context = null)
        {
            PrepareTempData(context ?? _httpContextAccessor.HttpContext, NotifyType.Warning, message);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="context">HttpContext</param>
        public virtual void ErrorNotification(string message, HttpContext context = null)
        {
            PrepareTempData(context ?? _httpContextAccessor.HttpContext, NotifyType.Error, message);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        /// <param name="context">HttpContext</param>
        public virtual void ErrorNotification(Exception exception, bool logException = true, HttpContext context = null)
        {
            if (exception == null)
                return;

            if (logException)
                LogException(exception);

            ErrorNotification(exception.Message, context);
        }

        #endregion
    }
}
