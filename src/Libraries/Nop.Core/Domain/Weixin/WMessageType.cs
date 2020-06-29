namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a MessageType
    /// </summary>
    public enum WMessageType : byte
    {
        /// <summary>
        /// 不回复任何消息SUCCESS
        /// </summary>
        None = 0,
        /// <summary>
        /// 文本
        /// </summary>
        Text = 1,
        /// <summary>
        /// 图片
        /// </summary>
        Image = 2,
        /// <summary>
        /// 声音
        /// </summary>
        Voice = 3,
        /// <summary>
        /// 视频
        /// </summary>
        Video = 4,
        /// <summary>
        /// 音乐
        /// </summary>
        Music = 5,
        /// <summary>
        /// 图文消息（点击跳转到外链）
        /// </summary>
        News = 6,
        /// <summary>
        /// 图文消息（点击跳转到图文消息页面）
        /// </summary>
        MpNews = 7,
        /// <summary>
        /// 卡券消息
        /// </summary>
        WxCard = 8,
        /// <summary>
        /// 小程序消息
        /// </summary>
        MiniProgramPage = 9,
        /// <summary>
        /// 链接形式消息
        /// </summary>
        Link = 10,
    }
}
