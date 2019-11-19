using LinqToDB.Mapping;
using Nop.Core.Domain.Forums;

namespace Nop.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a forum subscription mapping configuration
    /// </summary>
    public partial class ForumSubscriptionMap : NopEntityTypeConfiguration<ForumSubscription>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ForumSubscription> builder)
        {
            builder.HasTableName(NopMappingDefaults.ForumsSubscriptionTable);

            builder.Property(subscription => subscription.SubscriptionGuid);
            builder.Property(subscription => subscription.CustomerId);
            builder.Property(subscription => subscription.ForumId);
            builder.Property(subscription => subscription.TopicId);
            builder.Property(subscription => subscription.CreatedOnUtc);
        }

        #endregion
    }
}