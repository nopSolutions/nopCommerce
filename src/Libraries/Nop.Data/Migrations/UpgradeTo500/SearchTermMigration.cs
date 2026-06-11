using System.Data;
using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-08-01 00:00:01", "SearchTerm migration")]
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
        _ = _dataProvider.TruncateAsync<SearchTerm>().Result;

        this.DeleteColumnsIfExists<SearchTerm>(["Count"]);

        this.AddOrAlterColumnFor<SearchTerm>(t => t.CreatedOnUtc)
            .AsDateTime2();

        var searchTermTableName = NameCompatibilityManager.GetTableName(typeof(SearchTerm));
        var searchTermCustomerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(SearchTerm), nameof(SearchTerm.CustomerId));

        if (Schema.Table(searchTermTableName).Column(searchTermCustomerIdColumnName).Exists())
        {
            var customerTableName = NameCompatibilityManager.GetTableName(typeof(Customer));
            var customerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(Customer), nameof(BaseEntity.Id));

            Alter.Table(searchTermTableName)
                .AlterColumn(searchTermCustomerIdColumnName)
                .AsInt32()
                .Nullable()
                .ForeignKey(customerTableName, customerIdColumnName).OnDelete(Rule.SetNull);
        }

        this.AddOrAlterColumnFor<SearchTerm>(t => t.Deleted)
            .AsBoolean();
    }
}