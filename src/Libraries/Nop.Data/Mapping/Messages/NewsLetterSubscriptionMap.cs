using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Messages;

namespace Nop.Data.Mapping.Messages
{
    /// <summary>
    /// Represents a newsLetter subscription mapping configuration
    /// </summary>
    public partial class NewsLetterSubscriptionMap : NopEntityTypeConfiguration<NewsLetterSubscription>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<NewsLetterSubscription> builder)
        {
            builder.ToTable(nameof(NewsLetterSubscription));
            builder.HasKey(subscription => subscription.Id);

            builder.Property(subscription => subscription.Email).HasMaxLength(255).IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}