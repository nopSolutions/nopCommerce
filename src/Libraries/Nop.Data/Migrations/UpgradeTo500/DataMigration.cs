using FluentMigrator;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.ScheduleTasks;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-04-20 00:00:00", "5.00", UpdateMigrationType.Data)]
public class DataMigration : Migration
{
    private readonly INopDataProvider _dataProvider;

    public DataMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#8120
        if (!_dataProvider.GetTable<ScheduleTask>().Any(st => string.Compare(st.Type, "Nop.Services.Orders.AutoCancelOrdersTask, Nop.Services", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(new ScheduleTask()
            {
                Name = "Auto-cancel unpaid orders",
                //60 minutes
                Seconds = 3600,
                Type = "Nop.Services.Orders.AutoCancelOrdersTask, Nop.Services",
                Enabled = true,
                LastEnabledUtc = DateTime.UtcNow,
                StopOnError = false
            });
        }

        var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

        //#1832
        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "AddNewContactFormAttribute", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "AddNewContactFormAttribute",
                    Enabled = true,
                    Name = "Add a new contact form attribute"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "EditContactFormAttribute", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "EditContactFormAttribute",
                    Enabled = true,
                    Name = "Edit a contact form attribute"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "DeleteContactFormAttribute", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "DeleteContactFormAttribute",
                    Enabled = true,
                    Name = "Delete a contact form attribute"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "AddNewContactFormAttributeValue", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "AddNewContactFormAttributeValue",
                    Enabled = true,
                    Name = "Add a new contact form attribute value"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "EditContactFormAttributeValue", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "EditContactFormAttributeValue",
                    Enabled = true,
                    Name = "Edit a contact form attribute value"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "DeleteContactFormAttributeValue", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "DeleteContactFormAttributeValue",
                    Enabled = true,
                    Name = "Delete a contact form attribute value"
                }
            );
        }

        //#8098
        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "AddNewPriceList", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "AddNewPriceList",
                    Enabled = true,
                    Name = "Add a new price list"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "DeletePriceList", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "DeletePriceList",
                    Enabled = true,
                    Name = "Delete a price list"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "EditPriceList", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "EditPriceList",
                    Enabled = true,
                    Name = "Edit a price list"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ExportPriceLists", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "ExportPriceLists",
                    Enabled = true,
                    Name = "Export price lists"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "ImportPriceLists", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "ImportPriceLists",
                    Enabled = true,
                    Name = "Import price lists"
                }
            );
        }

        //#8161
        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name, MessageTemplateSystemNames.RETURN_REQUEST_WITHDRAWAL_LINK_MESSAGE, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");
            _dataProvider.InsertEntity(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.RETURN_REQUEST_WITHDRAWAL_LINK_MESSAGE,
                Subject = "%Store.Name%. Confirm your withdrawal request.",
                Body = $"<p>We have received your withdrawal request.{Environment.NewLine}Click the <a href=\"%ReturnRequest.WithdrawalUrl%\">link</a> to confirm the request.{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }

        //#309
        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name, MessageTemplateSystemNames.NEXT_RECURRING_PAYMENT_CUSTOMER_NOTIFICATION, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");
            _dataProvider.InsertEntity(new MessageTemplate
            {
                Name = MessageTemplateSystemNames.NEXT_RECURRING_PAYMENT_CUSTOMER_NOTIFICATION,
                Subject = "%Store.Name%. Notification of upcoming payment",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Hello %Customer.FullName%,{Environment.NewLine}<br />{Environment.NewLine}The next payment for order <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderNumber%</a> will be debited in %RecurringPayment.NextRecurringPaymentDelay% day(s).{Environment.NewLine}<br />{Environment.NewLine}Please make sure you have sufficient funds on your card for the upcoming debit.</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }
    }

    /// <summary>Collects the DOWN migration expressions</summary>
    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}