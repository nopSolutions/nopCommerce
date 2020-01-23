using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class SpecificationAttributeOptionBuilder : BaseEntityBuilder<SpecificationAttributeOption>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(SpecificationAttributeOption.Name)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(SpecificationAttributeOption.ColorSquaresRgb)).AsString(100).Nullable()
                .WithColumn(nameof(SpecificationAttributeOption.SpecificationAttributeId))
                    .AsInt32()
                    .ForeignKey<SpecificationAttribute>();
        }

        #endregion
    }
}