using FluentMigrator;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2024-12-01 00:00:00", "SchemaMigration for 4.90.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#7387
        var productTableName = nameof(Product);

        var ageVerificationColumnName = nameof(Product.AgeVerification);
        if (!Schema.Table(productTableName).Column(ageVerificationColumnName).Exists())
        {
            Alter.Table(productTableName)
                .AddColumn(ageVerificationColumnName)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        var minimumAgeToPurchaseColumnName = nameof(Product.MinimumAgeToPurchase);
        if (!Schema.Table(productTableName).Column(minimumAgeToPurchaseColumnName).Exists())
        {
            Alter.Table(productTableName)
                .AddColumn(minimumAgeToPurchaseColumnName)
                .AsInt32()
                .NotNullable()
                .WithDefaultValue(0);
        }
    }
}
