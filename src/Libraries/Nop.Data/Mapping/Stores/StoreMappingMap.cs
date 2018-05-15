using Nop.Core.Domain.Stores;

namespace Nop.Data.Mapping.Stores
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class StoreMappingMap : NopEntityTypeConfiguration<StoreMapping>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public StoreMappingMap()
        {
            this.ToTable("StoreMapping");
            this.HasKey(sm => sm.Id);

            this.Property(sm => sm.EntityName).IsRequired().HasMaxLength(400);

            this.HasRequired(sm => sm.Store)
                .WithMany()
                .HasForeignKey(sm => sm.StoreId)
                .WillCascadeOnDelete(true);
        }
    }
}