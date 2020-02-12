using LinqToDB.Mapping;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum mapping configuration
    /// </summary>
    public partial class ForumMap : NopEntityTypeConfiguration<Forum>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Forum> builder)
        {
            builder.HasTableName(NopMappingDefaults.ForumTable);

            builder.Property(forum => forum.Name).HasLength(200).IsNullable(false);
            builder.Property(forum => forum.ForumGroupId);
            builder.Property(forum => forum.Description);
            builder.Property(forum => forum.NumTopics);
            builder.Property(forum => forum.NumPosts);
            builder.Property(forum => forum.LastTopicId);
            builder.Property(forum => forum.LastPostId);
            builder.Property(forum => forum.LastPostCustomerId);
            builder.Property(forum => forum.LastPostTime);
            builder.Property(forum => forum.DisplayOrder);
            builder.Property(forum => forum.CreatedOnUtc);
            builder.Property(forum => forum.UpdatedOnUtc);
        }

        #endregion
    }
}