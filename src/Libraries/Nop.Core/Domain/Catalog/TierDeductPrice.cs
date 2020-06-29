using System;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a Tier Deduct Price:满减/满折
    /// </summary>
    public partial class TierDeductPrice : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier (0 - all stores)
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        public int? CustomerRoleId { get; set; }

        /// <summary>
        /// 减少价格
        /// </summary>
        public decimal DeductPrice { get; set; }
        /// <summary>
        /// 满减价格阶梯条件
        /// </summary>
        public decimal TierAmount { get; set; }
        /// <summary>
        /// 满减或每满减（循环计算，不是只减一次）
        /// </summary>
        public bool UsedRepeately { get; set; }

        /// <summary>
        /// 仅新用户使用
        /// </summary>
        public bool NewUsersOnly { get; set; }

        /// <summary>
        /// Gets or sets the start date and time in UTC
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the end date and time in UTC
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }
    }
}
