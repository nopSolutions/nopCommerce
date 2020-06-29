using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Marketing;

namespace Nop.Data.Mapping.Builders.Marketing
{
    /// <summary>
    /// Represents a Marketing note entity builder
    /// </summary>
    public partial class DivisionsCodeChinaBuilder : NopEntityBuilder<DivisionsCodeChina>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(DivisionsCodeChina.AreaCode)).AsAnsiString(15).NotNullable()
                .WithColumn(nameof(DivisionsCodeChina.AreaName)).AsString(64).NotNullable()
                .WithColumn(nameof(DivisionsCodeChina.CountryCode)).AsAnsiString(15).Nullable()
                ;
        }

        #endregion
    }
}
