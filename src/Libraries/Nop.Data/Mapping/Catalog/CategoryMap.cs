using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a category mapping configuration
    /// </summary>
    public partial class CategoryMap : NopEntityTypeConfiguration<Category>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Category> builder)
        {
            builder.HasTableName(nameof(Category));

            builder.Property(category => category.Name).HasLength(400).IsNullable(false);
            builder.Property(category => category.MetaKeywords).HasLength(400);
            builder.Property(category => category.MetaTitle).HasLength(400);
            builder.Property(category => category.PriceRanges).HasLength(400);
            builder.Property(category => category.PageSizeOptions).HasLength(200);
            builder.Property(category => category.Description);
            builder.Property(category => category.CategoryTemplateId);
            builder.Property(category => category.MetaDescription);
            builder.Property(category => category.ParentCategoryId);
            builder.Property(category => category.PictureId);
            builder.Property(category => category.PageSize);
            builder.Property(category => category.AllowCustomersToSelectPageSize);
            builder.Property(category => category.ShowOnHomepage);
            builder.Property(category => category.IncludeInTopMenu);
            builder.Property(category => category.SubjectToAcl);
            builder.Property(category => category.LimitedToStores);
            builder.Property(category => category.Published);
            builder.Property(category => category.Deleted);
            builder.Property(category => category.DisplayOrder);
            builder.Property(category => category.CreatedOnUtc);
            builder.Property(category => category.UpdatedOnUtc);
        }

        #endregion
    }
}