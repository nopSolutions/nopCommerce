using System;
using Nop.Core.Domain.Marketing;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 卡券使用访问绑定表
    /// </summary>
    public partial class SupplierVoucherCouponAppliedValue : BaseEntity
    {
        /// <summary>
        /// 值
        /// </summary>
        public int PromotionUseScopeValue { get; set; }

        /// <summary>
        /// 【SupplierVoucherCoupon.Id】
        /// </summary>
        public int SupplierVoucherCouponId { get; set; }

        /// <summary>
        /// 【PromotionUseScopeType】使用范围：跨站点，站点内，频道内，商品类别，店铺内，指定商品（用于筛选）
        /// </summary>
        public byte PromotionUseScopeTypeId { get; set; }

        public PromotionUseScopeType PromotionUseScopeType
        {
            get => (PromotionUseScopeType)PromotionUseScopeTypeId;
            set => PromotionUseScopeTypeId = (byte)value;
        }

    }
}
