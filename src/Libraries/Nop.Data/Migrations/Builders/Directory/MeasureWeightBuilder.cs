using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Migrations.Builders
{
    public partial class MeasureWeightBuilder : BaseEntityBuilder<MeasureWeight>
    {
        #region Methods

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