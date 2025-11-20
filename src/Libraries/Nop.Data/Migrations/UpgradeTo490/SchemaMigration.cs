using FluentMigrator;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Menus;
using Nop.Core.Domain.Topics;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo490;

[NopSchemaMigration("2025-01-02 00:00:01", "SchemaMigration for 4.90.0")]
public class SchemaMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        //#7387
        var ageVerificationColumnName = nameof(Product.AgeVerification);
        if (!Schema.TableFor<Product>().Column(ageVerificationColumnName).Exists())
        {
            Alter.TableFor<Product>()
                .AddColumn(ageVerificationColumnName)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        var minimumAgeToPurchaseColumnName = nameof(Product.MinimumAgeToPurchase);
        if (!Schema.TableFor<Product>().Column(minimumAgeToPurchaseColumnName).Exists())
        {
            Alter.TableFor<Product>()
                .AddColumn(minimumAgeToPurchaseColumnName)
                .AsInt32()
                .NotNullable()
                .WithDefaultValue(0);
        }

        //#7294
        var topicTableName = nameof(Topic);
        var topicAvailableEndDateColumnName = nameof(Topic.AvailableEndDateTimeUtc);
        var topicAvailableStartDateColumnName = nameof(Topic.AvailableStartDateTimeUtc);

        if (!Schema.TableFor<Topic>().Column(topicAvailableEndDateColumnName).Exists())
        {
            Alter.TableFor<Topic>()
                .AddColumn(topicAvailableEndDateColumnName)
                .AsDateTime()
                .Nullable();
        }

        if (!Schema.TableFor<Topic>().Column(topicAvailableStartDateColumnName).Exists())
        {
            Alter.TableFor<Topic>()
                .AddColumn(topicAvailableStartDateColumnName)
                .AsDateTime()
                .Nullable();
        }

        //#873

        if (!Schema.TableFor<ProductTag>().Column(nameof(ProductTag.MetaDescription)).Exists())
            Alter.TableFor<ProductTag>().AddColumn(nameof(ProductTag.MetaDescription)).AsString().Nullable();

        if (!Schema.TableFor<ProductTag>().Column(nameof(ProductTag.MetaKeywords)).Exists())
            Alter.TableFor<ProductTag>().AddColumn(nameof(ProductTag.MetaKeywords)).AsString(400).Nullable();

        if (!Schema.TableFor<ProductTag>().Column(nameof(ProductTag.MetaTitle)).Exists())
            Alter.TableFor<ProductTag>().AddColumn(nameof(ProductTag.MetaTitle)).AsString(400).Nullable();

        //#7390
        if (!Schema.TableFor<Menu>().Exists())
            Create.TableFor<Menu>();

        if (!Schema.TableFor<Menu>().Exists())
            Create.TableFor<MenuItem>();

        var footerColumn1ColumnName = "IncludeInFooterColumn1";
        if (Schema.TableFor<Topic>().Column(footerColumn1ColumnName).Exists())
            Delete.Column(footerColumn1ColumnName).FromTable(topicTableName);

        var footerColumn2ColumnName = "IncludeInFooterColumn2";
        if (Schema.TableFor<Topic>().Column(footerColumn2ColumnName).Exists())
            Delete.Column(footerColumn2ColumnName).FromTable(topicTableName);

        var footerColumn3ColumnName = "IncludeInFooterColumn3";
        if (Schema.TableFor<Topic>().Column(footerColumn3ColumnName).Exists())
            Delete.Column(footerColumn3ColumnName).FromTable(topicTableName);

        var includeTopicInTopMenuColumnName = "IncludeInTopMenu";
        if (Schema.TableFor<Topic>().Column(includeTopicInTopMenuColumnName).Exists())
            Delete.Column(includeTopicInTopMenuColumnName).FromTable(topicTableName);

        var categoryTableName = nameof(Category);
        var includeCategoryInTopMenuColumnName = "IncludeInTopMenu";
        if (Schema.TableFor<Category>().Column(includeCategoryInTopMenuColumnName).Exists())
            Delete.Column(includeCategoryInTopMenuColumnName).FromTable(categoryTableName);
    }
}
