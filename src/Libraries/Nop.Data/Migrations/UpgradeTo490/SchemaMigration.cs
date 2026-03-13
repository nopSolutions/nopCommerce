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
        this.AddOrAlterColumnFor<Product>(t => t.AgeVerification)
            .AsBoolean()
            .NotNullable()
            .WithDefaultValue(false);

        this.AddOrAlterColumnFor<Product>(t => t.MinimumAgeToPurchase)
            .AsInt32()
            .NotNullable()
            .WithDefaultValue(0);

        //#7294
        this.AddOrAlterColumnFor<Topic>(t => t.AvailableEndDateTimeUtc)
            .AsDateTime()
            .Nullable();

        this.AddOrAlterColumnFor<Topic>(t => t.AvailableStartDateTimeUtc)
            .AsDateTime()
            .Nullable();

        //#873
        this.AddOrAlterColumnFor<ProductTag>(t => t.MetaDescription)
            .AsString()
            .Nullable();

        this.AddOrAlterColumnFor<ProductTag>(t => t.MetaKeywords)
            .AsString(400)
            .Nullable();

        this.AddOrAlterColumnFor<ProductTag>(t => t.MetaTitle)
            .AsString(400)
            .Nullable();

        //#7390
        this.CreateTableIfNotExists<Menu>();

        this.CreateTableIfNotExists<MenuItem>();

        this.DeleteColumnsIfExists<Topic>(["IncludeInFooterColumn1", "IncludeInFooterColumn2", "IncludeInFooterColumn3", "IncludeInTopMenu"]);
        this.DeleteColumnsIfExists<Category>(["IncludeInTopMenu"]);
    }
}
