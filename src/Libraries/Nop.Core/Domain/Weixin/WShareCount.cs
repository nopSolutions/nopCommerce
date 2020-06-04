namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WShareCount
    /// </summary>
    public partial class WShareCount : BaseEntity
    {
        /// <summary>
        /// WShareLink.Id值
        /// </summary>
        public int WShareLinkId { get; set; }
        /// <summary>
        /// 阅读用户OpenId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 是否点赞，默认=false
        /// </summary>
        public bool IsThumbUp { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// 是否无效
        /// </summary>
        public bool Invalid { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public int CreatTime { get; set; }

    }
}
