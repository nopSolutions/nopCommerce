using LinqToDB.Mapping;
using Nop.Core.Domain.Forums;
using Nop.Data.Data;

namespace Nop.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum post mapping configuration
    /// </summary>
    public partial class ForumPostMap : NopEntityTypeConfiguration<ForumPost>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ForumPost> builder)
        {
            builder.HasTableName(NopMappingDefaults.ForumsPostTable);

            builder.HasColumn(post => post.Text).IsColumnRequired();
            builder.Property(post => post.IPAddress).HasLength(100);

            builder.Property(forumpost => forumpost.TopicId);
            builder.Property(forumpost => forumpost.CustomerId);
            builder.Property(forumpost => forumpost.CreatedOnUtc);
            builder.Property(forumpost => forumpost.UpdatedOnUtc);
            builder.Property(forumpost => forumpost.VoteCount);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(post => post.ForumTopic)
            //    .WithMany()
            //    .HasForeignKey(post => post.TopicId)
            //    .IsColumnRequired();

            //builder.HasOne(post => post.Customer)
            //   .WithMany()
            //   .HasForeignKey(post => post.CustomerId)
            //   .IsColumnRequired()
            //   .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion
    }
}