using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Migrations.Builders
{
    public partial class ReviewTypeBuilder : BaseEntityBuilder<ReviewType>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ReviewType.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(ReviewType.Description)).AsString(400).NotNullable();
        }

        #endregion
    }
}
