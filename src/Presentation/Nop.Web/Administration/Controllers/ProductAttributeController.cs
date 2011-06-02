using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Security.Permissions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductAttributeController : BaseNopController
    {
        #region Fields

        private readonly IProductAttributeService _productAttributeService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;

        #endregion Fields

        #region Constructors

        public ProductAttributeController(IProductAttributeService productAttributeService,
            ILanguageService languageService, ILocalizedEntityService localizedEntityService)
        {
            this._productAttributeService = productAttributeService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
        }

        #endregion Constructors
        
        #region Utilities

        [NonAction]
        public void UpdateLocales(ProductAttribute productAttribute, ProductAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productAttribute,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(productAttribute,
                                                           x => x.Description,
                                                           localized.Description,
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
            var productAttributes = _productAttributeService.GetAllProductAttributes();
            var gridModel = new GridModel<ProductAttributeModel>
            {
                Data = productAttributes.Select(x => x.ToModel()),
                Total = productAttributes.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var productAttributes = _productAttributeService.GetAllProductAttributes();
            var gridModel = new GridModel<ProductAttributeModel>
            {
                Data = productAttributes.Select(x => x.ToModel()),
                Total = productAttributes.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        
        //create
        public ActionResult Create()
        {
            var model = new ProductAttributeModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Create(ProductAttributeModel model, bool continueEditing)
        {
            //decode description
            model.Description = HttpUtility.HtmlDecode(model.Description);
            foreach (var localized in model.Locales)
                localized.Description = HttpUtility.HtmlDecode(localized.Description);

            if (ModelState.IsValid)
            {
                var productAttribute = model.ToEntity();
                _productAttributeService.InsertProductAttribute(productAttribute);
                UpdateLocales(productAttribute, model);

                return continueEditing ? RedirectToAction("Edit", new { id = productAttribute.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            var productAttribute = _productAttributeService.GetProductAttributeById(id);
            if (productAttribute == null)
                throw new ArgumentException("No product attribute found with the specified id", "id");
            var model = productAttribute.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = productAttribute.GetLocalized(x => x.Name, languageId, false);
                locale.Description = productAttribute.GetLocalized(x => x.Description, languageId, false);
            });

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(ProductAttributeModel model, bool continueEditing)
        {
            var productAttribute = _productAttributeService.GetProductAttributeById(model.Id);
            if (productAttribute == null)
                throw new ArgumentException("No product attribute found with the specified id");
            
            //decode description
            model.Description = HttpUtility.HtmlDecode(model.Description);
            foreach (var localized in model.Locales)
                localized.Description = HttpUtility.HtmlDecode(localized.Description);

            if (ModelState.IsValid)
            {
                productAttribute = model.ToEntity(productAttribute);
                _productAttributeService.UpdateProductAttribute(productAttribute);

                UpdateLocales(productAttribute, model);

                return continueEditing ? RedirectToAction("Edit", productAttribute.Id) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var productAttribute = _productAttributeService.GetProductAttributeById(id);
            _productAttributeService.DeleteProductAttribute(productAttribute);
            return RedirectToAction("List");
        }

        #endregion
    }
}
