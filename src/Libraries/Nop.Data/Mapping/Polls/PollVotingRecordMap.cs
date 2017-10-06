using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Polls;

namespace Nop.Data.Mapping.Polls
{
    public partial class PollVotingRecordMap : NopEntityTypeConfiguration<PollVotingRecord>
    {
        public override void Configure(EntityTypeBuilder<PollVotingRecord> builder)
        {
            base.Configure(builder);
            builder.ToTable("PollVotingRecord");
            builder.HasKey(pr => pr.Id);

            builder.HasOne(pvr => pvr.PollAnswer)
                .WithMany(pa => pa.PollVotingRecords)
                .IsRequired(true)
                .HasForeignKey(pvr => pvr.PollAnswerId);

            builder.HasOne(cc => cc.Customer)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(cc => cc.CustomerId);
        }
    }
}