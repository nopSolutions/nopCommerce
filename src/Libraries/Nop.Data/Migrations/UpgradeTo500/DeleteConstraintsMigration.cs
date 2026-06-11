using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Data.Mapping;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopSchemaMigration("2026-01-01 00:00:00", "Delete constraints migration")]
public class DeleteConstraintsMigration : ForwardOnlyMigration
{
    private readonly INopDataProvider _dataProvider;

    public DeleteConstraintsMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        var customerTableName = NameCompatibilityManager.GetTableName(typeof(Customer));
        var customerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(Customer), nameof(BaseEntity.Id));

        var searchTermTableName = NameCompatibilityManager.GetTableName(typeof(SearchTerm));
        var searchTermCustomerIdColumnName = NameCompatibilityManager.GetColumnName(typeof(SearchTerm), nameof(SearchTerm.CustomerId));

        if (Schema.Table(searchTermTableName).Column(searchTermCustomerIdColumnName).Exists())
        {
            var constraintName = _dataProvider
                .CreateForeignKeyName(searchTermTableName, searchTermCustomerIdColumnName, customerTableName, customerIdColumnName);

            if (Schema.Table(searchTermTableName).Constraint(constraintName).Exists())
                Delete.UniqueConstraint(constraintName).FromTable(searchTermTableName);
        }
    }
}