using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.ToTable(nameof(Poll));
            builder.HasKey(poll => poll.Id);

            builder.Property(poll => poll.Name).IsRequired();

            builder.HasOne(poll => poll.Language)
                .WithMany()
                .HasForeignKey(poll => poll.LanguageId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}