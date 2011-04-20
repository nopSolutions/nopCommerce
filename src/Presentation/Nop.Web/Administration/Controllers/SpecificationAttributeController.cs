using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class SpecificationAttributeController : BaseNopController
    {
        #region Fields

        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;

        #endregion Fields

        #region Constructors

        public SpecificationAttributeController(ISpecificationAttributeService specificationAttributeService,
            ILanguageService languageService, ILocalizedEntityService localizedEntityService)
        {
            this._specificationAttributeService = specificationAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
        }

        #endregion Constructors
        
        #region Utilities

        [NonAction]
        public void UpdateLocales(SpecificationAttribute specificationAttribute, SpecificationAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(specificationAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
            }
        }


        #endregion
        
        #region Methods

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var specificationAttributes = _specificationAttributeService.GetSpecificationAttributes();
            var gridModel = new GridModel<SpecificationAttributeModel>
            {
                Data = specificationAttributes.Select(x => x.ToModel()),
                Total = specificationAttributes.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var specificationAttributes = _specificationAttributeService.GetSpecificationAttributes();
            var gridModel = new GridModel<SpecificationAttributeModel>
            {
                Data = specificationAttributes.Select(x => x.ToModel()),
                Total = specificationAttributes.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        
        //create
        public ActionResult Create()
        {
            var model = new SpecificationAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(SpecificationAttributeModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var specificationAttribute = model.ToEntity();
                _specificationAttributeService.InsertSpecificationAttribute(specificationAttribute);
                UpdateLocales(specificationAttribute, model);

                return continueEditing ? RedirectToAction("Edit", new { id = specificationAttribute.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);
            if (specificationAttribute == null)
                throw new ArgumentException("No specification attribute found with the specified id", "id");
            var model = specificationAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = specificationAttribute.GetLocalized(x => x.Name, languageId, false);
            });

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(SpecificationAttributeModel model, bool continueEditing)
        {
            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(model.Id);
            if (specificationAttribute == null)
                throw new ArgumentException("No specification attribute found with the specified id");
            if (ModelState.IsValid)
            {
                specificationAttribute = model.ToEntity(specificationAttribute);
                _specificationAttributeService.UpdateSpecificationAttribute(specificationAttribute);

                UpdateLocales(specificationAttribute, model);

                return continueEditing ? RedirectToAction("Edit", specificationAttribute.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);
            _specificationAttributeService.DeleteSpecificationAttribute(specificationAttribute);
            return RedirectToAction("List");
        }

        #endregion
    }
}
