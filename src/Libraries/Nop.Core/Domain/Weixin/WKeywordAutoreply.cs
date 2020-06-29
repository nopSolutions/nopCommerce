namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WKeywordAutoreply
    /// </summary>
    public partial class WKeywordAutoreply : BaseEntity
    {
        /// <summary>
        /// 规则名称，不为空
        /// </summary>
        public string RuleName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// 【AutoReplyModeType】回复模式，reply_all代表全部回复，random_one代表随机回复其中一条
        /// </summary>
        public byte AutoReplyModeTypeId { get; set; }
        /// <summary>
        /// 回复模式
        /// </summary>
        public AutoReplyModeType AutoReplyModeType
        {
            get => (AutoReplyModeType)AutoReplyModeTypeId;
            set => AutoReplyModeTypeId = (byte)value;
        }

    }
}
