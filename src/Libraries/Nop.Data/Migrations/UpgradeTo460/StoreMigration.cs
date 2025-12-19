using FluentMigrator;
using Nop.Core.Domain.Stores;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo460;

[NopSchemaMigration("2022-12-06 15:09:01", "Added new fields to store table")]
public class StoreMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        this.AddOrAlterColumnFor<Store>(t => t.Deleted)
            .AsBoolean()
            .WithDefaultValue(false);

        this.AddOrAlterColumnFor<Store>(t => t.DefaultTitle)
            .AsString(int.MaxValue)
            .Nullable();

        this.AddOrAlterColumnFor<Store>(t => t.DefaultMetaDescription)
            .AsString(int.MaxValue)
            .Nullable();

        this.AddOrAlterColumnFor<Store>(t => t.DefaultMetaKeywords)
            .AsString(int.MaxValue)
            .Nullable();

        this.AddOrAlterColumnFor<Store>(t => t.HomepageDescription)
            .AsString(int.MaxValue)
            .Nullable();

        this.AddOrAlterColumnFor<Store>(t => t.HomepageTitle)
            .AsString(int.MaxValue)
            .Nullable();

    }
}