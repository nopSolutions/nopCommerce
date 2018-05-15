using System;
using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Discount (for caching)
    /// </summary>
    [Serializable]
    public class DiscountForCaching
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DiscountTypeId { get; set; }
        public bool UsePercentage { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal? MaximumDiscountAmount { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
        public bool RequiresCouponCode { get; set; }
        public string CouponCode { get; set; }
        public bool IsCumulative { get; set; }
        public int DiscountLimitationId { get; set; }
        public int LimitationTimes { get; set; }
        public int? MaximumDiscountedQuantity { get; set; }
        public bool AppliedToSubCategories { get; set; }

        public DiscountType DiscountType
        {
            get { return (DiscountType) DiscountTypeId; }
            set { DiscountTypeId = (int) value; }
        }

        public DiscountLimitationType DiscountLimitation
        {
            get { return (DiscountLimitationType) DiscountLimitationId; }
            set { DiscountLimitationId = (int) value; }
        }
    }
}
