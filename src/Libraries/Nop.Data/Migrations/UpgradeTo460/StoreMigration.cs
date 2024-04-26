using FluentMigrator;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Migrations.UpgradeTo460;

[NopSchemaMigration("2022-12-06 15:09:01", "Added new fields to store table")]
public class StoreMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        if (!Schema.Table(nameof(Store)).Column(nameof(Store.Deleted)).Exists())
        {
            //add new column
            Alter.Table(nameof(Store))
                .AddColumn(nameof(Store.Deleted)).AsBoolean().WithDefaultValue(false);

            Alter.Table(nameof(Store))
                .AlterColumn(nameof(Store.Deleted)).AsBoolean();
        }

        if (!Schema.Table(nameof(Store)).Column(nameof(Store.DefaultTitle)).Exists())
            //add new column
            Alter.Table(nameof(Store))
                .AddColumn(nameof(Store.DefaultTitle)).AsString(int.MaxValue).Nullable();
        else
            Alter.Table(nameof(Store)).AlterColumn(nameof(Store.DefaultTitle)).AsString(int.MaxValue).Nullable();

        if (!Schema.Table(nameof(Store)).Column(nameof(Store.DefaultMetaDescription)).Exists())
            //add new column
            Alter.Table(nameof(Store))
                .AddColumn(nameof(Store.DefaultMetaDescription)).AsString(int.MaxValue).Nullable();
        else
            Alter.Table(nameof(Store)).AlterColumn(nameof(Store.DefaultMetaDescription)).AsString(int.MaxValue).Nullable();

        if (!Schema.Table(nameof(Store)).Column(nameof(Store.DefaultMetaKeywords)).Exists())
            //add new column
            Alter.Table(nameof(Store))
                .AddColumn(nameof(Store.DefaultMetaKeywords)).AsString(int.MaxValue).Nullable();
        else
            Alter.Table(nameof(Store)).AlterColumn(nameof(Store.DefaultMetaKeywords)).AsString(int.MaxValue).Nullable();

        if (!Schema.Table(nameof(Store)).Column(nameof(Store.HomepageDescription)).Exists())
            //add new column
            Alter.Table(nameof(Store))
                .AddColumn(nameof(Store.HomepageDescription)).AsString(int.MaxValue).Nullable();
        else
            Alter.Table(nameof(Store)).AlterColumn(nameof(Store.HomepageDescription)).AsString(int.MaxValue).Nullable();

        if (!Schema.Table(nameof(Store)).Column(nameof(Store.HomepageTitle)).Exists())
            //add new column
            Alter.Table(nameof(Store))
                .AddColumn(nameof(Store.HomepageTitle)).AsString(int.MaxValue).Nullable();
        else
            Alter.Table(nameof(Store)).AlterColumn(nameof(Store.HomepageTitle)).AsString(int.MaxValue).Nullable();
    }
}