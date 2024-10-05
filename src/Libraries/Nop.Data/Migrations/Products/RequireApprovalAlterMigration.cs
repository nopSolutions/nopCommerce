using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Products;

[NopUpdateMigration("2024-10-05 11:20:00", "RequiresApproval AlterMigration", UpdateMigrationType.Data)]
public class RequireApprovalAlterMigration : Migration
{
    public override void Up()
    {
        var productTableName = nameof(Product);

        if (!Schema.Table(productTableName).Column(nameof(Product.RequireApproval)).Exists())
            Alter.Table(productTableName)
                .AddColumn(nameof(Product.RequireApproval)).AsBoolean().WithDefaultValue(0).NotNullable();
    }

    public override void Down()
    {
        //add the downgrade logic if necessary 
    }
}
