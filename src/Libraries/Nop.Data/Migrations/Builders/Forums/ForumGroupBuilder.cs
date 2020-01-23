using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Migrations.Builders
{
    public partial class ForumGroupBuilder : BaseEntityBuilder<ForumGroup>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
               .WithColumn(nameof(ForumGroup.Name)).AsString(200).NotNullable();
        }

        #endregion
    }
}