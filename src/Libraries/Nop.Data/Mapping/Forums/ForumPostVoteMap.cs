using LinqToDB.Mapping;
using Nop.Core.Domain.Forums;
using Nop.Data.Data;

namespace Nop.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum post vote mapping configuration
    /// </summary>
    public partial class ForumPostVoteMap : NopEntityTypeConfiguration<ForumPostVote>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ForumPostVote> builder)
        {
            builder.HasTableName(NopMappingDefaults.ForumsPostVoteTable);

            builder.Property(forumpostvote => forumpostvote.ForumPostId);
            builder.Property(forumpostvote => forumpostvote.CustomerId);
            builder.Property(forumpostvote => forumpostvote.IsUp);
            builder.Property(forumpostvote => forumpostvote.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(postVote => postVote.ForumPost)
            //    .WithMany()
            //    .HasForeignKey(postVote => postVote.ForumPostId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}