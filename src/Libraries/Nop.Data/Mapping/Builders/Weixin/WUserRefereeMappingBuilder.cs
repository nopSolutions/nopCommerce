using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WUserRefereeMappingBuilder : NopEntityBuilder<WUserRefereeMapping>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(WUserRefereeMapping), nameof(WUserRefereeMapping.OpenId)))
                    .AsAnsiString(32).PrimaryKey()
                .WithColumn(NameCompatibilityManager.GetColumnName(typeof(WUserRefereeMapping), nameof(WUserRefereeMapping.OpenIdReferee)))
                    .AsAnsiString(32).PrimaryKey()
                ;
        }

        #endregion
    }
}
