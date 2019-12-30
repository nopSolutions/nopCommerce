using LinqToDB.Mapping;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Topics
{
    /// <summary>
    /// Represents a topic template mapping configuration
    /// </summary>
    public partial class TopicTemplateMap : NopEntityTypeConfiguration<TopicTemplate>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<TopicTemplate> builder)
        {
            builder.HasTableName(nameof(TopicTemplate));

            builder.Property(template => template.Name).HasLength(400).IsNullable(false);
            builder.Property(template => template.ViewPath).HasLength(400).IsNullable(false);
            builder.Property(template => template.DisplayOrder);
        }

        #endregion
    }
}