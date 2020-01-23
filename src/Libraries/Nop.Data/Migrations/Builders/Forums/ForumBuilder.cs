using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Forums;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class ForumBuilder : BaseEntityBuilder<Forum>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Forum.Name)).AsString(200).NotNullable()
                .WithColumn(nameof(Forum.ForumGroupId))
                    .AsInt32()
                    .ForeignKey<ForumGroup>();
        }

        #endregion
    }
}