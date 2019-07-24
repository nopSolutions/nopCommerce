using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount
    /// </summary>
    public partial class Discount : BaseEntity
    {
        private ICollection<DiscountRequirement> _discountRequirements;
        private ICollection<DiscountCategoryMapping> _discountCategoryMappings;
        private ICollection<DiscountManufacturerMapping> _discountManufacturerMappings;
        private ICollection<DiscountProductMapping> _discountProductMappings;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the discount type identifier
        /// </summary>
        public int DiscountTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use percentage
        /// </summary>
        public bool UsePercentage { get; set; }

        /// <summary>
        /// Gets or sets the discount percentage
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Gets or sets the discount amount
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the maximum discount amount (used with "UsePercentage")
        /// </summary>
        public decimal? MaximumDiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the discount start date and time
        /// </summary>
        public DateTime? StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the discount end date and time
        /// </summary>
        public DateTime? EndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether discount requires coupon code
        /// </summary>
        public bool RequiresCouponCode { get; set; }

        /// <summary>
        /// Gets or sets the coupon code
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether discount can be used simultaneously with other discounts (with the same discount type)
        /// </summary>
        public bool IsCumulative { get; set; }

        /// <summary>
        /// Gets or sets the discount limitation identifier
        /// </summary>
        public int DiscountLimitationId { get; set; }

        /// <summary>
        /// Gets or sets the discount limitation times (used when Limitation is set to "N Times Only" or "N Times Per Customer")
        /// </summary>
        public int LimitationTimes { get; set; }

        /// <summary>
        /// Gets or sets the maximum product quantity which could be discounted
        /// Used with "Assigned to products" or "Assigned to categories" type
        /// </summary>
        public int? MaximumDiscountedQuantity { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether it should be applied to all subcategories or the selected one
        /// Used with "Assigned to categories" type only.
        /// </summary>
        public bool AppliedToSubCategories { get; set; }

        /// <summary>
        /// Gets or sets the discount type
        /// </summary>
        public DiscountType DiscountType
        {
            get => (DiscountType)DiscountTypeId;
            set => DiscountTypeId = (int)value;
        }

        /// <summary>
        /// Gets or sets the discount limitation
        /// </summary>
        public DiscountLimitationType DiscountLimitation
        {
            get => (DiscountLimitationType)DiscountLimitationId;
            set => DiscountLimitationId = (int)value;
        }

        /// <summary>
        /// Gets or sets the discount requirement
        /// </summary>
        public virtual ICollection<DiscountRequirement> DiscountRequirements
        {
            get => _discountRequirements ?? (_discountRequirements = new List<DiscountRequirement>());
            protected set => _discountRequirements = value;
        }

        /// <summary>
        /// Gets or sets the discount-category mappings
        /// </summary>
        public virtual ICollection<DiscountCategoryMapping> DiscountCategoryMappings
        {
            get => _discountCategoryMappings ?? (_discountCategoryMappings = new List<DiscountCategoryMapping>());
            protected set => _discountCategoryMappings = value;
        }

        /// <summary>
        /// Gets or sets the discount-manufacturer mappings
        /// </summary>
        public virtual ICollection<DiscountManufacturerMapping> DiscountManufacturerMappings
        {
            get => _discountManufacturerMappings ?? (_discountManufacturerMappings = new List<DiscountManufacturerMapping>());
            protected set => _discountManufacturerMappings = value;
        }

        /// <summary>
        /// Gets or sets the discount-product mappings
        /// </summary>
        public virtual ICollection<DiscountProductMapping> DiscountProductMappings
        {
            get => _discountProductMappings ?? (_discountProductMappings = new List<DiscountProductMapping>());
            protected set => _discountProductMappings = value;
        }
    }
}
