using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Payments.Momo.Models;

namespace Nop.Plugin.Payments.Momo.Data;

/// <summary>
/// Represents a MoMo transaction entity builder
/// </summary>
public class MomoTransactionBuilder : NopEntityBuilder<MomoTransactionModel>
{
    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(MomoTransactionModel.ReferenceId)).AsString(50).NotNullable()
            .WithColumn(nameof(MomoTransactionModel.PhoneNumber)).AsString(20).NotNullable()
            .WithColumn(nameof(MomoTransactionModel.Amount)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(MomoTransactionModel.Currency)).AsString(5).NotNullable()
            .WithColumn(nameof(MomoTransactionModel.Status)).AsString(20).NotNullable()
            .WithColumn(nameof(MomoTransactionModel.CreatedAt)).AsDateTime().NotNullable()
            .WithColumn(nameof(MomoTransactionModel.UpdatedAt)).AsDateTime().Nullable()
            .WithColumn(nameof(MomoTransactionModel.ErrorMessage)).AsString(int.MaxValue).Nullable()
            .WithColumn(nameof(MomoTransactionModel.OrderId)).AsInt32().NotNullable();
    }
}
