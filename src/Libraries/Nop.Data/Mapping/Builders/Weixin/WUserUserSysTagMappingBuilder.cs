using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WUserUserSysTagMappingBuilder : NopEntityBuilder<WUserUserSysTagMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(WUserUserSysTagMapping), nameof(WUserUserSysTagMapping.WUserId)))
                    .AsInt32().PrimaryKey().ForeignKey<WUser>()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(WUserUserSysTagMapping), nameof(WUserUserSysTagMapping.WUserSysTagId)))
                    .AsInt32().PrimaryKey().ForeignKey<WUserSysTag>()
                ;
        }

        #endregion
    }
}
