using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    public partial class ForumPostVoteMap : NopEntityTypeConfiguration<ForumPostVote>
    {
        public override void Configure(EntityTypeBuilder<ForumPostVote> builder)
        {
            base.Configure(builder);
            builder.ToTable("Forums_PostVote");
            builder.HasKey(fpv => fpv.Id);

            builder.HasOne(fpv => fpv.ForumPost)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(fpv => fpv.ForumPostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
