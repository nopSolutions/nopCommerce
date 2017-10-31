using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Stores;

namespace Nop.Data.Mapping.Stores
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class StoreMappingMap : NopEntityTypeConfiguration<StoreMapping>
    {
        public override void Configure(EntityTypeBuilder<StoreMapping> builder)
        {
            base.Configure(builder);
            builder.ToTable("StoreMapping");
            builder.HasKey(sm => sm.Id);

            builder.Property(sm => sm.EntityName).IsRequired().HasMaxLength(400);

            builder.HasOne(sm => sm.Store)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(sm => sm.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}