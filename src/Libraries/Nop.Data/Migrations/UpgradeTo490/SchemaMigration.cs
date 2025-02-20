using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-01-01 00:00:01", "SchemaMigration for 4.90.0")]
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

        //#7294
        var topicTableName = nameof(Topic);
        var topicAvailableEndDateColumnName = nameof(Topic.AvailableEndDateTimeUtc);
        var topicAvailableStartDateColumnName = nameof(Topic.AvailableStartDateTimeUtc);

        if (!Schema.Table(topicTableName).Column(topicAvailableEndDateColumnName).Exists())
        {
            Alter.Table(topicTableName)
                .AddColumn(topicAvailableEndDateColumnName)
                .AsDateTime()
                .Nullable();
        }

        if (!Schema.Table(topicTableName).Column(topicAvailableStartDateColumnName).Exists())
        {
            Alter.Table(topicTableName)
                .AddColumn(topicAvailableStartDateColumnName)
                .AsDateTime()
                .Nullable();
        }

        //#873
        var productTagTableName = nameof(ProductTag);

        if (!Schema.Table(productTagTableName).Column(nameof(ProductTag.MetaDescription)).Exists())
            Alter.Table(productTagTableName).AddColumn(nameof(ProductTag.MetaDescription)).AsString().Nullable();

        if (!Schema.Table(productTagTableName).Column(nameof(ProductTag.MetaKeywords)).Exists())
            Alter.Table(productTagTableName).AddColumn(nameof(ProductTag.MetaKeywords)).AsString(400).Nullable();

        if (!Schema.Table(productTagTableName).Column(nameof(ProductTag.MetaTitle)).Exists())
            Alter.Table(productTagTableName).AddColumn(nameof(ProductTag.MetaTitle)).AsString(400).Nullable();
    }
}
