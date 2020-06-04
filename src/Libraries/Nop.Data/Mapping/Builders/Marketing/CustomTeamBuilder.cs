using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Weixin;
using Nop.Data.Extensions;
using System.Data;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class CustomTeamBuilder : NopEntityBuilder<CustomTeam>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(CustomTeam.ProductId)).AsInt32().ForeignKey<Product>()
                .WithColumn(nameof(CustomTeam.ProductAttributeValueId)).AsInt32().Nullable().ForeignKey<ProductAttributeValue>().OnDelete(Rule.None)
                .WithColumn(nameof(CustomTeam.WUserId)).AsInt32().Nullable().ForeignKey<WUser>().OnDelete(Rule.SetNull)
                .WithColumn(nameof(CustomTeam.GroupCode)).AsAnsiString(6).Nullable()
                .WithColumn(nameof(CustomTeam.MaxGroupAmount)).AsDecimal(9,2)
                .WithColumn(nameof(CustomTeam.MinGroupAmount)).AsDecimal(9,2)
                .WithColumn(nameof(CustomTeam.GroupPassword)).AsString(32).Nullable()
                .WithColumn(nameof(CustomTeam.StartDateTimeUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(CustomTeam.EndDateTimeUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(CustomTeam.Name)).AsString(64).NotNullable()
                .WithColumn(nameof(CustomTeam.Description)).AsString(8000).Nullable()
                .WithColumn(nameof(CustomTeam.UseCondition)).AsString(8000).Nullable()
                .WithColumn(nameof(CustomTeam.Content)).AsString(8000).Nullable()
                ;
        }

        #endregion
    }
}
