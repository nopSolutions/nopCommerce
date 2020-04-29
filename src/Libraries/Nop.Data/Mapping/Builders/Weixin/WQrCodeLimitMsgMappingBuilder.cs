using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WQrCodeLimitMsgMappingBuilder : NopEntityBuilder<WQrCodeLimitMsgMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WQrCodeLimitMsgMapping.WQrCodeLimitId)).AsInt32().ForeignKey<WQrCodeLimit>()
                .WithColumn(nameof(WQrCodeLimitMsgMapping.MessageIds)).AsAnsiString(64).Nullable()
                ;
        }

        #endregion
    }
}
