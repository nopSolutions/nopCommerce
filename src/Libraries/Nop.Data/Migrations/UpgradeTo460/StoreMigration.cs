using FluentMigrator;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo460;

[NopSchemaMigration("2022-12-06 15:09:01", "Added new fields to store table")]
public class StoreMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        if (!Schema.ColumnExist<Store>(t => t.Deleted))
            //add new column
            Alter.AddColumnFor<Store>(t => t.Deleted).AsBoolean().WithDefaultValue(false);

        if (!Schema.ColumnExist<Store>(t => t.DefaultTitle))
            //add new column
            Alter.AddColumnFor<Store>(t => t.DefaultTitle).AsString(int.MaxValue).Nullable();
        else
            Alter.AlterColumnFor<Store>(t => t.DefaultTitle).AsString(int.MaxValue).Nullable();

        if (!Schema.ColumnExist<Store>(t => t.DefaultMetaDescription))
            //add new column
            Alter.AddColumnFor<Store>(t => t.DefaultMetaDescription).AsString(int.MaxValue).Nullable();
        else
            Alter.AlterColumnFor<Store>(t => t.DefaultMetaDescription).AsString(int.MaxValue).Nullable();

        if (!Schema.ColumnExist<Store>(t => t.DefaultMetaKeywords))
            //add new column
            Alter.AddColumnFor<Store>(t => t.DefaultMetaKeywords).AsString(int.MaxValue).Nullable();
        else
            Alter.AlterColumnFor<Store>(t => t.DefaultMetaKeywords).AsString(int.MaxValue).Nullable();

        if (!Schema.ColumnExist<Store>(t => t.HomepageDescription))
            //add new column
            Alter.AddColumnFor<Store>(t => t.HomepageDescription).AsString(int.MaxValue).Nullable();
        else
            Alter.AlterColumnFor<Store>(t => t.HomepageDescription).AsString(int.MaxValue).Nullable();

        if (!Schema.ColumnExist<Store>(t => t.HomepageTitle))
            //add new column
            Alter.AddColumnFor<Store>(t => t.HomepageTitle).AsString(int.MaxValue).Nullable();
        else
            Alter.AlterColumnFor<Store>(t => t.HomepageTitle).AsString(int.MaxValue).Nullable();
    }
}