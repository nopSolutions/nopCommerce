using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
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
		#region Fields 

        private IProductService _productService;
        private IWorkContext _workContext;
        private ILanguageService _languageService;

        #endregion Fields 

		#region Constructors 

        public ProductController(IProductService productService, IWorkContext workContext, ILanguageService languageService)
        {
            _languageService = languageService;
            _workContext = workContext;
            _productService = productService;
        }

        #endregion Constructors 

        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            //if (!_permissionService.Authorize(CatalogPermissionProvider.ManageCategories))
            //{
            //    //TODO redirect to access denied page
            //}

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

        #region Edit

        public ActionResult Edit(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null) throw new ArgumentException("No category found with the specified id", "id");
            var model = product.ToModel();
            foreach (var language in _languageService.GetAllLanguages(true))
            {
                var localizedModel = new ProductLocalizedModel
                {
                    Language = language,
                    Name = product.GetLocalized(x => x.Name, language.Id, false),
                    ShortDescription = product.GetLocalized(x => x.ShortDescription, language.Id, false),
                    FullDescription = product.GetLocalized(x => x.FullDescription, language.Id, false),
                    MetaKeywords = product.GetLocalized(x => x.MetaKeywords, language.Id, false),
                    MetaDescription = product.GetLocalized(x => x.MetaDescription, language.Id, false),
                    MetaTitle = product.GetLocalized(x => x.MetaTitle, language.Id, false),
                    SeName = product.GetLocalized(x => x.SeName, language.Id, false),
                };
                model.Locales.Add(localizedModel);
            }
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

            //UpdateLocales(category, categoryModel);
            //UpdateCategoryProducts(category, addedCategoryProducts, removedCategoryProducts);

            if (continueEditing)
            {
                //return RedirectToAction("Edit", category.Id);
            }
            return RedirectToAction("List");
        }

        #endregion

		#region Methods 

		#region Public Methods 

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

		#endregion Public Methods 

		#endregion Methods 
    }
}
