using FluentMigrator;
using Nop.Core.Domain.PriceLists;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-03-23 00:00:00", "Price lists")]
public class PriceListMigration : ForwardOnlyMigration
{
    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        this.CreateTableIfNotExists<PriceList>();
        this.CreateTableIfNotExists<PriceListItem>();
        this.CreateTableIfNotExists<PriceListCustomer>();
        this.CreateTableIfNotExists<PriceListCustomerRole>();
    }
}
