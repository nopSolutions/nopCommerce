using LinqToDB.Mapping;
using Nop.Core.Domain.Affiliates;

namespace Nop.Data.Mapping.Affiliates
{
    /// <summary>
    /// Represents an affiliate mapping configuration
    /// </summary>
    public partial class AffiliateMap : NopEntityTypeConfiguration<Affiliate>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Affiliate> builder)
        {
            builder.HasTableName(nameof(Affiliate));

            builder.Property(affiliate => affiliate.AddressId);
            builder.Property(affiliate => affiliate.AdminComment);
            builder.Property(affiliate => affiliate.FriendlyUrlName);
            builder.Property(affiliate => affiliate.Deleted);
            builder.Property(affiliate => affiliate.Active);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(affiliate => affiliate.Address)
            //    .WithMany()
            //    .HasForeignKey(affiliate => affiliate.AddressId)
            //    .IsColumnRequired()
            //    .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion
    }
}