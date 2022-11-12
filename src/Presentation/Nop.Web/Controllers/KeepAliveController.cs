<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public partial class KeepAliveController : Controller
    {
        public virtual IActionResult Index()
        {
            return Content("I am alive!");
        }
    }
=======
﻿using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public partial class KeepAliveController : Controller
    {
        public virtual IActionResult Index()
        {
            return Content("I am alive!");
        }
    }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
}