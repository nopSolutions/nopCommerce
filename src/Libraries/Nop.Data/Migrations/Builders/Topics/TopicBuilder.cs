using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Topics;
using Nop.Data.Extensions;

namespace Nop.Data.Migrations.Builders
{
    public partial class TopicBuilder : BaseEntityBuilder<Topic>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(Topic.TopicTemplateId))
                    .AsInt32()
                    .ForeignKey<TopicTemplate>();
        }

        #endregion
    }
}