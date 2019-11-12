using LinqToDB.Mapping;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount requirement mapping configuration
    /// </summary>
    public partial class DiscountRequirementMap : NopEntityTypeConfiguration<DiscountRequirement>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DiscountRequirement> builder)
        {
            builder.HasTableName(nameof(DiscountRequirement));

            builder.Property(requirement => requirement.DiscountId);
            builder.Property(requirement => requirement.DiscountRequirementRuleSystemName);
            builder.Property(requirement => requirement.ParentId);
            builder.Property(requirement => requirement.InteractionTypeId);
            builder.Property(requirement => requirement.IsGroup);

            builder.Ignore(requirement => requirement.InteractionType);

            //TODO: 239 Try to add ForeignKey
            //builder.HasMany(requirement => requirement.ChildRequirements)
            //    .WithOne()
            //    .HasForeignKey(requirement => requirement.ParentId);
        }

        #endregion
    }
}