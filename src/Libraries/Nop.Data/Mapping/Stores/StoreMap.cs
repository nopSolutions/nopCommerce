using LinqToDB.Mapping;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Mapping.Stores
{
    /// <summary>
    /// Represents a store mapping configuration
    /// </summary>
    public partial class StoreMap : NopEntityTypeConfiguration<Store>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Store> builder)
        {
            builder.HasTableName(nameof(Store));

            builder.Property(store => store.Name).HasLength(400).IsNullable(false);
            builder.Property(store => store.Url).HasLength(400).IsNullable(false);
            builder.Property(store => store.Hosts).HasLength(1000);
            builder.Property(store => store.CompanyName).HasLength(1000);
            builder.Property(store => store.CompanyAddress).HasLength(1000);
            builder.Property(store => store.CompanyPhoneNumber).HasLength(1000);
            builder.Property(store => store.CompanyVat).HasLength(1000);

            builder.Property(store => store.SslEnabled);
            builder.Property(store => store.DefaultLanguageId);
            builder.Property(store => store.DisplayOrder);
        }

        #endregion
    }
}