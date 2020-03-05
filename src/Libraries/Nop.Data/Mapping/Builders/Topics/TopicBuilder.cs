using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Topics;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Topics
{
    /// <summary>
    /// Represents a topic entity builder
    /// </summary>
    public partial class TopicBuilder : NopEntityBuilder<Topic>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(Topic.TopicTemplateId)).AsInt32().ForeignKey<TopicTemplate>();
        }

        #endregion
    }
}