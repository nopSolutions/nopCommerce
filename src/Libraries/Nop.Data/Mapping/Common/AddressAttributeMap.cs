using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class AddressAttributeMap : NopEntityTypeConfiguration<AddressAttribute>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public AddressAttributeMap()
        {
            this.ToTable("AddressAttribute");
            this.HasKey(aa => aa.Id);
            this.Property(aa => aa.Name).IsRequired().HasMaxLength(400);

            this.Ignore(aa => aa.AttributeControlType);
        }
    }
}