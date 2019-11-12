using LinqToDB.Mapping;
using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    /// <summary>
    /// Represents an address mapping configuration
    /// </summary>
    public partial class AddressMap : NopEntityTypeConfiguration<Address>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Address> builder)
        {
            builder.HasTableName(nameof(Address));

            builder.Property(address => address.FirstName);
            builder.Property(address => address.LastName);
            builder.Property(address => address.Email);
            builder.Property(address => address.Company);
            builder.Property(address => address.CountryId);
            builder.Property(address => address.StateProvinceId);
            builder.Property(address => address.County);
            builder.Property(address => address.City);
            builder.Property(address => address.Address1);
            builder.Property(address => address.Address2);
            builder.Property(address => address.ZipPostalCode);
            builder.Property(address => address.PhoneNumber);
            builder.Property(address => address.FaxNumber);
            builder.Property(address => address.CustomAttributes);
            builder.Property(address => address.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(address => address.Country)
            //    .WithMany()
            //    .HasForeignKey(address => address.CountryId);

            //builder.HasOne(address => address.StateProvince)
            //    .WithMany()
            //    .HasForeignKey(address => address.StateProvinceId);
        }

        #endregion
    }
}