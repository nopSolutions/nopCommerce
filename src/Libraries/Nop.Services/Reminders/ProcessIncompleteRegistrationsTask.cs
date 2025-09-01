using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Reminders;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Reminders;

/// <summary>
/// Represents a task to process incomplete registrations 
/// </summary>
public partial class ProcessIncompleteRegistrationsTask : IScheduleTask
{
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IMessageTemplateService _messageTemplateService;
    protected readonly IRepository<Customer> _customerRepository;
    protected readonly IRepository<GenericAttribute> _genericAttributeRepository;
    protected readonly IWorkflowMessageService _workflowMessageService;
    protected readonly ReminderSettings _reminderSettings;

    #endregion

    #region Ctor

    public ProcessIncompleteRegistrationsTask(CustomerSettings customerSettings,
        IGenericAttributeService genericAttributeService,
        IMessageTemplateService messageTemplateService,
        IRepository<Customer> customerRepository,
        IRepository<GenericAttribute> genericAttributeRepository,
        IWorkflowMessageService workflowMessageService,
        ReminderSettings reminderSettings)
    {
        _customerSettings = customerSettings;
        _genericAttributeService = genericAttributeService;
        _messageTemplateService = messageTemplateService;
        _customerRepository = customerRepository;
        _genericAttributeRepository = genericAttributeRepository;
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

        var messageTemplates = await _messageTemplateService
            .GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE);
        var followUpMessage = messageTemplates.FirstOrDefault(template => template.IsActive && template.DelayBeforeSend is not null);

        if (followUpMessage is null)
            return;

        var attributeName = NopReminderDefaults.IncompleteRegistrations.FollowUpAttributeName;
        var timeToFollowUp = DateTime.UtcNow - TimeSpan.FromHours(followUpMessage.DelayPeriod.ToHours(followUpMessage.DelayBeforeSend.Value));

        //find customers to follow-up about registration activation
        var customersToFollowUp = from c in _customerRepository.Table
                                  join incomplteAttr in _genericAttributeRepository.Table on
                                       new { EntityId = c.Id, KeyGroup = nameof(Customer), Key = NopCustomerDefaults.AccountActivationTokenAttribute } equals
                                       new { incomplteAttr.EntityId, incomplteAttr.KeyGroup, incomplteAttr.Key }
                                  join attribute in _genericAttributeRepository.Table on
                                       new { EntityId = c.Id, KeyGroup = nameof(Customer), Key = attributeName } equals
                                       new { attribute.EntityId, attribute.KeyGroup, attribute.Key } into attribute
                                  from followUpAttr in attribute.DefaultIfEmpty()
                                  where !c.Deleted && !c.Active
                                       && incomplteAttr.CreatedOrUpdatedDateUTC != null && incomplteAttr.CreatedOrUpdatedDateUTC.Value < timeToFollowUp
                                       && followUpAttr == null
                                  select c;

        foreach (var customerToFollowUp in customersToFollowUp.ToList())
        {
            var emailIds = await _workflowMessageService.SendIncompleteRegistrationNotificationMessageAsync(customerToFollowUp);

            //the follow up has been sent
            if (emailIds.Any())
                await _genericAttributeService.SaveAttributeAsync(customerToFollowUp, attributeName, DateTime.UtcNow, customerToFollowUp.RegisteredInStoreId);
        }
    }

    #endregion
}