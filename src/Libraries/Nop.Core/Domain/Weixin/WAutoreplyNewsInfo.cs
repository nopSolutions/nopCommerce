namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WKeywordAutoreply
    /// </summary>
    public partial class WAutoreplyNewsInfo : BaseEntity
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Digest { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 封面图片的URL
        /// </summary>

        public string CoverUrl { get; set; }
        /// <summary>
        /// 方图/缩微图
        /// </summary>
        public string CoverUrlThumb { get; set; }
        /// <summary>
        /// 正文的URL
        /// </summary>
        public string ContentUrl { get; set; }
        /// <summary>
        /// 原文的URL，若置空则无查看原文入口
        /// </summary>
        public string SourceUrl { get; set; }
        /// <summary>
        /// 是否显示封面，0为不显示，1为显示（是否可用作为封面）
        /// </summary>
        public bool ShowCover { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public bool Published { get; set; }
        /// <summary>
        /// 删除
        /// </summary>
        public bool Deleted { get; set; }


    }
}
