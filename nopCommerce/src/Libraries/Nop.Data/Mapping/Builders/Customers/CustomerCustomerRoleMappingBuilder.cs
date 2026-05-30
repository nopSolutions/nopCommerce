using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Customers;

/// <summary>
/// Represents a customer customer role mapping entity builder
/// </summary>
public partial class CustomerCustomerRoleMappingBuilder : NopEntityBuilder<CustomerCustomerRoleMapping>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerCustomerRoleMapping), nameof(CustomerCustomerRoleMapping.CustomerId)))
            .AsInt32().ForeignKey<Customer>().PrimaryKey()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(CustomerCustomerRoleMapping), nameof(CustomerCustomerRoleMapping.CustomerRoleId)))
            .AsInt32().ForeignKey<CustomerRole>().PrimaryKey();
    }

    #endregion
}