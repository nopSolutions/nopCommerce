using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;

namespace Nop.Admin.Controllers
{
    public class ProductVariantController : BaseNopController
    {
        private IProductService _productService;
        private ILanguageService _languageService;

        public ProductVariantController(IProductService productService, ILanguageService languageService)
        {
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

            //var product = _productService.GetProductById(productModel.Id);
            //product = productModel.ToEntity(product);
            //_productService.UpdateProduct(product);

            //UpdateLocales(product, productModel);

            return continueEditing ? RedirectToAction("Edit", productVariantModel.Id) : RedirectToAction("List");
        }

        #endregion
    }
}
