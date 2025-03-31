using FluentMigrator;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopUpdateMigration("2024-12-25 00:00:00", "4.90", UpdateMigrationType.Data)]
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
        //#3425
        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name, MessageTemplateSystemNames.ORDER_COMPLETED_STORE_OWNER_NOTIFICATION, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");
            _dataProvider.InsertEntity(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.ORDER_COMPLETED_STORE_OWNER_NOTIFICATION,
                Subject = "%Store.Name%. Order #%Order.OrderNumber% completed",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.CustomerFullName% has just completed an order. Below is the summary of the order.{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Billing Address{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingFirstName% %Order.BillingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingCity% %Order.BillingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.BillingStateProvince% %Order.BillingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%if (%Order.Shippable%) Shipping Address{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingFirstName% %Order.ShippingLastName%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress1%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingAddress2%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingCity% %Order.ShippingZipPostalCode%{Environment.NewLine}<br />{Environment.NewLine}%Order.ShippingStateProvince% %Order.ShippingCountry%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Shipping Method: %Order.ShippingMethod%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine} endif% %Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = false, //this template is disabled by default
                EmailAccountId = eaGeneral.Id
            });
        }

        var activityLogTypeTable = _dataProvider.GetTable<ActivityLogType>();

        //#6407
        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "PublicStore.PasswordChanged", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "PublicStore.PasswordChanged",
                    Enabled = true,
                    Name = "Public store. Change password"
                }
            );
        }

        //#6874 Multiple newsletter lists
        var newsLetterSubscriptionTypeTable = _dataProvider.GetTable<NewsLetterSubscriptionType>();
        if (!newsLetterSubscriptionTypeTable.Any(alt => string.Compare(alt.Name, "Newsletter", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var subscriptionType = _dataProvider.InsertEntity(
                new NewsLetterSubscriptionType
                {
                    Name = "Newsletter",
                    TickedByDefault = true,
                    DisplayOrder = 0
                }
            );

            var newsLetterSubscriptions = _dataProvider.GetTable<NewsLetterSubscription>().ToList();
            foreach (var newsLetterSubscription in newsLetterSubscriptions)
            {
                newsLetterSubscription.TypeId = subscriptionType.Id;
            }

            _dataProvider.UpdateEntities(newsLetterSubscriptions);
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "AddSubscriptionType", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "AddSubscriptionType",
                    Enabled = true,
                    Name = "Add a new subscription type"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "DeleteSubscriptionType", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "DeleteSubscriptionType",
                    Enabled = true,
                    Name = "Delete a subscription type"
                }
            );
        }

        if (!activityLogTypeTable.Any(alt => string.Compare(alt.SystemKeyword, "EditSubscriptionType", StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            _dataProvider.InsertEntity(
                new ActivityLogType
                {
                    SystemKeyword = "EditSubscriptionType",
                    Enabled = true,
                    Name = "Edit a subscription type"
                }
            );
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}