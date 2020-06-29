namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a 回复的类型
    /// </summary>
    public enum AutoReplyType : byte
    {
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 图片
        /// </summary>
        Img,
        /// <summary>
        /// 语音
        /// </summary>
        Voice,
        /// <summary>
        /// 视频
        /// </summary>
        Video,
        /// <summary>
        /// 关键词自动回复则还多了图文消息
        /// </summary>
        News
    }
}
