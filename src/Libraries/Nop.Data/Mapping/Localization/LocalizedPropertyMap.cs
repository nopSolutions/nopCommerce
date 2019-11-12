using LinqToDB.Mapping;
using Nop.Core.Domain.Localization;

namespace Nop.Data.Mapping.Localization
{
    /// <summary>
    /// Represents a localized property mapping configuration
    /// </summary>
    public partial class LocalizedPropertyMap : NopEntityTypeConfiguration<LocalizedProperty>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<LocalizedProperty> builder)
        {
            builder.HasTableName(nameof(LocalizedProperty));

            builder.Property(property => property.LocaleKeyGroup).HasLength(400);
            builder.Property(property => property.LocaleKey).HasLength(400);
            builder.HasColumn(property => property.LocaleValue).IsColumnRequired();
            builder.HasColumn(property => property.LocaleKeyGroup).IsColumnRequired();
            builder.HasColumn(property => property.LocaleKey).IsColumnRequired();

            builder.Property(localizedproperty => localizedproperty.EntityId);
            builder.Property(localizedproperty => localizedproperty.LanguageId);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(property => property.Language)
            //    .WithMany()
            //    .HasForeignKey(property => property.LanguageId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}