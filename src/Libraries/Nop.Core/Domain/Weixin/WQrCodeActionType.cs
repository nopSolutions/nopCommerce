namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// 二维码类型：永久值，永久字符串，临时值，临时字符串
    /// </summary>
    public enum WQrCodeActionType : byte
    {
        /// <summary>
        /// 1=QR_SCENE为临时的整型参数值
        /// </summary>
        QR_SCENE = 1,
        /// <summary>
        /// 2=QR_STR_SCENE为临时的字符串参数值
        /// </summary>
        QR_STR_SCENE = 2,
        /// <summary>
        /// 3=QR_LIMIT_SCENE为永久的整型参数值
        /// </summary>
        QR_LIMIT_SCENE = 3,
        /// <summary>
        /// 4=QR_LIMIT_STR_SCENE为永久的字符串参数值
        /// </summary>
        QR_LIMIT_STR_SCENE = 4
    }
}
