using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductController : BaseNopController
    {
		#region Fields

        private IProductService _productService;
        private IWorkContext _workContext;
        private ILanguageService _languageService;
        private ILocalizedEntityService _localizedEntityService;

        #endregion Fields 

		#region Constructors

        public ProductController(IProductService productService, IWorkContext workContext, ILanguageService languageService, ILocalizedEntityService localizedEntityService)
        {
            this._localizedEntityService = localizedEntityService;
            this._languageService = languageService;
            this._workContext = workContext;
            this._productService = productService;
        }

        #endregion Constructors 

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var products = _productService.SearchProducts(0, 0, null, null, null, 0, 0, string.Empty, false,
                                                            _workContext.WorkingLanguage.Id, new List<int>(),
                                                            Core.Domain.Catalog.ProductSortingEnum.Position, 0, 10, true);
            var gridModel = new GridModel<ProductModel>
                                {
                                    Data = products.Select(x => x.ToModel()),
                                    Total = products.TotalCount
                                };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var model = new GridModel();
            var products = _productService.SearchProducts(0, 0, null, null, null, 0, 0, string.Empty, false,
                                                          _workContext.WorkingLanguage.Id, new List<int>(),
                                                          Core.Domain.Catalog.ProductSortingEnum.Position,
                                                          command.Page - 1, command.PageSize, true);
            model.Data = products.Select(x => x.ToModel());
            model.Total = products.TotalCount;
            return new JsonResult
            {
                Data = model
            };
        }

        #endregion

        #region Delete

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = _productService.GetProductById(id);
            _productService.DeleteProduct(product);
            return RedirectToAction("List");
        }

        #endregion

        #region Edit

        public ActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null) throw new ArgumentException("No category found with the specified id", "id");
            var model = product.ToModel();
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
                                                            {
                                                                locale.Name = 
                                                                    product.GetLocalized(x => x.Name,
                                                                                                   languageId, false);
                                                                locale.ShortDescription =
                                                                    product.GetLocalized(x => x.ShortDescription,
                                                                                         languageId, false);
                                                                locale.FullDescription =
                                                                    product.GetLocalized(x => x.FullDescription,
                                                                                         languageId, false);
                                                                locale.MetaKeywords =
                                                                    product.GetLocalized(x => x.MetaKeywords, languageId,
                                                                                         false);
                                                                locale.MetaDescription =
                                                                    product.GetLocalized(x => x.MetaDescription,
                                                                                         languageId, false);
                                                                locale.MetaTitle = 
                                                                    product.GetLocalized(x => x.MetaTitle, languageId, false);
                                                                locale.SeName = product.GetLocalized(x => x.SeName,
                                                                                                     languageId, false);
                                                            });
            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        public ActionResult Edit(ProductModel productModel, bool continueEditing)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", productModel);
            }

            var product = _productService.GetProductById(productModel.Id);
            product = productModel.ToEntity(product);
            _productService.UpdateProduct(product);

            UpdateLocales(product, productModel);

            return continueEditing ? RedirectToAction("Edit", product.Id) : RedirectToAction("List");
        }

        #endregion

        #region Create

        public ActionResult Create()
        {
            var model = new ProductModel();
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProductModel model)
        {
            var product = model.ToEntity();
            _productService.InsertProduct(product);
            UpdateLocales(product, model);
            return RedirectToAction("Edit", new { product.Id });
        }

        #endregion

        #region Saving/Updating/Inserting

        public void UpdateLocales(Product product, ProductModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.Language.Id);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.ShortDescription,
                                                               localized.ShortDescription,
                                                               localized.Language.Id);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.FullDescription,
                                                               localized.FullDescription,
                                                               localized.Language.Id);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.MetaKeywords,
                                                               localized.MetaKeywords,
                                                               localized.Language.Id);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.MetaDescription,
                                                               localized.MetaDescription,
                                                               localized.Language.Id);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.MetaTitle,
                                                               localized.MetaTitle,
                                                               localized.Language.Id);
                _localizedEntityService.SaveLocalizedValue(product,
                                                               x => x.SeName,
                                                               localized.SeName,
                                                               localized.Language.Id);
            }
        }

        #endregion

		#region Methods 

        [HttpPost]
        public ActionResult _AjaxComboBo(string text)
        {
            var products = _productService.GetAllProducts(true).Where(x => x.Name.ToLower().Contains(text.ToLower()));

            return new JsonResult
            {
                Data = new SelectList(products.ToList(), "Id",
                    "Name")
            };
        }
        
		#endregion Methods 
    }
}
