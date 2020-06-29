namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WMessageAutoReply
    /// </summary>
    public partial class WMessageAutoReply : BaseEntity
    {
        /// <summary>
        /// 【WConfig.Id】
        /// </summary>
        public int WConfigId { get; set; }
        /// <summary>
        /// 关注后自动回复是否开启，0代表未开启，1代表开启
        /// </summary>
        public bool IsAddFriendReplyOpen { get; set; }
        /// <summary>
        /// 收到消息自动回复是否开启，0代表未开启，1代表开启
        /// </summary>
        public bool IsAutoreplyOpen { get; set; }
        /// <summary>
        /// 关注时自动回复的类型。关注后自动回复和消息自动回复的类型仅支持文本（text）、图片（img）、语音（voice）、视频（video），关键词自动回复则还多了图文消息（news）
        /// </summary>
        public byte AddFriendAutoreplyTypeId { get; set; }
        /// <summary>
        /// 关注回复类型
        /// </summary>
        public AutoReplyType AddFriendAutoreplyType
        {
            get => (AutoReplyType)AddFriendAutoreplyTypeId;
            set => AddFriendAutoreplyTypeId = (byte)value;
        }
        /// <summary>
        /// 【AutoreplyType】对于文本类型，content是文本内容，对于图文、图片、语音、视频类型，content是mediaID
        /// </summary>
        public string AddFriendAutoreplyContent { get; set; }
        /// <summary>
        /// 【AutoreplyType】默认自动回复的类型。关注后自动回复和消息自动回复的类型仅支持文本（text）、图片（img）、语音（voice）、视频（video），关键词自动回复则还多了图文消息（news）
        /// </summary>
        public byte AutoreplyTypeId { get; set; }
        /// <summary>
        /// 默认自动回复的类型
        /// </summary>
        public AutoReplyType AutoreplyType
        {
            get => (AutoReplyType)AutoreplyTypeId;
            set => AutoreplyTypeId = (byte)value;
        }
        /// <summary>
        /// 对于文本类型，content是文本内容，对于图文、图片、语音、视频类型，content是mediaID
        /// </summary>
        public string AutoreplyContent { get; set; }

    }
}
