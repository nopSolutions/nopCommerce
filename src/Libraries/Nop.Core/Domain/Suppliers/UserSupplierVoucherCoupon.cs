using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 允许合作伙伴赠送（仅可赠送免费领取的卡券，需要购买获取的卡券不能赠送）
    /// </summary>
    public partial class UserSupplierVoucherCoupon : BaseEntity
    {
        /// <summary>
        /// 【WUser.Id】合作伙伴ID
        /// </summary>
        public int WUserId { get; set; }

        /// <summary>
        /// 【SupplierVoucherCoupon.Id】ID
        /// </summary>
        public int SupplierVoucherCouponId { get; set; }

    }
}
