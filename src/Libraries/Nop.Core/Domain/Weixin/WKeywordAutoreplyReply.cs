namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WKeywordAutoreply
    /// </summary>
    public partial class WKeywordAutoreplyReply : BaseEntity
    {
        /// <summary>
        /// 【WKeywordAutoreply.Id】
        /// </summary>
        public int WKeywordAutoreplyId { get; set; }
        /// <summary>
        /// 【AutoreplyType】自动回复的类型。关注后自动回复和消息自动回复的类型仅支持文本（text）、图片（img）、语音（voice）、视频（video），关键词自动回复则还多了图文消息（news）
        /// </summary>
        public byte AutoreplyTypeId { get; set; }
        /// <summary>
        /// 匹配方式
        /// </summary>
        public AutoReplyType AutoReplyType
        {
            get => (AutoReplyType)AutoreplyTypeId;
            set => AutoreplyTypeId = (byte)value;
        }
        /// <summary>
        /// 对于文本类型，content是文本内容，对于图文、图片、语音、视频类型，content是mediaID
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 【WAutoreplyNewsInfo】Id列表，以逗号分开，多条图文，首个Id为显示封面
        /// </summary>
        public string WAutoreplyNewsInfoIds { get; set; }

    }
}
