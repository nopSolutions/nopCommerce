using LinqToDB.Mapping;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Represents an external authentication record mapping configuration
    /// </summary>
    public partial class ExternalAuthenticationRecordMap : NopEntityTypeConfiguration<ExternalAuthenticationRecord>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ExternalAuthenticationRecord> builder)
        {
            builder.HasTableName(nameof(ExternalAuthenticationRecord));

            builder.Property(record => record.CustomerId);
            builder.Property(record => record.Email);
            builder.Property(record => record.ExternalIdentifier);
            builder.Property(record => record.ExternalDisplayIdentifier);
            builder.Property(record => record.OAuthToken);
            builder.Property(record => record.OAuthAccessToken);
            builder.Property(record => record.ProviderSystemName);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(record => record.Customer)
            //    .WithMany(customer => customer.ExternalAuthenticationRecords)
            //    .HasForeignKey(record => record.CustomerId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}