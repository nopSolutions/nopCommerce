using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Nop.Core.Http.Extensions;
using Senparc.CO2NET;
using Senparc.CO2NET.Trace;
using Senparc.Weixin.MP.CommonService.Mvc.Extension.Filters;

namespace Senparc.Weixin.MP.CommonService.Mvc.Filters
{
    public class CustomOAuthAttribute : SenparcOAuthAttribute
    {
        public CustomOAuthAttribute(string appId, string oauthCallbackUrl) : base(appId, oauthCallbackUrl)
        {
            //如果是多租户，appId 可以传入 null，并且忽略下一行，使用 IsLogined() 方法中的动态赋值语句
            //填写公众号AppId（适用于公众号、微信支付、JsApi等）
            base._appId ??= Config.SenparcWeixinSetting.TenPayV3_AppId;
        }

        public override bool IsLogined(HttpContext httpContext)
        {
            //如果是多租户，也可以这样写，通过 URL 参数来区分：
            //base._appId = httpContext.Request.Query["appId"].FirstOrDefault();//appId也可以是数据库存储的Id，避免暴露真实的AppId


            //.NET Core 3.0 中，Attribute 似乎会在系统初始化的时候进行第一次初始化（执行构造函数），而不是在第一次需要被访问到的时候。
            //因此，构造函数中的 appId 等默认值未必可以有赋值（当时 其他对象都还没有值），这里为了确保 appId 有值，再次进行判断。
            base._appId ??= Config.SenparcWeixinSetting.TenPayV3_AppId;
            
            return httpContext != null && httpContext.Session.GetString("OpenId") != null;
            
            //也可以使用其他方法如Session验证用户登录
            //return httpContext != null && httpContext.User.Identity.IsAuthenticated;
        }
    }
}
