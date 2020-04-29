using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WOauthBuilder : NopEntityBuilder<WOauth>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WOauth.OpenId)).AsAnsiString(32).NotNullable()
                .WithColumn(nameof(WOauth.AccessToken)).AsAnsiString(128).NotNullable()
                .WithColumn(nameof(WOauth.RefreshToken)).AsAnsiString(128).NotNullable()
                .WithColumn(nameof(WOauth.Scope)).AsString(128).Nullable()
                ;
        }

        #endregion
    }
}
