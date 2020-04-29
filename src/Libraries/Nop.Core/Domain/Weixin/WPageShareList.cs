namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WPageShareList
    /// </summary>
    public partial class WPageShareList : BaseEntity
    {
        /// <summary>
        /// 分享用户Openid
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 阅读统计（每次删除【WPageShareCount】时，累计加入）
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 点赞统计（每次删除【WPageShareCount】时，累计加入）
        /// </summary>
        public int ThumbUpCount { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 页面URL路径（去除参数后的url）
        /// </summary>
        public string PagePath { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreatTime { get; set; }

    }
}
