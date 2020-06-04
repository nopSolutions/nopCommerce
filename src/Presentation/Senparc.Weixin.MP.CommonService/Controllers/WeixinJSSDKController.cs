using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Helpers;

namespace Senparc.Weixin.MP.CommonService.Controllers
{
    public partial class WeixinJSSDKController : Controller
    {
        private readonly SenparcWeixinSetting _senparcWeixinSetting;
        private readonly IWebHelper _webHelper;

        public WeixinJSSDKController(SenparcWeixinSetting senparcWeixinSetting,
            IWebHelper webHelper
            )
        {
            _senparcWeixinSetting = senparcWeixinSetting;
            _webHelper = webHelper;
        }

        public IActionResult Index()
        {
            var jssdkUiPackage = JSSDKHelper.GetJsSdkUiPackage(_senparcWeixinSetting.WeixinAppId, _senparcWeixinSetting.WeixinAppSecret, _webHelper.GetUrlReferrer());
            //ViewData["JsSdkUiPackage"] = jssdkUiPackage;
            //ViewData["AppId"] = appId;
            //ViewData["Timestamp"] = jssdkUiPackage.Timestamp;
            //ViewData["NonceStr"] = jssdkUiPackage.NonceStr;
            //ViewData["Signature"] = jssdkUiPackage.Signature;

            //wx.config({
            //    debug: true, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
            //    appId: '', // 必填，公众号的唯一标识
            //    timestamp: , // 必填，生成签名的时间戳
            //    nonceStr: '', // 必填，生成签名的随机串
            //    signature: '',// 必填，签名
            //    jsApiList: [] // 必填，需要使用的JS接口列表
            //  });

            return Json(new { Result = true, Url = _webHelper.GetThisPageUrl(true), JssdkPackage = jssdkUiPackage });
        }
    }
}
