using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Weixin;

namespace Nop.Data.Mapping.Builders.Weixin
{
    /// <summary>
    /// Represents a vendor note entity builder
    /// </summary>
    public partial class WLocationBuilder : NopEntityBuilder<WLocation>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(WLocation.Latitude)).AsDecimal(9, 6)
                .WithColumn(nameof(WLocation.Longitude)).AsDecimal(9, 6)
                .WithColumn(nameof(WLocation.Precision)).AsDecimal(9, 6)
                ;
        }

        #endregion
    }
}
