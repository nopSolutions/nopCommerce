using System;
using Nop.Core.Domain.Discounts;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 产品卡券销售绑定关系
    /// </summary>
    public partial class ProductSupplierVoucherCouponMapping : BaseEntity
    {
        /// <summary>
        /// 绑定的产品ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 【ProductAttributeValue.Id】产品不同规格价格调整ID
        /// </summary>
        public int? ProductAttributeValueId { get; set; }
        /// <summary>
        /// 【SupplierVoucherCoupon.Id】
        /// </summary>
        public int SupplierVoucherCouponId { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        public int? CustomerRoleId { get; set; }
        /// <summary>
        /// Store
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// 使用限制：0=不限制，15=绑定的产品限制总次数，25=每个用户限制总次数
        /// </summary>
        public byte DiscountLimitationTypeId { get; set; }
        /// <summary>
        /// 使用限制：0=不限制，15=绑定的产品限制总次数，25=每个用户限制总次数
        /// </summary>
        public DiscountLimitationType DiscountLimitationType
        {
            get => (DiscountLimitationType)DiscountLimitationTypeId;
            set => DiscountLimitationTypeId = (byte)value;
        }
        /// <summary>
        /// 限制次数
        /// </summary>
        public int LimitTimes { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }

    }
}
