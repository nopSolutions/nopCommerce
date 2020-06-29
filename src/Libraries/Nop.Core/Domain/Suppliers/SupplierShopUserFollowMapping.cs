using System;
using Humanizer;

namespace Nop.Core.Domain.Suppliers
{
    /// <summary>
    /// 店铺关注人映射表
    /// </summary>
    public partial class SupplierShopUserFollowMapping : BaseEntity
    {
        /// <summary>
        /// 店铺ID
        /// </summary>
        public int SupplierShopId { get; set; }
            /// <summary>
            /// 关注人ID
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



    }
}
