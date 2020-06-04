using System;
using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 产品用户关注信息
    /// </summary>
    public partial class ProductUserFollowMapping : BaseEntity
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 微信用户ID，关注人ID
        /// </summary>
        public int WUserId { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public int LastUpdateTime { get; set; }
        /// <summary>
        /// 是否关注（主要用于限制访客不停刷关注状态）
        /// </summary>
        public bool Subscribe { get; set; }
        /// <summary>
        /// 降价提醒
        /// </summary>
        public bool PriceNotice { get; set; }
        /// <summary>
        /// 有货通知
        /// </summary>
        public bool AvailableNotice { get; set; }

    }
}
