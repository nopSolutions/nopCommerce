using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a general and common settings model
    /// </summary>
    public partial class WeixinSettingsModel : BaseNopModel, ISettingsModel
    {
        #region Properties
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Weixin.ForcedAccessWeChatBrowser")]
        public bool ForcedAccessWeChatBrowser { get; set; }
        public bool ForcedAccessWeChatBrowser_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Weixin.CheckWebBrowser")]
        public bool CheckWebBrowser { get; set; }
        public bool CheckWebBrowser_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Weixin.UseSnsapiBase")]
        public bool UseSnsapiBase { get; set; }
        public bool UseSnsapiBase_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Weixin.Debug")]
        public bool Debug { get; set; }
        public bool Debug_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Weixin.TraceLog")]
        public bool TraceLog { get; set; }
        public bool TraceLog_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Weixin.JSSDKDebug")]
        public bool JSSDKDebug { get; set; }
        public bool JSSDKDebug_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Settings.Weixin.JsApiList")]
        public string JsApiList { get; set; }
        public bool JsApiList_OverrideForStore { get; set; }

        #endregion
    }
}