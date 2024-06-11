using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Payments.PayPalCommerce.Domain;

namespace Nop.Plugin.Payments.PayPalCommerce.Data;

/// <summary>
/// Represents the payment token entity builder
/// </summary>
public class PayPalTokenBuilder : NopEntityBuilder<PayPalToken>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PayPalToken.CustomerId)).AsInt32().NotNullable().ForeignKey<Customer>()
            .WithColumn(nameof(PayPalToken.VaultId)).AsString(100).Nullable()
            .WithColumn(nameof(PayPalToken.VaultCustomerId)).AsString(100).Nullable()
            .WithColumn(nameof(PayPalToken.TransactionId)).AsString(100).Nullable()
            .WithColumn(nameof(PayPalToken.Title)).AsString(200).Nullable()
            .WithColumn(nameof(PayPalToken.Expiration)).AsString(100).Nullable()
            .WithColumn(nameof(PayPalToken.Type)).AsString(100).Nullable()
            .WithColumn(nameof(PayPalToken.ClientId)).AsString(200).NotNullable();
    }

    #endregion
}