using LinqToDB.Mapping;
using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Topics
{
    /// <summary>
    /// Represents a topic mapping configuration
    /// </summary>
    public partial class TopicMap : NopEntityTypeConfiguration<Topic>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Topic> builder)
        {
            builder.HasTableName(nameof(Topic));

            builder.Property(topic => topic.SystemName);
            builder.Property(topic => topic.IncludeInSitemap);
            builder.Property(topic => topic.IncludeInTopMenu);
            builder.Property(topic => topic.IncludeInFooterColumn1);
            builder.Property(topic => topic.IncludeInFooterColumn2);
            builder.Property(topic => topic.IncludeInFooterColumn3);
            builder.Property(topic => topic.DisplayOrder);
            builder.Property(topic => topic.AccessibleWhenStoreClosed);
            builder.Property(topic => topic.IsPasswordProtected);
            builder.Property(topic => topic.Password);
            builder.Property(topic => topic.Title);
            builder.Property(topic => topic.Body);
            builder.Property(topic => topic.Published);
            builder.Property(topic => topic.TopicTemplateId);
            builder.Property(topic => topic.MetaKeywords);
            builder.Property(topic => topic.MetaDescription);
            builder.Property(topic => topic.MetaTitle);
            builder.Property(topic => topic.SubjectToAcl);
            builder.Property(topic => topic.LimitedToStores);
        }

        #endregion
    }
}