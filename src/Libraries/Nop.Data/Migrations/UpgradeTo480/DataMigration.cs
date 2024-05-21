using FluentMigrator;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Migrations.UpgradeTo480;

[NopUpdateMigration("2023-05-21 12:00:00", "4.80", UpdateMigrationType.Data)]
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
        //#7108 New message template
        if (!_dataProvider.GetTable<MessageTemplate>().Any(st => string.Compare(st.Name, MessageTemplateSystemNames.ORDER_CANCELLED_VENDOR_NOTIFICATION, StringComparison.InvariantCultureIgnoreCase) == 0))
        {
            var eaGeneral = _dataProvider.GetTable<EmailAccount>().FirstOrDefault() ?? throw new Exception("Default email account cannot be loaded");
            _dataProvider.InsertEntity(new MessageTemplate()
            {
                Name = MessageTemplateSystemNames.ORDER_CANCELLED_VENDOR_NOTIFICATION,
                Subject = "%Store.Name%. Order #%Order.OrderNumber% cancelled",
                Body = $"<p>{Environment.NewLine}<a href=\"%Store.URL%\">%Store.Name%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order #%Order.OrderNumber% has been cancelled.{Environment.NewLine}<br />{Environment.NewLine}Customer: %Order.CustomerFullName%,{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Order Number: %Order.OrderNumber%{Environment.NewLine}<br />{Environment.NewLine}Date Ordered: %Order.CreatedOn%{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}%Order.Product(s)%{Environment.NewLine}</p>{Environment.NewLine}",
                IsActive = true,
                EmailAccountId = eaGeneral.Id
            });
        }
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}