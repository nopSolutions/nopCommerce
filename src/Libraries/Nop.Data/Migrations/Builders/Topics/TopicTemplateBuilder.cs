using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Migrations.Builders
{
    public partial class TopicTemplateBuilder : BaseEntityBuilder<TopicTemplate>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(TopicTemplate.Name)).AsString(400).NotNullable()
                .WithColumn(nameof(TopicTemplate.ViewPath)).AsString(400).NotNullable();
        }

        #endregion
    }
}