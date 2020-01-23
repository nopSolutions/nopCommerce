using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Migrations.Builders
{
    public partial class MeasureDimensionBuilder : BaseEntityBuilder<MeasureDimension>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MeasureDimension.Name)).AsString(100).NotNullable()
                .WithColumn(nameof(MeasureDimension.SystemKeyword)).AsString(100).NotNullable()
                .WithColumn(nameof(MeasureDimension.Ratio)).AsDecimal(18, 8);
        }

        #endregion
    }
}