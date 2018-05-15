using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class AddressMap : NopEntityTypeConfiguration<Address>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public AddressMap()
        {
            this.ToTable("Address");
            this.HasKey(a => a.Id);

            this.HasOptional(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryId).WillCascadeOnDelete(false);

            this.HasOptional(a => a.StateProvince)
                .WithMany()
                .HasForeignKey(a => a.StateProvinceId).WillCascadeOnDelete(false);
        }
    }
}
