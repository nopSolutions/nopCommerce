using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Web.Extensions;
using Nop.Web.Models;

namespace Nop.Web.Controllers
{
    public class HomeController : BaseNopController
    {
        private readonly ILanguageService _languageService;
        private IWorkContext _workContext;

        public HomeController(ILanguageService languageService, IWorkContext workContext)
        {
            _workContext = workContext;
            _languageService = languageService;
        }

        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult LanguageSelector()
        {
            var model = new LanguageSelectorModel();
            var avaibleLanguages = _languageService.GetAllLanguages();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvaibleLanguages = avaibleLanguages.Select(x=> x.ToModel()).ToList();
            return PartialView(model);
        }

        public ActionResult LanguageSelected(int id)
        {
            var language = _languageService.GetLanguageById(id);
            if(language != null)
            {
                _workContext.WorkingLanguage = language;
            }
            var model = new LanguageSelectorModel();
            var avaibleLanguages = _languageService.GetAllLanguages();
            model.CurrentLanguage = _workContext.WorkingLanguage.ToModel();
            model.AvaibleLanguages = avaibleLanguages.Select(x => x.ToModel()).ToList();
            model.IsAjaxRequest = true;
            return PartialView("LanguageSelector", model);
        }
    }
}
