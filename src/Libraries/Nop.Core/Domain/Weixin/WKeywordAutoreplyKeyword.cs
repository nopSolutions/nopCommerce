namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents an WKeywordAutoreply
    /// </summary>
    public partial class WKeywordAutoreplyKeyword : BaseEntity
    {
        /// <summary>
        /// 【WKeywordAutoreply.Id】
        /// </summary>
        public int WKeywordAutoreplyId { get; set; }
        /// <summary>
        /// 匹配方式
        /// </summary>
        public byte MatchModeTypeId { get; set; }
        /// <summary>
        /// 匹配方式
        /// </summary>
        public AutoReplyMatchModeType MatchModeType
        {
            get => (AutoReplyMatchModeType)MatchModeTypeId;
            set => MatchModeTypeId = (byte)value;
        }
        /// <summary>
        /// 默认为0 = text，暂时无其他值
        /// </summary>
        public byte Type { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否展示或发布
        /// </summary>
        public bool Published { get; set; }

    }
}
