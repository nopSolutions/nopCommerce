using LinqToDB.Mapping;
using Nop.Core.Domain.Polls;

namespace Nop.Data.Mapping.Polls
{
    /// <summary>
    /// Represents a poll mapping configuration
    /// </summary>
    public partial class PollMap : NopEntityTypeConfiguration<Poll>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Poll> builder)
        {
            builder.HasTableName(nameof(Poll));

            builder.HasColumn(poll => poll.Name).IsColumnRequired();

            builder.Property(poll => poll.LanguageId);
            builder.Property(poll => poll.SystemKeyword);
            builder.Property(poll => poll.Published);
            builder.Property(poll => poll.ShowOnHomepage);
            builder.Property(poll => poll.AllowGuestsToVote);
            builder.Property(poll => poll.DisplayOrder);
            builder.Property(poll => poll.LimitedToStores);
            builder.Property(poll => poll.StartDateUtc);
            builder.Property(poll => poll.EndDateUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(poll => poll.Language)
            //    .WithMany()
            //    .HasForeignKey(poll => poll.LanguageId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}