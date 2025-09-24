using FluentMigrator;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Reminders;

namespace Nop.Web.Framework.Migrations.UpgradeTo490;

[NopMigration("2025-09-12 12:00:00", "Reminders migration", MigrationProcessType.Update)]
public class RemindersMigration : Migration
{
    #region Fields

    private readonly IRepository<EmailAccount> _emailAccountRepository;
    private readonly IRepository<MessageTemplate> _messageTemplateRepository;
    private readonly IRepository<ScheduleTask> _scheduleTaskRepository;

    #endregion

    #region Ctor

    public RemindersMigration(IRepository<EmailAccount> emailAccountRepository, IRepository<MessageTemplate> messageTemplateRepository, IRepository<ScheduleTask> scheduleTaskRepository)
    {
        _emailAccountRepository = emailAccountRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _scheduleTaskRepository = scheduleTaskRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        var eaGeneral = _emailAccountRepository.Table.FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");

        #region Abandoned cart

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_1_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_1_MESSAGE,
                Subject = "Dear %Customer.FirstName%, you left some items in your cart.",
                Body = $"<p>Hi %Customer.FirstName%,</p>{Environment.NewLine}<p>We noticed you left an item in your cart and this is a friendly reminder to complete your purchase.</p>{Environment.NewLine}<p>Your shopping cart currently contains the following items:</p>{Environment.NewLine}%Customer.Cart%{Environment.NewLine}<p>Please visit your <a href=\"%Customer.ShoppingCartUrl%\">shopping cart</a> to complete your order</p>",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 2,
                DelayPeriod = MessageDelayPeriod.Hours,
            });
        }

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_2_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_2_MESSAGE,
                Subject = "Dear %Customer.FirstName%, was there a problem? What can we help you with?",
                Body = $"<p>Hi %Customer.FirstName%,</p>{Environment.NewLine}<p>We noticed you left something at checkout:</p>{Environment.NewLine}%Customer.Cart%{Environment.NewLine}<p>Please visit your <a href=\"%Customer.ShoppingCartUrl%\">shopping cart</a> to complete your order</p>{Environment.NewLine}<p>Was there a problem or any questions? Please reply to this email and we will help you.</p>",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 1,
                DelayPeriod = MessageDelayPeriod.Days,
            });
        }

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_3_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_ABANDONED_CART_FOLLOW_UP_3_MESSAGE,
                Subject = "Dear %Customer.FirstName%, you left some items in your cart.",
                Body = $"<p>Hi %Customer.FirstName%,</p>{Environment.NewLine}<p>We noticed you left an item in your cart and this is a friendly reminder to complete your purchase.</p>{Environment.NewLine}<p>Your shopping cart currently contains the following items:</p>{Environment.NewLine}%Customer.Cart%{Environment.NewLine}<p>Please visit your <a href=\"%Customer.ShoppingCartUrl%\">shopping cart</a> to complete your order</p>",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 5,
                DelayPeriod = MessageDelayPeriod.Days,
            });
        }

        if (!_scheduleTaskRepository.Table.Any(st => string.Compare(st.Type, NopReminderDefaults.AbandonedCarts.ProcessTaskTypeFullName, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _scheduleTaskRepository.Insert(
                new ScheduleTask
                {
                    Name = "Process abandoned carts",
                    Seconds = 20 * 60,
                    Type = NopReminderDefaults.AbandonedCarts.ProcessTaskTypeFullName,
                    Enabled = false,
                    StopOnError = false
                }
            );
        }

        #endregion

        #region Pending order

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_1_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_1_MESSAGE,
                Subject = "You haven’t completed the order",
                Body = $"<h1>You haven’t completed the order</h1>{Environment.NewLine}<p>Dear %Order.CustomerFullName%,</p>{Environment.NewLine}<p>We noticed that you haven’t completed the payment for your order on <a href=\"%Store.URL%\">%Store.Name%</a></p>{Environment.NewLine}<p>Below is the summary of the order:</p>{Environment.NewLine}<p></p>{Environment.NewLine}<p>Name: %Order.CustomerFullName% (%Order.CustomerEmail%)</p>{Environment.NewLine}<p>Order Number: %Order.OrderNumber%</p>{Environment.NewLine}<p>Date Ordered: %Order.CreatedOn%</p>{Environment.NewLine}<p>Product(s):</p>{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}<p>To complete your order:</p>{Environment.NewLine}<p>Go to our website and place a new order.</p>",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 3,
                DelayPeriod = MessageDelayPeriod.Days
            });
        }

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_2_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_PENDING_ORDER_FOLLOW_UP_2_MESSAGE,
                Subject = "The payment has not been completed",
                Body = $"<h1>You haven’t completed the order</h1>{Environment.NewLine}<p>Dear %Order.CustomerFullName%,</p>{Environment.NewLine}<p>We noticed that you haven’t completed the payment for your order on <a href=\"%Store.URL%\">%Store.Name%</a></p>{Environment.NewLine}<p>Below is the summary of the order:</p>{Environment.NewLine}<p></p>{Environment.NewLine}<p>Name: %Order.CustomerFullName% (%Order.CustomerEmail%)</p>{Environment.NewLine}<p>Order Number: %Order.OrderNumber%</p>{Environment.NewLine}<p>Date Ordered: %Order.CreatedOn%</p>{Environment.NewLine}<p>Product(s):</p>{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}<p>To complete your order:</p>{Environment.NewLine}<p>Go to your order details <a href=\"%Order.OrderURLForCustomer%\">here</a> and click the \"Retry payment\" button.</p>",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 10,
                DelayPeriod = MessageDelayPeriod.Days
            });
        }

        if (!_scheduleTaskRepository.Table.Any(st => string.Compare(st.Type, NopReminderDefaults.PendingOrders.ProcessTaskTypeFullName, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _scheduleTaskRepository.Insert(
                new ScheduleTask
                {
                    Name = "Process incomplete orders",
                    Seconds = 60 * 60,
                    Type = NopReminderDefaults.PendingOrders.ProcessTaskTypeFullName,
                    Enabled = false,
                    StopOnError = false
                }
            );
        }

        #endregion

        #region Incomplete registration

        if (!_messageTemplateRepository.Table.Any(st => string.Compare(st.Name, MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _messageTemplateRepository.Insert(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.REMINDER_REGISTRATION_FOLLOW_UP_MESSAGE,
                Subject = "Registration at %Store.Name%.",
                Body = $"<h1>Confirm your email</h1>{Environment.NewLine}<p>You’re receiving this message because you recently signed up on our website. Please confirm your email address by clicking the link below:</p>{Environment.NewLine}<p><a href=\"%Customer.AccountActivationURL%\">%Customer.AccountActivationURL%</a></p>{Environment.NewLine}<p>This step adds extra security to your business by verifying you own this email.</p>{Environment.NewLine}<p>Thank You!</p>",
                IsActive = true,
                EmailAccountId = eaGeneral.Id,
                DelayBeforeSend = 1,
                DelayPeriod = MessageDelayPeriod.Days
            });
        }

        if (!_scheduleTaskRepository.Table.Any(st => string.Compare(st.Type, NopReminderDefaults.IncompleteRegistrations.ProcessTaskTypeFullName, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _scheduleTaskRepository.Insert(
                new ScheduleTask
                {
                    Name = "Process incomplete registrations",
                    Seconds = 60 * 60,
                    Type = NopReminderDefaults.IncompleteRegistrations.ProcessTaskTypeFullName,
                    Enabled = false,
                    StopOnError = false
                }
            );
        }

        #endregion
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }

    #endregion
}