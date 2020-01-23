using FluentMigrator.Builders.Create.Table;
using LinqToDB.Mapping;
using Nop.Core.Domain.Media;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class DownloadBuilder : BaseEntityBuilder<Download>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {

        }

        #endregion
    }
}