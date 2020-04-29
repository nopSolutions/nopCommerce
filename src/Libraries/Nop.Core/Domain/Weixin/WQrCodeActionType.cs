namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a QrCodeActionType
    /// </summary>
    public enum WQrCodeActionType : byte
    {
        //二维码类型
        //1=QR_SCENE为临时的整型参数值
        QR_SCENE = 1,
        //2=QR_STR_SCENE为临时的字符串参数值
        QR_STR_SCENE = 2,
        //3=QR_LIMIT_SCENE为永久的整型参数值
        QR_LIMIT_SCENE = 3,
        //4=QR_LIMIT_STR_SCENE为永久的字符串参数值
        QR_LIMIT_STR_SCENE = 4
    }
}
