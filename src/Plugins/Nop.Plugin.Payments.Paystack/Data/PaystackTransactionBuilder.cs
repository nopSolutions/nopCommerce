using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Payments.Paystack.Models;

namespace Nop.Plugin.Payments.Paystack.Data;

/// <summary>
/// Represents a Paystack transaction entity builder (database mapping)
/// </summary>
public class PaystackTransactionBuilder : NopEntityBuilder<PaystackTransactionModel>
{
    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(PaystackTransactionModel.Reference)).AsString(100).NotNullable()
            .WithColumn(nameof(PaystackTransactionModel.CustomerEmail)).AsString(500).NotNullable()
            .WithColumn(nameof(PaystackTransactionModel.Amount)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(PaystackTransactionModel.Currency)).AsString(5).NotNullable()
            .WithColumn(nameof(PaystackTransactionModel.Status)).AsString(50).NotNullable()
            .WithColumn(nameof(PaystackTransactionModel.CreatedAt)).AsDateTime().NotNullable()
            .WithColumn(nameof(PaystackTransactionModel.UpdatedAt)).AsDateTime().Nullable()
            .WithColumn(nameof(PaystackTransactionModel.ErrorMessage)).AsString(int.MaxValue).Nullable()
            .WithColumn(nameof(PaystackTransactionModel.OrderId)).AsInt32().NotNullable();
    }
}
