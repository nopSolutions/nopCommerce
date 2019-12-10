using LinqToDB.Mapping;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a checkout attribute mapping configuration
    /// </summary>
    public partial class CheckoutAttributeMap : NopEntityTypeConfiguration<CheckoutAttribute>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<CheckoutAttribute> builder)
        {
            builder.HasTableName(nameof(CheckoutAttribute));

            builder.Property(attribute => attribute.Name).HasLength(400).IsNullable(false);
            builder.Property(attribute => attribute.TextPrompt);
            builder.Property(attribute => attribute.IsRequired);
            builder.Property(attribute => attribute.ShippableProductRequired);
            builder.Property(attribute => attribute.IsTaxExempt);
            builder.Property(attribute => attribute.TaxCategoryId);
            builder.Property(attribute => attribute.AttributeControlTypeId);
            builder.Property(attribute => attribute.DisplayOrder);
            builder.Property(attribute => attribute.LimitedToStores);
            builder.Property(attribute => attribute.ValidationMinLength);
            builder.Property(attribute => attribute.ValidationMaxLength);
            builder.Property(attribute => attribute.ValidationFileAllowedExtensions);
            builder.Property(attribute => attribute.ValidationFileMaximumSize);
            builder.Property(attribute => attribute.DefaultValue);
            builder.Property(attribute => attribute.ConditionAttributeXml);

            builder.Ignore(attribute => attribute.AttributeControlType);
        }

        #endregion
    }
}