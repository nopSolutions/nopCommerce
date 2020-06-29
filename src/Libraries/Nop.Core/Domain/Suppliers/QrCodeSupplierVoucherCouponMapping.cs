using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 二维码绑定客户营销卡券
    /// </summary>
    public partial class QrCodeSupplierVoucherCouponMapping : BaseEntity
    {
        /// <summary>
        /// 二维码ID
        /// </summary>
        public int QrCodeId { get; set; }
        /// <summary>
        /// 商家提供的优惠券ID，是折扣广告图才调用该ID（可免费领取类型）
        /// </summary>
        public int SupplierVoucherCouponId { get; set; }
        /// <summary>
        /// 开始发布时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// 结束发布时间
        /// </summary>
        public DateTime? EndDateTime { get; set; }
        /// <summary>
        /// 是否永久二维码类型
        /// </summary>
        public bool QrcodeLimit { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public bool Published { get; set; }
    }
}
