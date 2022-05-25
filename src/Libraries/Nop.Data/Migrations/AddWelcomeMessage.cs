using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations;


[NopMigration("2022/05/25 12:00:00", "Category. Add some new property", MigrationProcessType.Update)]
public class AddWelcomeMessage: MigrationBase
{
    /// <summary>Collect the UP migration expressions</summary>
    public override void Up()
    {
        if (!Schema.Table(NameCompatibilityManager.GetTableName(typeof(Customer))).Column(nameof(Customer.WelcomeMessage)).Exists())
        {
            //add new column
            Alter.Table(NameCompatibilityManager.GetTableName(typeof(Customer)))
                .AddColumn(nameof(Customer.WelcomeMessage))
                .AsString(255)
                .Nullable();
        }
    }

    public override void Down()
    {
        
    }
}