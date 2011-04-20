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

namespace Nop.Admin.Controllers
{
    public class ProductVariantController : BaseNopController
    {
        private IProductService _productService;
        private ILanguageService _languageService;
        private ILocalizedEntityService _localizedEntityService;

        public ProductVariantController(IProductService productService, ILanguageService languageService, ILocalizedEntityService localizedEntityService)
        {
            _localizedEntityService = localizedEntityService;
            _languageService = languageService;
            _productService = productService;
        }

        #region Edit

        public ActionResult Edit(int id)
        {
            var variant = _productService.GetProductVariantById(id);
            if (variant == null) throw new ArgumentException("No product variant found with the specified id", "id");
            var model = variant.ToModel();
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name =
                    variant.GetLocalized(x => x.Name,
                                            languageId, false);
                locale.Description =
                    variant.GetLocalized(x => x.Description,
                                            languageId, false);
            });
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(ProductVariantModel productVariantModel, bool continueEditing)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", productVariantModel);
            }

            var variant = _productService.GetProductVariantById(productVariantModel.Id);
            variant = productVariantModel.ToEntity(variant);
            _productService.UpdateProductVariant(variant);
            UpdateLocales(variant, productVariantModel);

            return continueEditing ? RedirectToAction("Edit", productVariantModel.Id) : RedirectToAction("List", "Product");
        }

        #endregion

        #region Saving/Updating/Inserting

        public void UpdateLocales(ProductVariant variant, ProductVariantModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(variant,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(variant,
                                                               x => x.Description,
                                                               localized.Description,
                                                               localized.LanguageId);
            }
        }

        #endregion
    }
}
