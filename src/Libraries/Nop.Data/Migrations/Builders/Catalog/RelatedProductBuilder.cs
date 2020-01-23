using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class RelatedProductBuilder : BaseEntityBuilder<RelatedProduct>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {

        }

        #endregion
    }
}