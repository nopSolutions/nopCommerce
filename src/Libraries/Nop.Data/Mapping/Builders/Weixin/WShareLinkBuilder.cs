using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WShareLinkBuilder : NopEntityBuilder<WShareLink>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WShareLink.LinkId)).AsAnsiString(6).NotNullable()
                .WithColumn(nameof(WShareLink.OpenId)).AsAnsiString(32).NotNullable()
                .WithColumn(nameof(WShareLink.Url)).AsString(1024).NotNullable()
                ;
        }

        #endregion
    }
}
