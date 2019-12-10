using LinqToDB.Mapping;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum topic mapping configuration
    /// </summary>
    public partial class ForumTopicMap : NopEntityTypeConfiguration<ForumTopic>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ForumTopic> builder)
        {
            builder.HasTableName(NopMappingDefaults.ForumsTopicTable);

            builder.Property(topic => topic.Subject).HasLength(450).IsNullable(false);
            builder.Property(forumtopic => forumtopic.ForumId);
            builder.Property(forumtopic => forumtopic.CustomerId);
            builder.Property(forumtopic => forumtopic.TopicTypeId);
            builder.Property(forumtopic => forumtopic.NumPosts);
            builder.Property(forumtopic => forumtopic.Views);
            builder.Property(forumtopic => forumtopic.LastPostId);
            builder.Property(forumtopic => forumtopic.LastPostCustomerId);
            builder.Property(forumtopic => forumtopic.LastPostTime);
            builder.Property(forumtopic => forumtopic.CreatedOnUtc);
            builder.Property(forumtopic => forumtopic.UpdatedOnUtc);

            builder.Ignore(topic => topic.ForumTopicType);
            builder.Ignore(topic => topic.NumReplies);
        }

        #endregion
    }
}