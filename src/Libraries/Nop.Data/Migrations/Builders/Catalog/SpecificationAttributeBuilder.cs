using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Builders
{
    public partial class SpecificationAttributeBuilder : BaseEntityBuilder<SpecificationAttribute>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(SpecificationAttribute.Name)).AsString(int.MaxValue).NotNullable();
        }

        #endregion
    }
}