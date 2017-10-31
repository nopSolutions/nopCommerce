using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class AddressMap : NopEntityTypeConfiguration<Address>
    {
        public override void Configure(EntityTypeBuilder<Address> builder)
        {
            base.Configure(builder);
            builder.ToTable("Address");
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.StateProvince)
                .WithMany()
                .HasForeignKey(a => a.StateProvinceId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
