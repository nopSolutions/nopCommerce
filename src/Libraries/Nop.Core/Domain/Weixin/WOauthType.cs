namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// Represents a OauthType
    /// </summary>
    public enum WOauthType : byte
    {
        //微信授权方式，0=不授权，1=snsapi_base，2=snsapi_userinfo，3=user_login
        //不授权
        None = 0,
        //静默授权
        SnsapiBase = 1,
        //基础信息授权
        SnsapiUserInfo = 2,
        //登录授权
        UserLogin = 3,
    }
}
