using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    public partial class ForumPostVoteMap : NopEntityTypeConfiguration<ForumPostVote>
    {
        public ForumPostVoteMap()
        {
            this.ToTable("Forums_PostVote");
            this.HasKey(fpv => fpv.Id);

            this.HasRequired(fpv => fpv.ForumPost)
                .WithMany()
                .HasForeignKey(fpv => fpv.ForumPostId)
                .WillCascadeOnDelete(true);
        }
    }
}
