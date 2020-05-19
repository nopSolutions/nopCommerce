namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WUserTag
    /// </summary>
    public partial class WUserTag : BaseEntity
    {
        /// <summary>
        /// 平台原始ID，用于区分不同平台
        /// </summary>
        public int? WConfigId { get; set; }
        /// <summary>
        /// 标签ID，由微信分配
        /// </summary>
        public int OfficialId { get; set; }
        /// <summary>
        /// 标签名，UTF8编码（30个字符以内）
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }
        /// <summary>
        /// 此标签下粉丝数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public int UpdateTime { get; set; }
        /// <summary>
        /// 删除标志
        /// </summary>
        public bool Deleted { get; set; }
    }
}
