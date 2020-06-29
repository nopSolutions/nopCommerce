using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 用户广告来源渠道统计表
    /// </summary>
    public partial class UserAdvertChannelAnalysis : BaseEntity
    {
        /// <summary>
        /// 新增用户的ID（不是推荐人ID）
        /// </summary>
        public int WUserId { get; set; }
        /// <summary>
        /// 供应商ID（用于统计）
        /// </summary>
        public int SupplierId { get; set; }
        /// <summary>
        /// 店铺ID（用于统计）
        /// </summary>
        public int SupplierShopId { get; set; }
        /// <summary>
        /// 所属产品ID（为回复消息传递必要参数）
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 宣传地点（用于统计）
        /// </summary>
        public int MarketingAdvertAddressId { get; set; }
        /// <summary>
        /// 单独表：易拉宝，店铺贴广告图，线上分享，DM单,个人名片……
        /// </summary>
        public int MarketingAdvertWayId { get; set; }
        /// <summary>
        /// 产品广告图ID，统计广告图效果
        /// </summary>
        public int ProductAdvertImageId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreatTime { get; set; }


    }
}
