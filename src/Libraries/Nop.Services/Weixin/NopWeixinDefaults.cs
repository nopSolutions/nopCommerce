using Nop.Core.Caching;

namespace Nop.Services.Weixin
{
    /// <summary>
    /// Represents default values related to Weixin services
    /// </summary>
    public static partial class NopWeixinDefaults
    {
        #region Products

        /// <summary>
        /// 头像链接地址
        /// {0}：mmopen 后链接参数，不包括“/”及后面数字
        /// {1}：头像大小，有0、46、64、96、132数值可选，0代表640*640正方形头像
        /// </summary>
        public static string HeadImageUrl => "http://thirdwx.qlogo.cn/mmopen/{0}/{1}";
        /// <summary>
        /// 预检查浏览器后，跳转的control-action名称
        /// </summary>
        public static string WechatBrowserControler => "WechatBrowser";
        /// <summary>
        /// 微信Oauth回调地址的routName值
        /// </summary>
        public static string WeixinOauthCallbackControler => "OAuth2";

        /// <summary>
        /// 保存SESSION：weixin oauth信息
        /// </summary>
        public static string WeixinOauthSession => "WeixinOauthSession";
        /// <summary>
        /// 保存 Oauth Code 字符串到session中，只使用一次。
        /// 组合方式为base/userinfo_随机数
        /// </summary>
        public static string WeixinOauthStateString => "WeixinOauthState";

        #endregion

        #region Caching defaults

        #endregion
    }
}