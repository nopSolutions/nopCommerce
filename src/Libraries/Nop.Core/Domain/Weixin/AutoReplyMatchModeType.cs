namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a 匹配模式
    /// </summary>
    public enum AutoReplyMatchModeType : byte
    {
        /// <summary>
        /// 代表消息中含有该关键词即可
        /// </summary>
        Contain = 0,
        /// <summary>
        /// 表示消息内容必须和关键词严格相同
        /// </summary>
        Equal = 1,
    }
}
