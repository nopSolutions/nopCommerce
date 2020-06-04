using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a WShare Count entity builder
    /// </summary>
    public partial class WShareCountBuilder : NopEntityBuilder<WShareCount>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WShareCount.WShareLinkId)).AsInt32().ForeignKey<WShareLink>()
                .WithColumn(nameof(WShareCount.OpenId)).AsAnsiString(32).NotNullable()
                ;
        }

        #endregion
    }
}
