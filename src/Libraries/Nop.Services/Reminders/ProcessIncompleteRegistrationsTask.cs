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
    protected readonly RemindersSettings _remindersSettings;

    #endregion

    #region Ctor

    public ProcessIncompleteRegistrationsTask(
        CustomerSettings customerSettings,
        IGenericAttributeService genericAttributeService,
        IMessageTemplateService messageTemplateService,
        IRepository<Customer> customerRepository,
        IRepository<GenericAttribute> genericAttributeRepository,
        IWorkflowMessageService workflowMessageService,
        RemindersSettings remindersSettings)
    {
        _customerSettings = customerSettings;
        _genericAttributeService = genericAttributeService;
        _messageTemplateService = messageTemplateService;
        _customerRepository = customerRepository;
        _genericAttributeRepository = genericAttributeRepository;
        _workflowMessageService = workflowMessageService;
        _remindersSettings = remindersSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes a task
    /// </summary>
    public virtual async Task ExecuteAsync()
    {
        if (!_remindersSettings.IncompleteRegistrationEnabled)
            return;

        //ensure the email validation currently enabled after registration
        if (_customerSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
            return;

        var followUpMessage = (await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE))
            .FirstOrDefault();

        if (followUpMessage is null || !followUpMessage.IsActive)
            return;

        if (followUpMessage.DelayBeforeSend is null)
            return;

        var timeToFollowUp = DateTime.UtcNow - TimeSpan.FromHours(followUpMessage.DelayPeriod.ToHours(followUpMessage.DelayBeforeSend.Value));

        //find customers to follow-up about registration activation
        var customersToFollowUp = _genericAttributeRepository.Table
            .Join(_customerRepository.Table,
                attribute => new { CustomerId = attribute.EntityId, attribute.KeyGroup },
                customer => new { CustomerId = customer.Id, KeyGroup = nameof(Customer) },
                (attribute, customer) => new { Customer = customer, attribute.Key, attribute.CreatedOrUpdatedDateUTC })
            .Where(customerAttribute => !customerAttribute.Customer.Deleted && !customerAttribute.Customer.Active)
            .Where(customerAttribute => 
                !_genericAttributeRepository.Table.Any(ga => ga.KeyGroup == nameof(Customer)
                && ga.EntityId == customerAttribute.Customer.Id
                && ga.Key == RemindersDefaults.IncompleteRegistrations.FollowUpAttributeName))
            .Where(customerAttribute => customerAttribute.Key == NopCustomerDefaults.AccountActivationTokenAttribute)
            .Where(customerAttribute => customerAttribute.CreatedOrUpdatedDateUTC != null && customerAttribute.CreatedOrUpdatedDateUTC.Value < timeToFollowUp)
            .Select(customerAttribute => customerAttribute.Customer)
            .ToList();

        foreach (var customerToFollowUp in customersToFollowUp)
        {
            var emailIds = await _workflowMessageService.SendIncompleteRegistrationNotificationMessageAsync(customerToFollowUp);

            //the follow up has been sent
            if (emailIds.Any())
                await _genericAttributeService.SaveAttributeAsync(customerToFollowUp, RemindersDefaults.IncompleteRegistrations.FollowUpAttributeName, DateTime.UtcNow);
        }
    }

    #endregion
}
