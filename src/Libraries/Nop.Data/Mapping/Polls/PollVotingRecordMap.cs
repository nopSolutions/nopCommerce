using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<PollVotingRecord> builder)
        {
            builder.HasTableName(nameof(PollVotingRecord));

            builder.Property(pollvotingrecord => pollvotingrecord.PollAnswerId);
            builder.Property(pollvotingrecord => pollvotingrecord.CustomerId);
            builder.Property(pollvotingrecord => pollvotingrecord.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(record => record.PollAnswer)
            //    .WithMany(pollAnswer => pollAnswer.PollVotingRecords)
            //    .HasForeignKey(record => record.PollAnswerId)
            //    .IsColumnRequired();

            //builder.HasOne(record => record.Customer)
            //    .WithMany()
            //    .HasForeignKey(record => record.CustomerId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}