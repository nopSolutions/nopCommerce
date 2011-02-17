
using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount
    /// </summary>
    public partial class Discount : BaseEntity
    {
        public Discount()
        {
            this.DiscountRequirements = new List<DiscountRequirement>();
            this.AppliedToCategories = new List<Category>();
            this.AppliedToProductVariants = new List<ProductVariant>();
            this.DiscountUsageHistory = new List<DiscountUsageHistory>();
        }

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
        /// Gets or sets the discount limitation identifier
        /// </summary>
        public int DiscountLimitationId { get; set; }

        /// <summary>
        /// Gets or sets the discount limitation times (used when Limitation is set to "N Times Only" or "N Times Per Customer")
        /// </summary>
        public int LimitationTimes { get; set; }
        
        /// <summary>
        /// Gets or sets the discount type
        /// </summary>
        public DiscountType DiscountType
        {
            get
            {
                return (DiscountType)this.DiscountTypeId;
            }
            set
            {
                this.DiscountTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the discount limitation
        /// </summary>
        public DiscountLimitationType DiscountLimitation
        {
            get
            {
                return (DiscountLimitationType)this.DiscountLimitationId;
            }
            set
            {
                this.DiscountLimitationId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the discount requirement
        /// </summary>
        public virtual ICollection<DiscountRequirement> DiscountRequirements { get; set; }

        /// <summary>
        /// Gets or sets the categories
        /// </summary>
        public virtual ICollection<Category> AppliedToCategories { get; set; }
        
        /// <summary>
        /// Gets or sets the product variants 
        /// </summary>
        public virtual ICollection<ProductVariant> AppliedToProductVariants { get; set; }

        /// <summary>
        /// Gets or sets discount usage history
        /// </summary>
        public virtual ICollection<DiscountUsageHistory> DiscountUsageHistory { get; set; }

        #region Methods

        /// <summary>
        /// Gets the discount amount for the specified value
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>The discount amount</returns>
        public decimal GetDiscountAmount(decimal amount)
        {
            decimal result = decimal.Zero;
            if (this.UsePercentage)
                result = (decimal)((((float)amount) * ((float)this.DiscountPercentage)) / 100f);
            else
                result = this.DiscountAmount;

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        #endregion
    }
}
