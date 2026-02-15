using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Payments.Momo.Models;

namespace Nop.Plugin.Payments.Momo.Data;

/// <summary>
/// Represents a MoMo transaction entity builder
/// </summary>
public class MomoTransactionBuilder : NopEntityBuilder<MomoTransaction>
{
    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(MomoTransaction.ReferenceId)).AsString(50).NotNullable()
            .WithColumn(nameof(MomoTransaction.PhoneNumber)).AsString(20).NotNullable()
            .WithColumn(nameof(MomoTransaction.Amount)).AsDecimal(18, 4).NotNullable()
            .WithColumn(nameof(MomoTransaction.Currency)).AsString(5).NotNullable()
            .WithColumn(nameof(MomoTransaction.Status)).AsString(20).NotNullable()
            .WithColumn(nameof(MomoTransaction.CreatedAt)).AsDateTime().NotNullable()
            .WithColumn(nameof(MomoTransaction.UpdatedAt)).AsDateTime().Nullable()
            .WithColumn(nameof(MomoTransaction.ErrorMessage)).AsString(int.MaxValue).Nullable()
            .WithColumn(nameof(MomoTransaction.OrderId)).AsInt32().NotNullable();
    }
}
