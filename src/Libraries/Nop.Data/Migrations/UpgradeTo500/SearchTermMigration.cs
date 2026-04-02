using FluentMigrator;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-03-01 00:00:01", "SearchTerm migration")]
public class SearchTermMigration : ForwardOnlyMigration
{
    private readonly INopDataProvider _dataProvider;

    public SearchTermMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        _dataProvider.TruncateAsync<SearchTerm>();

        this.DeleteColumnsIfExists<SearchTerm>(["Count"]);

        this.AddOrAlterColumnFor<SearchTerm>(t => t.CreatedOnUtc)
            .AsDateTime2();

        this.AddOrAlterColumnFor<SearchTerm>(t => t.CustomerId)
            .AsInt32()
            .ForeignKey<Customer>();

        this.AddOrAlterColumnFor<SearchTerm>(t => t.Deleted)
            .AsBoolean();
    }
}