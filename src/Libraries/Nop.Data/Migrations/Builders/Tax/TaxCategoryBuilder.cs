using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Tax;

namespace Nop.Data.Migrations.Builders
{
    public partial class TaxCategoryBuilder : BaseEntityBuilder<TaxCategory>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TaxCategory.Name)).AsString(400).NotNullable();
        }

        #endregion
    }
}