namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a SceneType
    /// </summary>
    public enum WSceneType : byte
    {
        //场景类型
        //未知(默认)
        None = 0,
        //广告类型二维码
        Adver = 1,
        //验证类型二维码
        Verify = 2,
        //通过Login授权方式登记
        Command = 3,
        //投票类型二维码
        Vote = 4,
        //名片，身份证类型二维码
        IDCard = 5,
    }
}
