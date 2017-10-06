using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Polls;

namespace Nop.Data.Mapping.Polls
{
    public partial class PollAnswerMap : NopEntityTypeConfiguration<PollAnswer>
    {
        public override void Configure(EntityTypeBuilder<PollAnswer> builder)
        {
            base.Configure(builder);
            builder.ToTable("PollAnswer");
            builder.HasKey(pa => pa.Id);
            builder.Property(pa => pa.Name).IsRequired();

            builder.HasOne(pa => pa.Poll)
                .WithMany(p => p.PollAnswers)
                .IsRequired(true)
                .HasForeignKey(pa => pa.PollId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}