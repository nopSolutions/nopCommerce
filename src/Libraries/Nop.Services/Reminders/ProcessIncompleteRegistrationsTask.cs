using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Reminders;
using Nop.Data;
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
    protected readonly IStoreService _storeService;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly ReminderSettings _reminderSettings;

    #endregion

    #region Ctor

    public ProcessIncompleteRegistrationsTask(CustomerSettings customerSettings,
        IMessageTemplateService messageTemplateService,
        IRepository<Customer> customerRepository,
        IRepository<GenericAttribute> genericAttributeRepository,
        IStoreService storeService,
        IWorkflowMessageService workflowMessageService,
        ReminderSettings reminderSettings)
    {
        _customerSettings = customerSettings;
        _messageTemplateService = messageTemplateService;
        _customerRepository = customerRepository;
        _genericAttributeRepository = genericAttributeRepository;
        _storeService = storeService;
        _workflowMessageService = workflowMessageService;
        _reminderSettings = reminderSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        if (!_reminderSettings.IncompleteRegistrationEnabled)
            return;

        //ensure the email validation currently enabled after registration
        if (_customerSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
            return;

        foreach (var store in await _storeService.GetAllStoresAsync())
        {

            var messageTemplates = await _messageTemplateService
                .GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE, storeId: store.Id);
            var followUpMessage = messageTemplates.FirstOrDefault(template => template.IsActive && template.DelayBeforeSend is not null);

            if (followUpMessage is null)
                continue;

            var timeToFollowUp = DateTime.UtcNow - TimeSpan.FromHours(followUpMessage.DelayPeriod.ToHours(followUpMessage.DelayBeforeSend.Value));

            //find customers to follow-up about registration activation
            var customersToFollowUp1 = from c in _customerRepository.Table
                                       join incomplteAttr in _genericAttributeRepository.Table on
                                            new { EntityId = c.Id, KeyGroup = nameof(Customer), Key = NopCustomerDefaults.AccountActivationTokenAttribute } equals
                                            new { incomplteAttr.EntityId, incomplteAttr.KeyGroup, incomplteAttr.Key }
                                       where !c.Deleted && !c.Active
                                            && c.RegisteredInStoreId == store.Id
                                            && c.LastRegistrationFollowUpDateUtc == null
                                            && incomplteAttr.CreatedOrUpdatedDateUTC != null && incomplteAttr.CreatedOrUpdatedDateUTC.Value < timeToFollowUp
                                       select c;

            var customersToFollowUp = customersToFollowUp1.ToList();

            if (customersToFollowUp.Count == 0)
                continue;

            var followedUpCustomers = new List<Customer>();
            try
            {
                foreach (var customer in customersToFollowUp)
                {
                    var emailIds = await _workflowMessageService.SendIncompleteRegistrationNotificationMessageAsync(customer);

                    //the follow up has been sent
                    if (emailIds.Any())
                    {
                        customer.LastRegistrationFollowUpNumber = 1;
                        customer.LastRegistrationFollowUpDateUtc = DateTime.UtcNow;
                        followedUpCustomers.Add(customer);
                    }
                }
            }
            finally
            {
                await _customerRepository.UpdateAsync(followedUpCustomers);
            }

        }
    }

    #endregion
}