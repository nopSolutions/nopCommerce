using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Companies;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tasks;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.CustomUpdateMigration
{
    [NopMigration("2020-06-10 09:30:17:6453226", "4.40.0", UpdateMigrationType.Data)]
    [SkipMigrationOnInstall]
    public class CustomDataMigration : Migration
    {
        private readonly INopDataProvider _dataProvider;

        public CustomDataMigration(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            
            var productTableName = NameCompatibilityManager.GetTableName(typeof(Product));
            var orderTableName = NameCompatibilityManager.GetTableName(typeof(Order));
            var customerTableName = NameCompatibilityManager.GetTableName(typeof(Customer));
            var companyTableName = NameCompatibilityManager.GetTableName(typeof(Company));
            var scheduleTaskTable = _dataProvider.GetTable<ScheduleTask>();

            if (!scheduleTaskTable.Any(alt => string.Compare(alt.Name, "Remind Me Notification Task", true) == 0))
                _dataProvider.InsertEntity(
                    new ScheduleTask
                    {
                        LastEndUtc = null,
                        LastStartUtc = null,
                        LastSuccessUtc = null,
                        StopOnError = true,
                        Type = "Nop.Services.Common.RemindMeNotificationTask, Nop.Services",
                        Enabled = true,
                        Seconds = 2400,
                        Name = "Remind Me Notification Task"
                    }
                );

            if (!scheduleTaskTable.Any(alt => string.Compare(alt.Name, "Rate Reminder Notification Task", true) == 0))
                _dataProvider.InsertEntity(
                    new ScheduleTask
                    {
                        LastEndUtc = null,
                        LastStartUtc = null,
                        LastSuccessUtc = null,
                        StopOnError = true,
                        Type = "Nop.Services.Common.RateRemainderNotificationTask, Nop.Services",
                        Enabled = true,
                        Seconds = 2400,
                        Name = "Rate Reminder Notification Task"
                    }
                );

            if (scheduleTaskTable.Any(alt => string.Compare(alt.Name, "Remind Me And Rate Reminder Notification Task", true) == 1))
            {
                var deleteScheduleTask = scheduleTaskTable.Where(x => x.Name == "Remind Me And Rate Reminder Notification Task").Any() ? scheduleTaskTable.Where(x => x.Name == "Remind Me And Rate Reminder Notification Task").FirstOrDefault() : null;
                if (deleteScheduleTask != null)
                {
                    _dataProvider.DeleteEntityAsync(deleteScheduleTask);
                }
            }
            var companyEmailColumnName = "Email";
            if (!Schema.Table(companyTableName).Column(companyEmailColumnName).Exists())
            {
                Alter.Table(companyTableName)
                    .AddColumn(companyEmailColumnName).AsString().Nullable().SetExistingRowsTo(null);
            }
            var companyTimeZoneColumnName = "TimeZone";
            if (!Schema.Table(companyTableName).Column(companyTimeZoneColumnName).Exists())
            {
                Alter.Table(companyTableName)
                    .AddColumn(companyTimeZoneColumnName).AsString().WithDefaultValue("Asia/Yerevan");
            }
            
            var ribbonEnableColumnName = "RibbonEnable";
            if (!Schema.Table(productTableName).Column(ribbonEnableColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(ribbonEnableColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var ribbonTextColumnName = "RibbonText";
            if (!Schema.Table(productTableName).Column(ribbonTextColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(ribbonTextColumnName).AsString().Nullable().SetExistingRowsTo(null);
            }

            //order
            var scheduleDateColumnName = "ScheduleDate";
            if (!Schema.Table(orderTableName).Column(scheduleDateColumnName).Exists())
            {
                Alter.Table(orderTableName)
                    .AddColumn(scheduleDateColumnName).AsString().Nullable().SetExistingRowsTo(null);
            }
            
            var ratingColumnName = "Rating";
            if (!Schema.Table(orderTableName).Column(ratingColumnName).Exists())
            {
                Alter.Table(orderTableName)
                    .AddColumn(ratingColumnName).AsInt32().NotNullable().SetExistingRowsTo(0);
            }
            var ratingTextColumnName = "RatingText";
            if (!Schema.Table(orderTableName).Column(ratingTextColumnName).Exists())
            {
                Alter.Table(orderTableName)
                    .AddColumn(ratingTextColumnName).AsString().Nullable().SetExistingRowsTo(null);
            }
            var rateNotificationSendColumnName = "RateNotificationSend";
            if (!Schema.Table(orderTableName).Column(rateNotificationSendColumnName).Exists())
            {
                Alter.Table(orderTableName)
                    .AddColumn(rateNotificationSendColumnName).AsBoolean().Nullable().SetExistingRowsTo(false);
            }

            //customer
            var pushTokenColumnName = "PushToken";
            if (!Schema.Table(customerTableName).Column(pushTokenColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(pushTokenColumnName).AsString().Nullable().SetExistingRowsTo(null);
            }
            var rateReminderNotificationColumnName = "RateReminderNotification";
            if (!Schema.Table(customerTableName).Column(rateReminderNotificationColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(rateReminderNotificationColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
            }
            var remindMeNotificationColumnName = "RemindMeNotification";
            if (!Schema.Table(customerTableName).Column(remindMeNotificationColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(remindMeNotificationColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
            }
            var orderStatusNotificationColumnName = "OrderStatusNotification";
            if (!Schema.Table(customerTableName).Column(orderStatusNotificationColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(orderStatusNotificationColumnName).AsBoolean().NotNullable().SetExistingRowsTo(true);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
