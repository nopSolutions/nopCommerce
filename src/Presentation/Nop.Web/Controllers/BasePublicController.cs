using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;

namespace Nop.Web.Controllers
{
    [HttpsRequirement(SslRequirement.NoMatter)]
    [WwwRequirement]
    [CheckAccessPublicStore]
    [CheckAccessClosedStore]
    [CheckLanguageSeoCode]
    [CheckDiscountCoupon]
    [CheckAffiliate]
    public abstract partial class BasePublicController : BaseController
    {
        protected virtual IActionResult InvokeHttp404()
        {
            Response.StatusCode = 404;
            return new EmptyResult();
        }
    }
}