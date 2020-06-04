namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WJSDKShare
    /// </summary>
    public partial class WJSDKShare : BaseEntity
    {
        /// <summary>
        /// 产品列表ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 分享标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 分享描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 分享链接，该链接域名或路径必须与当前页面对应的公众号JS安全域名一致
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 分享图标
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 分享类型,music、video或link，不填默认为link
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 如果type是music或video，则要提供数据链接，默认为空
        /// </summary>
        public string DataUrl { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreatTime { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Deleted { get; set; }
      

    }
}
