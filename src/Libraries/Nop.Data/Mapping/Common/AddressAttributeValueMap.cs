using LinqToDB.Mapping;
using Nop.Core.Domain.Common;

namespace Nop.Data.Mapping.Common
{
    /// <summary>
    /// Represents an address attribute value mapping configuration
    /// </summary>
    public partial class AddressAttributeValueMap : NopEntityTypeConfiguration<AddressAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<AddressAttributeValue> builder)
        {
            builder.HasTableName(nameof(AddressAttributeValue));

            builder.Property(value => value.Name).HasLength(400).IsNullable(false);
            builder.Property(addressattributevalue => addressattributevalue.AddressAttributeId);
            builder.Property(addressattributevalue => addressattributevalue.IsPreSelected);
            builder.Property(addressattributevalue => addressattributevalue.DisplayOrder);
        }

        #endregion
    }
}