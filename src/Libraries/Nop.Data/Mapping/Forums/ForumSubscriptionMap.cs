using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<ForumSubscription> builder)
        {
            builder.ToTable(NopMappingDefaults.ForumsSubscriptionTable);
            builder.HasKey(subscription => subscription.Id);

            builder.HasOne(subscription => subscription.Customer)
                .WithMany()
                .HasForeignKey(subscription => subscription.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }

        #endregion
    }
}