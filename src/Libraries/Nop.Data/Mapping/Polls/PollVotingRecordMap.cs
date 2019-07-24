using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Polls;

namespace Nop.Data.Mapping.Polls
{
    /// <summary>
    /// Represents a poll voting record mapping configuration
    /// </summary>
    public partial class PollVotingRecordMap : NopEntityTypeConfiguration<PollVotingRecord>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<PollVotingRecord> builder)
        {
            builder.ToTable(nameof(PollVotingRecord));
            builder.HasKey(record => record.Id);

            builder.HasOne(record => record.PollAnswer)
                .WithMany(pollAnswer => pollAnswer.PollVotingRecords)
                .HasForeignKey(record => record.PollAnswerId)
                .IsRequired();

            builder.HasOne(record => record.Customer)
                .WithMany()
                .HasForeignKey(record => record.CustomerId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}