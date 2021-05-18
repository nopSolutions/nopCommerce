using System.Linq;
using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.CustomUpdateMigration
{
    [NopMigration("2020-06-10 09:30:17:6455448", "4.40.0", UpdateMigrationType.Data)]
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

            //custom migrtion code
            var vegetarianColumnName = "Vegetarian";
            if (!Schema.Table(productTableName).Column(vegetarianColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(vegetarianColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var veganColumnName = "Vegan";
            if (!Schema.Table(productTableName).Column(veganColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(veganColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var gluttenFreeColumnName = "GluttenFree";
            if (!Schema.Table(productTableName).Column(gluttenFreeColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(gluttenFreeColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var halalColumnName = "Halal";
            if (!Schema.Table(productTableName).Column(halalColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(halalColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var allergyFriendlyColumnName = "AllergyFriendly";
            if (!Schema.Table(productTableName).Column(allergyFriendlyColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(allergyFriendlyColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var wellPackedColumnName = "WellPacked";
            if (!Schema.Table(productTableName).Column(wellPackedColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(wellPackedColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var sustainablePackagingColumnName = "SustainablePackaging";
            if (!Schema.Table(productTableName).Column(sustainablePackagingColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(sustainablePackagingColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var fastAndReliableColumnName = "FastAndReliable";
            if (!Schema.Table(productTableName).Column(fastAndReliableColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(fastAndReliableColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var excellentValueColumnName = "ExcellentValue";
            if (!Schema.Table(productTableName).Column(excellentValueColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(excellentValueColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var followOrderNotesColumnName = "FollowOrderNotes";
            if (!Schema.Table(productTableName).Column(followOrderNotesColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(followOrderNotesColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var minimumColumnName = "Minimum";
            if (!Schema.Table(productTableName).Column(minimumColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(minimumColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var averageColumnName = "Average";
            if (!Schema.Table(productTableName).Column(averageColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(averageColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }

            var expensiveColumnName = "Expensive";
            if (!Schema.Table(productTableName).Column(expensiveColumnName).Exists())
            {
                Alter.Table(productTableName)
                    .AddColumn(expensiveColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
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

            //customer
            var pushTokenColumnName = "PushToken";
            if (!Schema.Table(customerTableName).Column(pushTokenColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(pushTokenColumnName).AsString().Nullable().SetExistingRowsTo(null);
            }
            var offersColumnName = "Offers";
            if (!Schema.Table(customerTableName).Column(offersColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(offersColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
            var rewardsColumnName = "Rewards";
            if (!Schema.Table(customerTableName).Column(rewardsColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(rewardsColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
            var eatsPassColumnName = "EatsPass";
            if (!Schema.Table(customerTableName).Column(eatsPassColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(eatsPassColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
            var otherColumnName = "Other";
            if (!Schema.Table(customerTableName).Column(otherColumnName).Exists())
            {
                Alter.Table(customerTableName)
                    .AddColumn(otherColumnName).AsBoolean().NotNullable().SetExistingRowsTo(false);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
