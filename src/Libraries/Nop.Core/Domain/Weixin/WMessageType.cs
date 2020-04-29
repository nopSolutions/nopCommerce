namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a MessageType
    /// </summary>
    public enum WMessageType : byte
    {
        None = 0,
        Text = 1,
        Image = 2,
        Voice = 3,
        Video = 4,
        Music = 5,
        News = 6,
        //图文消息
        MpNews = 7,
        //卡券消息
        WxCard = 8,
        //小程序消息
        MiniProgramPage = 9,
    }
}
