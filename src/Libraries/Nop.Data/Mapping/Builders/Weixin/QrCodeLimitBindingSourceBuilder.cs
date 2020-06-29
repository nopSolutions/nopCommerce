using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a WShare Count entity builder
    /// </summary>
    public partial class QrCodeLimitBindingSourceBuilder : NopEntityBuilder<QrCodeLimitBindingSource>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(QrCodeLimitBindingSource.QrCodeLimitId)).AsInt32().ForeignKey<WQrCodeLimit>()
                .WithColumn(nameof(QrCodeLimitBindingSource.Address)).AsString(32).Nullable()
                .WithColumn(nameof(QrCodeLimitBindingSource.Url)).AsString(1024).Nullable()
                ;
        }

        #endregion
    }
}
