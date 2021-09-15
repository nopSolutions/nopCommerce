using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Builders.Directory
{
    /// <summary>
    /// Represents a measure weight entity builder
    /// </summary>
    public partial class MeasureWeightBuilder : NopEntityBuilder<MeasureWeight>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MeasureWeight.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(MeasureWeight.SystemKeyword)).AsString(100).NotNullable()
                .WithColumn(nameof(MeasureWeight.Ratio)).AsDecimal(18, 8);
        }

        #endregion
    }
}