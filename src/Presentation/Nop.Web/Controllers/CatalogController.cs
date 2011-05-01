using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Catalog;
using System.Linq;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Models;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Controllers
{
    public class CatalogController : BaseNopController
    {
		#region Fields

        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;

        private readonly MediaSettings _mediaSetting;
        

        #endregion Fields 

		#region Constructors

        public CatalogController(ICategoryService categoryService, 
            IProductService productService, IWorkContext workContext, 
            ITaxService taxService, ICurrencyService currencyService,
            IPictureService pictureService, MediaSettings mediaSetting)
        {
            this._currencyService = currencyService;
            this._taxService = taxService;
            this._workContext = workContext;
            this._productService = productService;
            this._categoryService = categoryService;
            this._pictureService = pictureService;
            this._mediaSetting = mediaSetting;
        }

		#endregion Constructors 
        
        #region Utilities

        /// <summary>
        /// Gets a category breadcrumb
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Category</returns>
        [NonAction]
        private IList<Category> GetCategoryBreadCrumb(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var breadCrumb = new List<Category>();
            
            while (category != null && //category is not null
                !category.Deleted && //category is not deleted
                category.Published) //category is published
            {
                breadCrumb.Add(category);
                category = _categoryService.GetCategoryById(category.ParentCategoryId);
            }
            breadCrumb.Reverse();
            return breadCrumb;
        }

        [NonAction]
        private ProductModel.ProductPriceModel BuildProductPriceModel(Product product)
        {
            var productPrice = new ProductModel.ProductPriceModel();

            var productVariants = _productService.GetProductVariantsByProductId(product.Id);
            if (!(productVariants.Count > 1))
            {
                //single product variant
                var productVariant = productVariants.ElementAt(0);
                //TODO:Support HidePricesForNonRegistered?
                //if (!this.SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered") ||
                //    (NopContext.Current.User != null &&
                //    !NopContext.Current.User.IsGuest))
                //{
                if (productVariant.CustomerEntersPrice)
                {
                    productPrice.CustomerEntersPrice = true;
                }
                else
                {
                    if (productVariant.CallForPrice)
                    {
                        productPrice.CallForPrice = true;
                    }
                    else
                    {
                        var taxRate = decimal.Zero;
                        var oldPriceBase = _taxService.GetProductPrice(productVariant, productVariant.OldPrice, out taxRate); //_taxService.GetPrice(productVariant, productVariant.OldPrice, out taxRate);
                        var finalPriceBase = _taxService.GetProductPrice(productVariant, productVariant.Price, _workContext.CurrentCustomer, out taxRate);

                        var oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase,
                                                                                            _workContext.
                                                                                                WorkingCurrency);
                        var finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase,
                                                                                          _workContext.
                                                                                              WorkingCurrency);

                        if (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero)
                        {
                            productPrice.OldPrice = oldPrice;
                        }
                        else
                        {
                            productPrice.OldPrice = null;
                        }
                        productPrice.Price = finalPrice;
                    }
                }
                //}
            }
            else
            {
                //multiple product variants
                var productVariant = product.MinimalPriceProductVariant();
                if (productVariant != null)
                {
                    //TODO:Support HidePricesForNonRegistered?
                    //if (!this.SettingManager.GetSettingValueBoolean("Common.HidePricesForNonRegistered") ||
                    //    (NopContext.Current.User != null &&
                    //    !NopContext.Current.User.IsGuest))
                    //{
                    if (productVariant.CustomerEntersPrice)
                    {
                        productPrice.CustomerEntersPrice = true;
                    }
                    else
                    {
                        if (productVariant.CallForPrice)
                        {
                            productPrice.CallForPrice = true;
                        }
                        else
                        {
                            decimal taxRate;
                            decimal fromPriceBase = _taxService.GetProductPrice(productVariant, productVariant.Price,
                                                                                out taxRate);
                            decimal fromPrice = _currencyService.ConvertFromPrimaryStoreCurrency(fromPriceBase,
                                                                                                 _workContext.
                                                                                                     WorkingCurrency);
                            productPrice.Price = fromPrice;
                        }
                    }
                    //}
                    //else
                    //{
                    //    lblOldPrice.Visible = false;
                    //    lblPrice.Visible = false;
                    //}
                }
            }
            return productPrice;
        }

        [NonAction]
        private int GetNumberOfProducts(Category category, bool includeSubCategories)
        {
            var products = _productService.SearchProducts(category.Id,
                        0, null, null, null, 0, 0, string.Empty, false, 0, null,
                        ProductSortingEnum.Position, 0, 1);

            var numberOfProducts = products.TotalCount;

            if (includeSubCategories)
            {
                var subCategories = _categoryService.GetAllCategoriesByParentCategoryId(category.Id);
                foreach (var subCategory in subCategories)
                    numberOfProducts += GetNumberOfProducts(subCategory, includeSubCategories);
            }
            return numberOfProducts;
        }

        [NonAction]
        private IList<CategoryNavigationModel> GetChildCategoryNavigationModel(IList<Category> breadCrumb, 
            int rootCategoryId, Category currentCategory, int level)
        {
            var result = new List<CategoryNavigationModel>();
            foreach (var category in _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId))
            {
                var model = new CategoryNavigationModel()
                {
                    Id = category.Id,
                    Name = category.GetLocalized(x => x.Name),
                    SeName = category.GetSeName(),
                    IsActive = currentCategory != null && currentCategory.Id == category.Id,
                    NumberOfParentCategories = level
                };

                //UNDONE display number of products for each category if 'Display.Products.ShowCategoryProductNumber' setting is 'true' (look at nopCommerce 1.90
                bool showCategoryProductNumber = false;
                if (showCategoryProductNumber)
                {
                    model.DisplayNumberOfProducts = true;
                    //UNDONE display number of products for each subcategory category if 'Display.Products.ShowCategoryProductNumber.IncludeSubCategories' setting is 'true' (look at nopCommerce 1.90
                    bool includeSubCategories = true;
                    model.NumberOfProducts = GetNumberOfProducts(category, includeSubCategories);
                }
                result.Add(model);

                for (int i = 0; i <= breadCrumb.Count - 1; i++)
                    if (breadCrumb[i].Id == category.Id)
                        result.AddRange(GetChildCategoryNavigationModel(breadCrumb, category.Id, currentCategory, level + 1));
            }

            return result;
        }
        #endregion

		#region Methods

        public ActionResult Category(int categoryId, PagingFilteringModel command)
        {
            var ttt = this.RouteData;
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null) return RedirectToAction("Index", "Home");

            if (command.PageSize <= 0) command.PageSize = category.PageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var model = category.ToModel();
            //products
            var products = _productService.SearchProducts(categoryId, 0, false,
                                                          command.PriceMin, command.PriceMax, 0, 0, string.Empty,
                                                          false,
                                                          _workContext.WorkingLanguage.Id, command.Specs,
                                                          command.ProductSorting,
                                                          command.PageNumber - 1, command.PageSize);
            model.Products = products.Select(x =>
            {
                var m = x.ToModel();
                //price
                m.ProductPrice = BuildProductPriceModel(x);
                //picture
                var picture = x.GetDefaultProductPicture(_pictureService);
                if (picture != null)
                    m.ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.ProductThumbPictureSize, true);
                else
                    m.ImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSetting.ProductThumbPictureSize);
                return m;
            })
            .ToList();

            //subcategories
            model.SubCategories = _categoryService
                .GetAllCategoriesByParentCategoryId(categoryId)
                .Select(x => {
                    var subCatModel = AutoMapper.Mapper.Map<Category, CategoryModel.SubCategoryModel>(x);
                    subCatModel.ImageUrl = _pictureService.GetPictureUrl(x.PictureId, _mediaSetting.CategoryThumbPictureSize, true);
                    return subCatModel;
                })
                .ToList();

            //category breadcrumb
            model.DisplayCategoryBreadcrumb = true; //UNDONE use "Media.CategoryBreadcrumbEnabled" setting
            if (model.DisplayCategoryBreadcrumb)
            {
                foreach (var catBr in GetCategoryBreadCrumb(category))
                {
                    model.CategoryBreadcrumb.Add(new CategoryModel()
                        {
                            Id = catBr.Id,
                            Name = catBr.GetLocalized(x => x.Name),
                            SeName = catBr.GetSeName()
                        });
                }
            }

            model.PagingFilteringContext.LoadPagedList(products);
            return View(model);
        }

        public ActionResult CategoryNavigation(int currentCategoryId)
        {
            var currentCategory = _categoryService.GetCategoryById(currentCategoryId);
            var breadCrumb = currentCategory != null ? GetCategoryBreadCrumb(currentCategory) : new List<Category>();
            var model = GetChildCategoryNavigationModel(breadCrumb, 0, currentCategory, 0);

            return PartialView(model);
        }

        public ActionResult Product(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null) return RedirectToAction("Index", "Home");
            var model = product.ToModel();
            model.ProductPrice = BuildProductPriceModel(product);
            return View(model);
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

		#endregion Methods 
    }
}
