using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<NewsLetterSubscription> builder)
        {
            builder.HasTableName(nameof(NewsLetterSubscription));

            builder.Property(subscription => subscription.Email).HasLength(255).IsNullable(false);
            builder.Property(subscription => subscription.NewsLetterSubscriptionGuid);
            builder.Property(subscription => subscription.Active);
            builder.Property(subscription => subscription.StoreId);
            builder.Property(subscription => subscription.CreatedOnUtc);
        }

        #endregion
    }
}