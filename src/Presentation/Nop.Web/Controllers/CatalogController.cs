using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using System.Linq;
using Nop.Services.Directory;
using Nop.Services.Tax;
using Nop.Web.Models;

namespace Nop.Web.Controllers
{
    public class CatalogController : Controller
    {
		#region Fields 

        private ICategoryService _categoryService;
        private IProductService _productService;
        private IWorkContext _workContext;
        private ITaxService _taxService;
        private ICurrencyService _currencyService;

        #endregion Fields 

		#region Constructors 

        public CatalogController(ICategoryService categoryService, 
            IProductService productService, 
            IWorkContext workContext, 
            ITaxService taxService, 
            ICurrencyService currencyService)
        {
            _currencyService = currencyService;
            _taxService = taxService;
            _workContext = workContext;
            _productService = productService;
            _categoryService = categoryService;
        }

		#endregion Constructors 

		#region Methods 

		#region Public Methods 

        public ActionResult Category(int categoryId, PagingFilteringModel command)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null) return RedirectToAction("Index", "Home");

            if (command.PageSize <= 0) command.PageSize = category.PageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var model = category.ToModel();
            var products = _productService.SearchProducts(categoryId, 0, false,
                                                          command.PriceMin, command.PriceMax, 0, 0, string.Empty,
                                                          false,
                                                          _workContext.WorkingLanguage.Id, command.Specs,
                                                          command.ProductSorting,
                                                          command.PageNumber - 1, command.PageSize);
            model.Products = products.Select(x =>
                                                 {
                                                     var m = x.ToModel();
                                                     m.ProductPrice = BuildProductPriceModel(x);
                                                     return m;
                                                 }).ToList();
            model.SubCategories =
                _categoryService.GetAllCategoriesByParentCategoryId(categoryId).Select(
                    AutoMapper.Mapper.Map<Category, CategoryModel.SubCategoryModel>).ToList();

            model.PagingFilteringContext.LoadPagedList(products);

            return View(model);
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

		#endregion Public Methods 

		#endregion Methods 

        #region NonActions

        private ProductModel.ProductPriceModel BuildProductPriceModel(Product product)
        {
            var productPrice = new ProductModel.ProductPriceModel();

            if (!(product.ProductVariants.Count > 1))
            {
                //single product variant
                var productVariant = product.ProductVariants.ElementAt(0);
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

        #endregion
    }
}
