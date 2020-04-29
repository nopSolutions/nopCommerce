namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a CheckInType
    /// </summary>
    public enum WCheckInType : byte
    {
        //未知
        None = 0,
        //通过Subscribe关注公众号方式登记
        Subscribe = 1,
        //通过网页Oauth授权登记
        Oauth = 2,
        //通过Login授权方式登记
        Login = 3,
    }
}
