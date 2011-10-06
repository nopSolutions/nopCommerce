using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    public partial class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            this.ToTable("Address");
            this.HasKey(a => a.Id);

            //TODO decide whether we should delete address if country is deleted 
            this.HasOptional(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryId).WillCascadeOnDelete(false);


            //TODO decide whether we should delete address if state is deleted
            this.HasOptional(a => a.StateProvince)
                .WithMany()
                .HasForeignKey(a => a.StateProvinceId).WillCascadeOnDelete(false);
        }
    }
}
