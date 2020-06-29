namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// 二维码跳转类型
    /// </summary>
    public enum WQrCodeJumpType : byte
    {
        //二维码内容类型
        /// <summary>
        /// 关注类型（永久二维码或临时二维码）
        /// </summary>
        Subscribe = 1,
        /// <summary>
        /// URL类型（URL生成二维码，扫码跳转到对应链接）
        /// </summary>
        Url = 2,
    }
}
