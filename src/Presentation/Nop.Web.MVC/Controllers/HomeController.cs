using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Localization;
using Nop.Services.Security.Permissions;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILanguageService _languageService;

        public HomeController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult List()
        {
            return View();
        }
    }
}
