using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a category
    /// </summary>
    public partial class Category : BaseEntity, ILocalizedEntity
    {
        private ICollection<Discount> _appliedDiscounts;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets a value of used category template identifier
        /// </summary>
        public virtual int CategoryTemplateId { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public virtual string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public virtual string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public virtual string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the search-engine name
        /// </summary>
        public virtual string SeName { get; set; }

        /// <summary>
        /// Gets or sets the parent category identifier
        /// </summary>
        public virtual int ParentCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public virtual int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public virtual int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        public virtual bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
        public virtual string PageSizeOptions { get; set; }

        /// <summary>
        /// Gets or sets the available price ranges
        /// </summary>
        public virtual string PriceRanges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the category on home page
        /// </summary>
        public virtual bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this category has discounts applied
        /// <remarks>The same as if we run category.AppliedDiscounts.Count > 0
        /// We use this property for performance optimization:
        /// if this property is set to false, then we do not need to load Applied Discounts navifation property
        /// </remarks>
        /// </summary>
        public virtual bool HasDiscountsApplied { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public virtual DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the collection of applied discounts
        /// </summary>
        public virtual ICollection<Discount> AppliedDiscounts
        {
            get { return _appliedDiscounts ?? (_appliedDiscounts = new List<Discount>()); }
            protected set { _appliedDiscounts = value; }
        }
    }
}
