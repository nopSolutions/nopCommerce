using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.MVC.Areas.Admin.Models;
using Telerik.Web.Mvc;

namespace Nop.Web.MVC.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class LanguageController : Controller
    {
        private readonly ILanguageService _languageService;
        private ILocalizationService _localizationService;

        public LanguageController(ILanguageService languageService, ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            _languageService = languageService;
        }


        public ActionResult Index()
        {
            return View("List");
        }

        #region Languages

        #region List

        public ActionResult List()
        {
            //if (!_permissionService.Authorize(CatalogPermissionProvider.ManageCategories))
            //{
            //TODO redirect to access denied page
            //}

            var languages = _languageService.GetAllLanguages(true);
            var gridModel = new GridModel<LanguageModel>
            {
                Data = languages.Select(x => new LanguageModel(x)),
                Total = languages.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var languages = _languageService.GetAllLanguages(true);
            var gridModel = new GridModel<LanguageModel>
            {
                Data = languages.Select(x => new LanguageModel(x)),
                Total = languages.Count()
            };

            //var categories = _languageService.GetAllCategories(command.Page - 1, command.PageSize);
            //model.Data = categories.Select(x =>
            //    new { Id = Url.Action("Edit", new { x.Id }), x.Name, x.DisplayOrder, Breadcrumb = GetCategoryBreadCrumb(x), x.Published });
            //model.Total = categories.TotalCount;
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion

        #region Edit

        public ActionResult Edit(int id)
        {
            var language = _languageService.GetLanguageById(id);
            if (language == null) throw new ArgumentException("No language found with the specified id", "id");
            return View(new LanguageModel(language));
        }

        [HttpPost]
        public ActionResult Edit(LanguageModel languageModel)
        {
            var language = _languageService.GetLanguageById(languageModel.Id);
            UpdateInstance(language, languageModel);
            _languageService.UpdateLanguage(language);
            return Edit(language.Id);
        }

        #endregion

        #region Delete

        public ActionResult Delete(int id)
        {
            var language = _languageService.GetLanguageById(id);
            if (language == null)
            {
                return List();
            }
            var modal = new LanguageModel(language);
            return Delete(modal);
        }

        public ActionResult Delete(LanguageModel model)
        {
            return PartialView(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var language = _languageService.GetLanguageById(id);
            _languageService.DeleteLanguage(language);
            return RedirectToAction("List");
        }

        #endregion

        #region Create

        public ActionResult Create()
        {
            return View(new LanguageModel());
        }

        [HttpPost]
        public ActionResult Create(LanguageModel model)
        {
            var language = new Language();
            UpdateInstance(language, model);
            _languageService.InsertLanguage(language);
            return RedirectToAction("Edit", new { id = language.Id });
        }

        #endregion

        #endregion

        #region Resources

        public ActionResult Resources(int languageId)
        {
            //if (!_permissionService.Authorize(CatalogPermissionProvider.ManageCategories))
            //{
                //TODO redirect to access denied page
            //}
            var resources = _localizationService.GetAllResourcesByLanguageId(languageId);
            var gridModel = new GridModel<LanguageResourceModel>
            {
                Data = resources.Take(10).Select(x => new LanguageResourceModel(x.Value)),
                Total = resources.Count
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Resources(int languageId, GridCommand command)
        {
            var model = new GridModel<LanguageResourceModel>();
            var resources = _localizationService.GetAllResourcesByLanguageId(languageId);
            model.Data = resources.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize).Select(x => new LanguageResourceModel(x.Value));
            model.Total = resources.Count;
            return new JsonResult
            {
                Data = model
            };
        }

        #endregion

        public void UpdateInstance(Language language, LanguageModel model)
        {
            language.Id = model.Id;
            language.Name = model.Name;
            language.LanguageCulture = model.LanguageCulture;
            language.FlagImageFileName = model.FlagImageFileName;
            language.Published = model.Published;
            language.DisplayOrder = model.DisplayOrder;
        }
    }
}
