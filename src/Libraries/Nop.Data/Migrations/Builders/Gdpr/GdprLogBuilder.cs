using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Gdpr;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class GdprLogBuilder : BaseEntityBuilder<GdprLog>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {

        }

        #endregion
    }
}