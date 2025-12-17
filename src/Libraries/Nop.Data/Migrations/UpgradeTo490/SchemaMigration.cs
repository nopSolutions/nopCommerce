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
        if (!Schema.ColumnExists<Product>(t => t.AgeVerification))
        {
            Alter.AddColumnFor<Product>(t => t.AgeVerification)
                .AsBoolean()
                .NotNullable()
                .WithDefaultValue(false);
        }

        if (!Schema.ColumnExists<Product>(t => t.MinimumAgeToPurchase))
        {
            Alter.AddColumnFor<Product>(t => t.MinimumAgeToPurchase)
                .AsInt32()
                .NotNullable()
                .WithDefaultValue(0);
        }

        //#7294

        if (!Schema.ColumnExists<Topic>(t => t.AvailableEndDateTimeUtc))
        {
            Alter.AddColumnFor<Topic>(t => t.AvailableEndDateTimeUtc)
                .AsDateTime()
                .Nullable();
        }

        if (!Schema.ColumnExists<Topic>(t => t.AvailableStartDateTimeUtc))
        {
            Alter.AddColumnFor<Topic>(t => t.AvailableStartDateTimeUtc)
                .AsDateTime()
                .Nullable();
        }

        //#873

        if (!Schema.ColumnExists<ProductTag>(t => t.MetaDescription))
        {
            Alter.AddColumnFor<ProductTag>(t => t.MetaDescription)
                .AsString()
                .Nullable();
        }

        if (!Schema.ColumnExists<ProductTag>(t => t.MetaKeywords))
        {
            Alter.AddColumnFor<ProductTag>(t => t.MetaKeywords)
                .AsString(400)
                .Nullable();
        }

        if (!Schema.ColumnExists<ProductTag>(t => t.MetaTitle))
        {
            Alter.AddColumnFor<ProductTag>(t => t.MetaTitle)
                .AsString(400)
                .Nullable();
        }

        //#7390
        if (!Schema.TableExists<Menu>())
            Create.TableFor<Menu>();

        if (!Schema.TableExists<Menu>())
            Create.TableFor<MenuItem>();

        var footerColumn1ColumnName = "IncludeInFooterColumn1";
        if (Schema.ColumnExists<Topic>(footerColumn1ColumnName))
            Delete.Column<Topic>(footerColumn1ColumnName);

        var footerColumn2ColumnName = "IncludeInFooterColumn2";
        if (Schema.ColumnExists<Topic>(footerColumn2ColumnName))
            Delete.Column<Topic>(footerColumn2ColumnName);

        var footerColumn3ColumnName = "IncludeInFooterColumn3";
        if (Schema.ColumnExists<Topic>(footerColumn3ColumnName))
            Delete.Column<Topic>(footerColumn3ColumnName);

        var includeTopicInTopMenuColumnName = "IncludeInTopMenu";
        if (Schema.ColumnExists<Topic>(includeTopicInTopMenuColumnName))
            Delete.Column(includeTopicInTopMenuColumnName);

        var includeCategoryInTopMenuColumnName = "IncludeInTopMenu";
        if (Schema.ColumnExists<Category>(includeCategoryInTopMenuColumnName))
            Delete.Column<Category>(includeCategoryInTopMenuColumnName);
    }
}
