namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a 回复模式
    /// </summary>
    public enum AutoReplyModeType : byte
    {
        /// <summary>
        /// 代表全部回复
        /// </summary>
        Reply_all = 0,
        /// <summary>
        /// 代表随机回复其中一条
        /// </summary>
        Random_one = 1,
    }
}
