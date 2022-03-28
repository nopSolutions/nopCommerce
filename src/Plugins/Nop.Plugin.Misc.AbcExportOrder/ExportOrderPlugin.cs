using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Plugin.Misc.AbcExportOrder.Tasks;
using Nop.Services.Logging;
using System;
using Nop.Core.Domain.Tasks;
using Nop.Services.Tasks;
using Nop.Services.Configuration;
using Nop.Plugin.Misc.AbcExportOrder.Extensions;
using Nop.Services.Plugins;
using Nop.Core;
using System.Collections.Generic;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Core.Domain.Messages;

namespace Nop.Plugin.Misc.AbcExportOrder
{
    public class ExportOrderPlugin : BasePlugin, IMiscPlugin, IConsumer<OrderPlacedEvent>
    {
        private readonly ILogger _logger;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILocalizationService _localizationService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;
        private readonly ExportOrderSettings _settings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IWebHelper _webHelper;

        private readonly string TaskType = $"{typeof(ResubmitOrdersTask).FullName}, {typeof(ExportOrderPlugin).Namespace}";


        public ExportOrderPlugin(
            ILogger logger,
            IEmailAccountService emailAccountService,
            IQueuedEmailService queuedEmailService,
            ILocalizationService localizationService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            ExportOrderSettings settings,
            EmailAccountSettings emailAccountSettings,
            IWebHelper webHelper
        )
        {
            _logger = logger;
            _emailAccountService = emailAccountService;
            _queuedEmailService = queuedEmailService;
            _localizationService = localizationService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _settings = settings;
            _emailAccountSettings = emailAccountSettings;
            _webHelper = webHelper;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AbcExportOrder/Configure";
        }

        public async System.Threading.Tasks.Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (!_settings.IsValid)
            {
                await _logger.WarningAsync("ExportOrder plugin settings invalid, order will not be exported to Yahoo tables.");
                return;
            }

            Order order = eventMessage.Order;

            try
            {
                await order.SubmitToISAMAsync();
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync($"Failure when submitting order #{order.Id} to ISAM", e);
                await SendAlertEmailAsync(order.Id, e.Message);
            }
        }

        public override async System.Threading.Tasks.Task InstallAsync()
        {
            await RemoveTaskAsync();
            await AddTaskAsync();
            await UpdateLocalizationsAsync();

            await base.InstallAsync();
        }

        private async System.Threading.Tasks.Task UpdateLocalizationsAsync()
        {
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                [ExportOrderLocaleKeys.OrderIdPrefix] = "Order ID Prefix",
                [ExportOrderLocaleKeys.OrderIdPrefixHint] = "The order ID prefix to use when sending orders to ISAM.",
                [ExportOrderLocaleKeys.TablePrefix] = "Table Prefix",
                [ExportOrderLocaleKeys.TablePrefixHint] = "The ISAM table prefix to send to.",
                [ExportOrderLocaleKeys.FailureAlertEmail] = "Failure Alert Email",
                [ExportOrderLocaleKeys.FailureAlertEmailHint] = "Email to send failure notifications to.",
            });
        }

        public override async System.Threading.Tasks.Task UninstallAsync()
        {
            await RemoveTaskAsync();

            await _localizationService.DeleteLocaleResourcesAsync(ExportOrderLocaleKeys.Base);

            await _settingService.DeleteSettingAsync<ExportOrderSettings>();

            await base.UninstallAsync();
        }

        private async System.Threading.Tasks.Task AddTaskAsync()
        {
            ScheduleTask task = new ScheduleTask
            {
                Name = $"Resubmit Failed ISAM Order Exports",
                Seconds = 14400,
                Type = TaskType,
                Enabled = true,
                StopOnError = false
            };

            await _scheduleTaskService.InsertTaskAsync(task);
        }

        private async System.Threading.Tasks.Task RemoveTaskAsync()
        {
            var task = await _scheduleTaskService.GetTaskByTypeAsync(TaskType);
            if (task != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(task);
            }
        }

        private async System.Threading.Tasks.Task SendAlertEmailAsync(int orderId, string exceptionMessage)
        {
            var failureEmail = _settings.FailureAlertEmail;
            if (!string.IsNullOrEmpty(failureEmail))
            {
                var defaultEmailAddress = await _emailAccountService.GetEmailAccountByIdAsync(
                    _emailAccountSettings.DefaultEmailAccountId
                );

                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    From = defaultEmailAddress.Email,
                    FromName = defaultEmailAddress.DisplayName,
                    To = failureEmail,
                    Subject = $"ISAM Order Export Failed (#{orderId})",
                    Body = $"More information:\n\n{exceptionMessage}",
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = defaultEmailAddress.Id,
                    DontSendBeforeDateUtc = null
                };

                await _queuedEmailService.InsertQueuedEmailAsync(email);
            }
            else
            {
                await _logger.WarningAsync("No failure alert email provided, will not provide alert notification.");
            }
        }
    }
}
