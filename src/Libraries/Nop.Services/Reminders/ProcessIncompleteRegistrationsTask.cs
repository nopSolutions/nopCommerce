using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Reminders;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;
using Nop.Services.Stores;

namespace Nop.Services.Reminders;

/// <summary>
/// Represents a task to process incomplete registrations 
/// </summary>
public partial class ProcessIncompleteRegistrationsTask : IScheduleTask
{
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<GenericAttribute> _genericAttributeRepository;
    protected readonly ISettingService _settingService;
    protected readonly IStoreService _storeService;
    protected readonly IWorkflowMessageService _workflowMessageService;

    #endregion

    #region Ctor

    public ProcessIncompleteRegistrationsTask(CustomerSettings customerSettings,
        IMessageTemplateService messageTemplateService,
        IRepository<Customer> customerRepository,
        IRepository<GenericAttribute> genericAttributeRepository,
        ISettingService settingService,
        IStoreService storeService,
        IWorkflowMessageService workflowMessageService)
    {
        _customerSettings = customerSettings;
        _messageTemplateService = messageTemplateService;
        _customerRepository = customerRepository;
        _genericAttributeRepository = genericAttributeRepository;
        _settingService = settingService;
        _storeService = storeService;
        _workflowMessageService = workflowMessageService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        //ensure the email validation currently enabled after registration
        if (_customerSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
            return;

        foreach (var store in await _storeService.GetAllStoresAsync())
        {
            var reminderSettings = await _settingService.LoadSettingAsync<ReminderSettings>(store.Id);
            if (!reminderSettings.IncompleteRegistrationEnabled)
                continue;

            var messageTemplates = await _messageTemplateService
                .GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE, storeId: store.Id);
            var followUpMessage = messageTemplates.FirstOrDefault(template => template.IsActive && template.DelayBeforeSend is not null);

            if (followUpMessage is null)
                continue;

            var timeToFollowUp = DateTime.UtcNow - TimeSpan.FromHours(followUpMessage.DelayPeriod.ToHours(followUpMessage.DelayBeforeSend.Value));

            //find customers to follow-up about registration activation
            var customersToFollowUp =
                from c in _customerRepository.Table
                join incomplteAttr in _genericAttributeRepository.Table on
                    new { EntityId = c.Id, KeyGroup = nameof(Customer), Key = NopCustomerDefaults.AccountActivationTokenAttribute } equals
                    new { incomplteAttr.EntityId, incomplteAttr.KeyGroup, incomplteAttr.Key }
                where !c.Deleted && !c.Active
                    && (reminderSettings.ProcessingStartDateUtc == null || c.CreatedOnUtc >= reminderSettings.ProcessingStartDateUtc)
                    && c.RegisteredInStoreId == store.Id
                    && c.RegistrationFollowUpDateUtc == null
                    && incomplteAttr.CreatedOrUpdatedDateUTC != null && incomplteAttr.CreatedOrUpdatedDateUTC.Value < timeToFollowUp
                select c;

            var skip = 0;
            while (true)
            {
                var customersToFollowUpPaged = customersToFollowUp.Skip(skip).Take(NopReminderDefaults.ProcessingBatchSize).ToList();
                if (!customersToFollowUpPaged.Any())
                    break;

                var followedUpCustomers = new List<Customer>();
                try
                {
                    foreach (var customer in customersToFollowUpPaged)
                    {
                        var emailIds = await _workflowMessageService.SendIncompleteRegistrationNotificationMessageAsync(customer);

                        //the follow up has been sent
                        if (emailIds.Any())
                        {
                            customer.RegistrationFollowUpDateUtc = DateTime.UtcNow;
                            followedUpCustomers.Add(customer);
                        }
                    }
                }
                finally
                {
                    await _customerRepository.UpdateAsync(followedUpCustomers);
                }

                if (customersToFollowUpPaged.Count < NopReminderDefaults.ProcessingBatchSize)
                    break;

                skip += NopReminderDefaults.ProcessingBatchSize;
            }
        }
    }

    #endregion
}