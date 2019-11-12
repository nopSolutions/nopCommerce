using LinqToDB.Mapping;
using Nop.Core.Domain.Polls;

namespace Nop.Data.Mapping.Polls
{
    /// <summary>
    /// Represents a poll answer mapping configuration
    /// </summary>
    public partial class PollAnswerMap : NopEntityTypeConfiguration<PollAnswer>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<PollAnswer> builder)
        {
            builder.HasTableName(nameof(PollAnswer));

            builder.HasColumn(pollAnswer => pollAnswer.Name).IsColumnRequired();
            builder.Property(pollAnswer => pollAnswer.PollId);
            builder.Property(pollAnswer => pollAnswer.NumberOfVotes);
            builder.Property(pollAnswer => pollAnswer.DisplayOrder);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(pollAnswer => pollAnswer.Poll)
            //    .WithMany(poll => poll.PollAnswers)
            //    .HasForeignKey(pollAnswer => pollAnswer.PollId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}