namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a QrCodeJumpType
    /// </summary>
    public enum WQrCodeJumpType : byte
    {
        //二维码内容类型
        //关注公众号类型二维码
        Subscribe = 1,
        //URL形式二维码
        Url = 2,
    }
}
