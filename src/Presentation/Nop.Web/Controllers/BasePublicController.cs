<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    [WwwRequirement]
    [CheckLanguageSeoCode]
    [CheckAccessPublicStore]
    [CheckAccessClosedStore]
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
=======
﻿using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    [WwwRequirement]
    [CheckLanguageSeoCode]
    [CheckAccessPublicStore]
    [CheckAccessClosedStore]
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
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}