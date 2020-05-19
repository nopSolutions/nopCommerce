using Nop.Core.Configuration;

namespace Nop.Core.Domain.Weixin
{
    /// <summary>
    /// GDPR settings
    /// </summary>
    public class WeixinSettings : ISettings
    {
        /// <summary>
        /// 强制在微信浏览器中使用
        /// </summary>
        public bool ForcedAccessWeChatBrowser { get; set; }
        /// <summary>
        /// 是否预先检查微信浏览器
        /// </summary>
        public bool CheckWebBrowser { get; set; }

        /// <summary>
        /// 前端页面是否使用SnsapiBase模式
        /// </summary>
        public bool UseSnsapiBase { get; set; }
    }
}