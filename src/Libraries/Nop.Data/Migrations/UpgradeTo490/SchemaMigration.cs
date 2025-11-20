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
        if (!Schema.TableFor<Product>().ColumnFor<Product>(t => t.AgeVerification).Exists())
        {
            Alter.TableFor<Product>()
                .AddColumnFor<Product>(t => t.AgeVerification)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        if (!Schema.TableFor<Product>().ColumnFor<Product>(t => t.MinimumAgeToPurchase).Exists())
        {
            Alter.TableFor<Product>()
                .AddColumnFor<Product>(t => t.MinimumAgeToPurchase)
                .AsInt32()
                .NotNullable()
                .WithDefaultValue(0);
        }

        //#7294

        if (!Schema.TableFor<Topic>().ColumnFor<Topic>(t => t.AvailableEndDateTimeUtc).Exists())
        {
            Alter.TableFor<Topic>()
                .AddColumnFor<Topic>(t => t.AvailableEndDateTimeUtc)
                .AsDateTime()
                .Nullable();
        }

        if (!Schema.TableFor<Topic>().ColumnFor<Topic>(t => t.AvailableStartDateTimeUtc).Exists())
        {
            Alter.TableFor<Topic>()
                .AddColumnFor<Topic>(t => t.AvailableStartDateTimeUtc)
                .AsDateTime()
                .Nullable();
        }

        //#873

        if (!Schema.TableFor<ProductTag>().ColumnFor<ProductTag>(t => t.MetaDescription).Exists())
        {
            Alter.TableFor<ProductTag>()
                .AddColumnFor<ProductTag>(t => t.MetaDescription)
                .AsString()
                .Nullable();
        }

        if (!Schema.TableFor<ProductTag>().ColumnFor<ProductTag>(t => t.MetaKeywords).Exists())
        {
            Alter.TableFor<ProductTag>()
                .AddColumnFor<ProductTag>(t => t.MetaKeywords)
                .AsString(400)
                .Nullable();
        }

        if (!Schema.TableFor<ProductTag>().ColumnFor<ProductTag>(t => t.MetaTitle).Exists())
        {
            Alter.TableFor<ProductTag>()
                .AddColumnFor<ProductTag>(t => t.MetaTitle)
                .AsString(400)
                .Nullable();
        }

        //#7390
        if (!Schema.TableFor<Menu>().Exists())
            Create.TableFor<Menu>();

        if (!Schema.TableFor<Menu>().Exists())
            Create.TableFor<MenuItem>();

        var footerColumn1ColumnName = "IncludeInFooterColumn1";
        if (Schema.TableFor<Topic>().Column(footerColumn1ColumnName).Exists())
            Delete.Column(footerColumn1ColumnName).FromTable<Topic>();

        var footerColumn2ColumnName = "IncludeInFooterColumn2";
        if (Schema.TableFor<Topic>().Column(footerColumn2ColumnName).Exists())
            Delete.Column(footerColumn2ColumnName).FromTable<Topic>();

        var footerColumn3ColumnName = "IncludeInFooterColumn3";
        if (Schema.TableFor<Topic>().Column(footerColumn3ColumnName).Exists())
            Delete.Column(footerColumn3ColumnName).FromTable<Topic>();

        var includeTopicInTopMenuColumnName = "IncludeInTopMenu";
        if (Schema.TableFor<Topic>().Column(includeTopicInTopMenuColumnName).Exists())
            Delete.Column(includeTopicInTopMenuColumnName).FromTable<Topic>();

        var includeCategoryInTopMenuColumnName = "IncludeInTopMenu";
        if (Schema.TableFor<Category>().Column(includeCategoryInTopMenuColumnName).Exists())
            Delete.Column(includeCategoryInTopMenuColumnName).FromTable<Category>();
    }
}
