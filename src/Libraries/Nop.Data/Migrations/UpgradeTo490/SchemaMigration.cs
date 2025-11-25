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
        if (!Schema.ColumnExist<Product>(t => t.AgeVerification))
        {
            Alter.AddColumnFor<Product>(t => t.AgeVerification)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        if (!Schema.ColumnExist<Product>(t => t.MinimumAgeToPurchase))
        {
            Alter.AddColumnFor<Product>(t => t.MinimumAgeToPurchase)
                .AsInt32()
                .NotNullable()
                .WithDefaultValue(0);
        }

        //#7294

        if (!Schema.ColumnExist<Topic>(t => t.AvailableEndDateTimeUtc))
        {
            Alter.AddColumnFor<Topic>(t => t.AvailableEndDateTimeUtc)
                .AsDateTime()
                .Nullable();
        }

        if (!Schema.ColumnExist<Topic>(t => t.AvailableStartDateTimeUtc))
        {
            Alter.AddColumnFor<Topic>(t => t.AvailableStartDateTimeUtc)
                .AsDateTime()
                .Nullable();
        }

        //#873

        if (!Schema.ColumnExist<ProductTag>(t => t.MetaDescription))
        {
            Alter.AddColumnFor<ProductTag>(t => t.MetaDescription)
                .AsString()
                .Nullable();
        }

        if (!Schema.ColumnExist<ProductTag>(t => t.MetaKeywords))
        {
            Alter.AddColumnFor<ProductTag>(t => t.MetaKeywords)
                .AsString(400)
                .Nullable();
        }

        if (!Schema.ColumnExist<ProductTag>(t => t.MetaTitle))
        {
            Alter.AddColumnFor<ProductTag>(t => t.MetaTitle)
                .AsString(400)
                .Nullable();
        }

        //#7390
        if (!Schema.TableExist<Menu>())
            Create.TableFor<Menu>();

        if (!Schema.TableExist<Menu>())
            Create.TableFor<MenuItem>();

        var footerColumn1ColumnName = "IncludeInFooterColumn1";
        if (Schema.ColumnExist<Topic>(footerColumn1ColumnName))
            Delete.Column(footerColumn1ColumnName).FromTable<Topic>();

        var footerColumn2ColumnName = "IncludeInFooterColumn2";
        if (Schema.ColumnExist<Topic>(footerColumn2ColumnName))
            Delete.Column(footerColumn2ColumnName).FromTable<Topic>();

        var footerColumn3ColumnName = "IncludeInFooterColumn3";
        if (Schema.ColumnExist<Topic>(footerColumn3ColumnName))
            Delete.Column(footerColumn3ColumnName).FromTable<Topic>();

        var includeTopicInTopMenuColumnName = "IncludeInTopMenu";
        if (Schema.ColumnExist<Topic>(includeTopicInTopMenuColumnName))
            Delete.Column(includeTopicInTopMenuColumnName).FromTable<Topic>();

        var includeCategoryInTopMenuColumnName = "IncludeInTopMenu";
        if (Schema.ColumnExist<Category>(includeCategoryInTopMenuColumnName))
            Delete.Column(includeCategoryInTopMenuColumnName).FromTable<Category>();
    }
}
