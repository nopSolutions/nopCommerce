using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Controllers
{
    public class CatalogController : BaseNopController
    {
		#region Fields

        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IWorkContext _workContext;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICustomerContentService _customerContentService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IAuthenticationService _authenticationService;
        private readonly IShoppingCartService _shoppingCartService;

        private readonly MediaSettings _mediaSetting;
        private readonly CatalogSettings _catalogSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        
        #endregion

		#region Constructors

        public CatalogController(ICategoryService categoryService, 
            IManufacturerService manufacturerService, IProductService productService, 
            IProductAttributeService productAttributeService, IProductAttributeParser productAttributeParser, 
            IWorkContext workContext, ITaxService taxService, ICurrencyService currencyService,
            IPictureService pictureService, ILocalizationService localizationService,
            IPriceCalculationService priceCalculationService, IPriceFormatter priceFormatter,
            IWebHelper webHelper, ISpecificationAttributeService specificationAttributeService,
            ICustomerContentService customerContentService, IDateTimeHelper dateTimeHelper,
            IAuthenticationService authenticationService, IShoppingCartService shoppingCartService,
            MediaSettings mediaSetting, CatalogSettings catalogSettings,
            CustomerSettings customerSettings, ShoppingCartSettings shoppingCartSettings)
        {
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._workContext = workContext;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._webHelper = webHelper;
            this._specificationAttributeService = specificationAttributeService;
            this._customerContentService = customerContentService;
            this._dateTimeHelper = dateTimeHelper;
            this._authenticationService = authenticationService;
            this._shoppingCartService = shoppingCartService;

            this._mediaSetting = mediaSetting;
            this._catalogSettings = catalogSettings;
            this._customerSettings = customerSettings;
            this._shoppingCartSettings = shoppingCartSettings;
        }

		#endregion Constructors 
        
        #region Utilities

        [NonAction]
        private ProductVariant GetMinimalPriceProductVariant(IList<ProductVariant> variants)
        {
            if (variants == null)
                throw new ArgumentNullException("variants");

            if (variants.Count == 0)
                return null;
            
            var tmp1= variants.ToList();
            tmp1.Sort(new GenericComparer<ProductVariant>("Price", GenericComparer<ProductVariant>.SortOrder.Ascending));
            return tmp1[0];
        }

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
        private ProductModel.ProductPriceModel PrepareProductPriceModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var model = new ProductModel.ProductPriceModel();
            var productVariants = _productService.GetProductVariantsByProductId(product.Id);

            switch (productVariants.Count)
            {
                case 0:
                    {
                        //no variants
                        model.OldPrice = null;
                        model.Price = null;
                    }
                    break;
                case 1:
                    {
                        //only one variant
                        var productVariant = productVariants[0];

                        if (!_catalogSettings.HidePricesForNonRegistered ||
                            (_workContext.CurrentCustomer != null &&
                            !_workContext.CurrentCustomer.IsGuest()))
                        {
                            if (!productVariant.CustomerEntersPrice)
                            {
                                if (productVariant.CallForPrice)
                                {
                                    model.OldPrice = null;
                                    model.Price = _localizationService.GetResource("Products.CallForPrice");
                                }
                                else
                                {
                                    decimal taxRate = decimal.Zero;
                                    decimal oldPriceBase = _taxService.GetProductPrice(productVariant, productVariant.OldPrice, out taxRate);
                                    decimal finalPriceBase = _taxService.GetProductPrice(productVariant, _priceCalculationService.GetFinalPrice(productVariant, true), out taxRate);

                                    decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                                    decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                                    if (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero)
                                    {
                                        model.OldPrice = _priceFormatter.FormatPrice(oldPrice);
                                        model.Price = _priceFormatter.FormatPrice(finalPrice);
                                    }
                                    else
                                    {
                                        model.OldPrice = null;
                                        model.Price = _priceFormatter.FormatPrice(finalPrice);
                                    }
                                }
                            }
                        }
                        else
                        {
                            model.OldPrice = null;
                            model.Price = null;
                        }
                    }
                    break;
                default:
                    {
                        //multiple variants
                        var productVariant = GetMinimalPriceProductVariant(productVariants);
                        if (productVariant != null)
                        {
                            if (!_catalogSettings.HidePricesForNonRegistered ||
                                (_workContext.CurrentCustomer != null &&
                                !_workContext.CurrentCustomer.IsGuest()))
                            {
                                if (!productVariant.CustomerEntersPrice)
                                {
                                    if (productVariant.CallForPrice)
                                    {
                                        model.OldPrice = null;
                                        model.Price = _localizationService.GetResource("Products.CallForPrice");
                                    }
                                    else
                                    {
                                        decimal taxRate = decimal.Zero;
                                        decimal fromPriceBase = _taxService.GetProductPrice(productVariant, _priceCalculationService.GetFinalPrice(productVariant, false), out taxRate);
                                        decimal fromPrice = _currencyService.ConvertFromPrimaryStoreCurrency(fromPriceBase, _workContext.WorkingCurrency);

                                        model.OldPrice = null;
                                        model.Price = String.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(fromPrice));
                                    }
                                }
                            }
                            else
                            {
                                model.OldPrice = null;
                                model.Price = null;
                            }
                        }
                    }
                    break;
            }

            //'add to cart' button
            switch (productVariants.Count)
            {
                case 0:
                    {
                        // no variants
                        model.DisableBuyButton = true;
                    }
                    break;
                case 1:
                    {

                        //only one variant
                        var productVariant = productVariants[0];
                        model.DisableBuyButton = productVariant.DisableBuyButton;
                        if (!_catalogSettings.HidePricesForNonRegistered ||
                            (_workContext.CurrentCustomer != null &&
                            !_workContext.CurrentCustomer.IsGuest()))
                        {
                            //invert condition
                        }
                        else
                        {
                            model.DisableBuyButton = true;
                        }
                    }
                    break;
                default:
                    {
                        //multiple variants
                        model.DisableBuyButton = true;
                    }
                    break;
            }
            
            return model;
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
        private IList<CategoryNavigationModel> GetChildCategoryNavigationModel(IList<Category> breadCrumb, int rootCategoryId, Category currentCategory, int level)
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

                if (_catalogSettings.ShowCategoryProductNumber)
                {
                    model.DisplayNumberOfProducts = true;
                    model.NumberOfProducts = GetNumberOfProducts(category, _catalogSettings.ShowCategoryProductNumberIncludingSubcategories);
                }
                result.Add(model);

                for (int i = 0; i <= breadCrumb.Count - 1; i++)
                    if (breadCrumb[i].Id == category.Id)
                        result.AddRange(GetChildCategoryNavigationModel(breadCrumb, category.Id, currentCategory, level + 1));
            }

            return result;
        }
        
        [NonAction]
        private ProductModel PrepareProductDetailsPageModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var model = product.ToModel();


            //pictures
            model.DefaultPictureZoomEnabled = _mediaSetting.DefaultPictureZoomEnabled;
            var pictures = _pictureService.GetPicturesByProductId(product.Id);
            if (pictures.Count > 0)
            {
                //default picture
                model.DefaultPictureModel = new PictureModel()
                {
                    ImageUrl = _pictureService.GetPictureUrl(pictures.FirstOrDefault(), _mediaSetting.ProductDetailsPictureSize),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(pictures.FirstOrDefault()),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name),
                };
                //all pictures
                foreach (var picture in pictures)
                {
                    model.PictureModels.Add(new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, 70),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name),

                    });
                }
            }
            else
            {
                //no images. set the default one
                model.DefaultPictureModel = new PictureModel()
                {
                    ImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSetting.ProductDetailsPictureSize),
                    FullSizeImageUrl = _pictureService.GetDefaultPictureUrl(),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name),
                };
            }


            //product variants
            foreach (var variant in _productService.GetProductVariantsByProductId(product.Id))
                model.ProductVariantModels.Add(PrepareProductVariantModel(new ProductModel.ProductVariantModel(), variant));

            return model;
        }

        [NonAction]
        private ProductReviewsModel PrepareProductReviewsModel(ProductReviewsModel model, Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (model == null)
                throw new ArgumentNullException("model");

            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();

            model.AddProductReview.AllowAnonymousUsersToReviewProduct = _catalogSettings.AllowAnonymousUsersToReviewProduct;
            model.AddProductReview.CustomerIsRegistered = _workContext.CurrentCustomer.IsRegistered();

            var productReviews = product.ProductReviews.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc);
            foreach (var pr in productReviews)
            {
                model.Items.Add(new ProductReviewModel()
                {
                    Id = pr.Id,
                    CustomerName = "TODO customername/email/username here",
                    Title = pr.Title,
                    ReviewText = Nop.Core.Html.HtmlHelper.FormatText(pr.ReviewText, false, true, false, false, false, false),
                    Rating = pr.Rating,
                    HelpfulYesTotal = pr.HelpfulYesTotal,
                    HelpfulNoTotal = pr.HelpfulNoTotal,
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                });
            }

            return model;
        }

        [NonAction]
        private ProductModel.ProductVariantModel PrepareProductVariantModel(ProductModel.ProductVariantModel model, ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            if (model == null)
                throw new ArgumentNullException("model");

            #region Properties

            model.Id = productVariant.Id;
            model.Name = productVariant.GetLocalized(x => x.Name);
            model.ShowSku = _catalogSettings.ShowProductSku;
            model.Sku = productVariant.Sku;
            model.Description = productVariant.GetLocalized(x => x.Description);
            model.ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber;
            model.ManufacturerPartNumber = productVariant.ManufacturerPartNumber;
            model.StockAvailablity = productVariant.FormatStockMessage(_localizationService);
            model.PictureModel.ImageUrl = _pictureService.GetPictureUrl(productVariant.PictureId, _mediaSetting.ProductVariantPictureSize, false);
            model.PictureModel.Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name);
            model.PictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name);
            if (productVariant.IsDownload && productVariant.HasSampleDownload)
            {
                model.DownloadSampleUrl = Url.Action("Sample", "Download", new { productVariantId = productVariant.Id });
            }
            #endregion

            #region Product variant price
            
            if (!_catalogSettings.HidePricesForNonRegistered ||
                        (_workContext.CurrentCustomer != null &&
                        !_workContext.CurrentCustomer.IsGuest()))
            {
                if (productVariant.CustomerEntersPrice)
                {
                    model.ProductVariantPrice.CustomerEntersPrice = true;
                }
                else
                {
                    if (productVariant.CallForPrice)
                    {
                        model.ProductVariantPrice.CallForPrice = true;
                    }
                    else
                    {
                        decimal taxRate = decimal.Zero;
                        decimal oldPriceBase = _taxService.GetProductPrice(productVariant, productVariant.OldPrice, out taxRate);
                        decimal finalPriceWithoutDiscountBase = _taxService.GetProductPrice(productVariant, _priceCalculationService.GetFinalPrice(productVariant, false), out taxRate);
                        decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(productVariant, _priceCalculationService.GetFinalPrice(productVariant, true), out taxRate);

                        decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            model.ProductVariantPrice.OldPrice = _priceFormatter.FormatPrice(oldPrice);

                        model.ProductVariantPrice.Price = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            model.ProductVariantPrice.PriceWithDiscount = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                    }
                }
            }
            else
            {
                model.ProductVariantPrice.OldPrice = null;
                model.ProductVariantPrice.Price = null;
            }
            #endregion

            #region 'Add to cart' model

            model.AddToCart.ProductVariantId = productVariant.Id;

            //quantity
            model.AddToCart.EnteredQuantity = productVariant.OrderMinimumQuantity;

            //'add to cart', 'add to wishlist' buttons
            if (productVariant.DisableBuyButton)
            {
                model.AddToCart.DisableBuyButton = true;
                model.AddToCart.DisableWishlistButton = true;
            }
            if (!_shoppingCartSettings.WishlistEnabled)
            {
                model.AddToCart.DisableWishlistButton = true;
            }
            if (!_catalogSettings.HidePricesForNonRegistered ||
                        (_workContext.CurrentCustomer != null &&
                        !_workContext.CurrentCustomer.IsGuest()))
            {
                //invert condition
            }
            else
            {
                model.AddToCart.DisableBuyButton = true;
                model.AddToCart.DisableWishlistButton = true;
            }

            model.AddToCart.CustomerEntersPrice = productVariant.CustomerEntersPrice;
            if (model.AddToCart.CustomerEntersPrice)
            {
                decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(productVariant.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(productVariant.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);

                model.AddToCart.CustomerEnteredPrice = minimumCustomerEnteredPrice;
                model.AddToCart.CustomerEnteredPriceRange = string.Format(_localizationService.GetResource("Products.EnterProductPrice.Range"),
                    _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                    _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false));
            }

            #endregion 
            
            #region Gift card

            model.GiftCard.IsGiftCard = productVariant.IsGiftCard;
            if (model.GiftCard.IsGiftCard)
            {
                model.GiftCard.GiftCardType = productVariant.GiftCardType;
                var customer = _workContext.CurrentCustomer;
                if (customer != null)
                {
                    var firstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                    var lastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                    model.GiftCard.SenderName = firstName + lastName;
                }
                var user = _authenticationService.GetAuthenticatedUser();
                if (user != null)
                    model.GiftCard.SenderEmail = user.Email;
            }

            #endregion

            #region Product attributes
            
            var productVariantAttributes = _productAttributeService.GetProductVariantAttributesByProductVariantId(productVariant.Id);
            foreach (var attribute in productVariantAttributes)
            {
                var pvaModel = new ProductModel.ProductVariantModel.ProductVariantAttributeModel()
                    {
                        Id = attribute.Id,
                        ProductVariantId = productVariant.Id,
                        ProductAttributeId = attribute.ProductAttributeId,
                        Name = attribute.ProductAttribute.GetLocalized(x => x.Name),
                        Description = attribute.ProductAttribute.GetLocalized(x => x.Description),
                        TextPrompt = attribute.TextPrompt,
                        IsRequired = attribute.IsRequired,
                        AttributeControlType = attribute.AttributeControlType
                    };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var pvaValues = _productAttributeService.GetProductVariantAttributeValues(attribute.Id);
                    foreach (var pvaValue in pvaValues)
                    {
                        var pvaValueModel = new ProductModel.ProductVariantModel.ProductVariantAttributeValueModel()
                        {
                            Id = pvaValue.Id,
                            Name = pvaValue.GetLocalized(x=>x.Name),
                            IsPreSelected = pvaValue.IsPreSelected
                        };
                        pvaModel.Values.Add(pvaValueModel);
                        
                        //display price if allowed
                        if (!_catalogSettings.HidePricesForNonRegistered ||
                            (_workContext.CurrentCustomer != null &&
                            !_workContext.CurrentCustomer.IsGuest()))
                        {
                            decimal taxRate = decimal.Zero;
                            decimal priceAdjustmentBase = _taxService.GetProductPrice(productVariant, pvaValue.PriceAdjustment, out taxRate);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                pvaValueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment, false, false);
                            else if (priceAdjustmentBase < decimal.Zero)
                                pvaValueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment, false, false);
                        }
                    }
                }

                model.ProductVariantAttributes.Add(pvaModel);
            }

            #endregion 

            return model;
        }
        
        #endregion

        #region Categories

        public ActionResult Category(int categoryId, PagingFilteringModel command)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted || !category.Published)
                return RedirectToAction("Index", "Home");

            if (command.PageSize <= 0) command.PageSize = category.PageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var model = category.ToModel();
            



            //sorting
            model.AllowProductFiltering = _catalogSettings.AllowProductSorting;
            if (model.AllowProductFiltering)
            {
                foreach (ProductSortingEnum enumValue in Enum.GetValues(typeof(ProductSortingEnum)))
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + ((int)enumValue).ToString(), null);
                    
                    var sortValue = enumValue.GetLocalizedEnum(_localizationService, _workContext);
                    model.AvailableSortOptions.Add(new SelectListItem()
                        {
                            Text = sortValue,
                            Value = sortUrl,
                            Selected = enumValue == (ProductSortingEnum)command.OrderBy
                        });
                }
            }



            //view mode
            model.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;
            if (model.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                model.AvailableViewModes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("Categories.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = command.ViewMode == "grid"
                });
                //list
                model.AvailableViewModes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("Categories.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = command.ViewMode == "list"
                });
            }



            //price ranges
            model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(category.PriceRanges, _webHelper, _priceFormatter);
            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper, category.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
            }




            //specs
            model.PagingFilteringContext.SpecificationFilter.LoadSpecsFilters(category, _specificationAttributeService, _webHelper, _workContext);
            IList<int> selectedSpecs = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            



            //category breadcrumb
            model.DisplayCategoryBreadcrumb = _catalogSettings.CategoryBreadcrumbEnabled;
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




            //subcategories
            model.SubCategories = _categoryService
                .GetAllCategoriesByParentCategoryId(categoryId)
                .Select(x =>
                {
                    var subCatModel = AutoMapper.Mapper.Map<Category, CategoryModel.SubCategoryModel>(x);
                    subCatModel.PictureModel.ImageUrl = _pictureService.GetPictureUrl(x.PictureId, _mediaSetting.CategoryThumbPictureSize, true);
                    subCatModel.PictureModel.Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), model.Name);
                    subCatModel.PictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), model.Name);
                    return subCatModel;
                })
                .ToList();




            //featured products
            var featuredProducts = _productService.SearchProducts(category.Id,
                0, true, null, null, 0, 0, null, false, _workContext.WorkingLanguage.Id, null,
                 ProductSortingEnum.Position, 0, int.MaxValue);
            model.FeaturedProducts = featuredProducts.Select(x =>
            {
                var m = x.ToModel();
                //price
                m.ProductPrice = PrepareProductPriceModel(x);
                //picture
                var picture = x.GetDefaultProductPicture(_pictureService);
                if (picture != null)
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.ProductThumbPictureSize, true);
                else
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSetting.ProductThumbPictureSize);
                m.DefaultPictureModel.Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), m.Name);
                m.DefaultPictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), m.Name);
                return m;
            })
            .ToList();




            //products
            var products = _productService.SearchProducts(category.Id, 0, false, minPriceConverted, maxPriceConverted,
                0, 0, string.Empty, false, _workContext.WorkingLanguage.Id, selectedSpecs,
                (ProductSortingEnum)command.OrderBy, command.PageNumber - 1, command.PageSize);
            model.Products = products.Select(x =>
            {
                var m = x.ToModel();
                //price
                m.ProductPrice = PrepareProductPriceModel(x);
                //picture
                var picture = x.GetDefaultProductPicture(_pictureService);
                if (picture != null)
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.ProductThumbPictureSize, true);
                else
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSetting.ProductThumbPictureSize);
                m.DefaultPictureModel.Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), m.Name);
                m.DefaultPictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), m.Name);
                return m;
            })
            .ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            model.PagingFilteringContext.ViewMode = command.ViewMode;
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult CategoryNavigation(int currentCategoryId)
        {
            var currentCategory = _categoryService.GetCategoryById(currentCategoryId);
            var breadCrumb = currentCategory != null ? GetCategoryBreadCrumb(currentCategory) : new List<Category>();
            var model = GetChildCategoryNavigationModel(breadCrumb, 0, currentCategory, 0);

            return PartialView(model);
        }

        #endregion

        #region Manufacturers

        public ActionResult Manufacturer(int manufacturerId, PagingFilteringModel command)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null || manufacturer.Deleted || !manufacturer.Published)
                return RedirectToAction("Index", "Home");

            if (command.PageSize <= 0) command.PageSize = manufacturer.PageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var model = manufacturer.ToModel();




            //sorting
            model.AllowProductFiltering = _catalogSettings.AllowProductSorting;
            if (model.AllowProductFiltering)
            {
                foreach (ProductSortingEnum enumValue in Enum.GetValues(typeof(ProductSortingEnum)))
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + ((int)enumValue).ToString(), null);

                    var sortValue = enumValue.GetLocalizedEnum(_localizationService, _workContext);
                    model.AvailableSortOptions.Add(new SelectListItem()
                    {
                        Text = sortValue,
                        Value = sortUrl,
                        Selected = enumValue == (ProductSortingEnum)command.OrderBy
                    });
                }
            }



            //view mode
            model.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;
            if (model.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                model.AvailableViewModes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("Manufacturers.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = command.ViewMode == "grid"
                });
                //list
                model.AvailableViewModes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("Manufacturers.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = command.ViewMode == "list"
                });
            }



            //price ranges
            model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(manufacturer.PriceRanges, _webHelper, _priceFormatter);
            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper, manufacturer.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
            }

            


            //featured products
            var featuredProducts = _productService.SearchProducts(0,
                manufacturer.Id, true, null, null, 0, 0, null, false, _workContext.WorkingLanguage.Id, null,
                ProductSortingEnum.Position, 0, int.MaxValue);
            model.FeaturedProducts = featuredProducts.Select(x =>
            {
                var m = x.ToModel();
                //price
                m.ProductPrice = PrepareProductPriceModel(x);
                //picture
                var picture = x.GetDefaultProductPicture(_pictureService);
                if (picture != null)
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.ProductThumbPictureSize, true);
                else
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSetting.ProductThumbPictureSize);
                m.DefaultPictureModel.Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), m.Name);
                m.DefaultPictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), m.Name);
                return m;
            })
            .ToList();




            //products
            var products = _productService.SearchProducts(0, manufacturer.Id, false, minPriceConverted, maxPriceConverted,
                0, 0, string.Empty, false, _workContext.WorkingLanguage.Id, null,
                (ProductSortingEnum)command.OrderBy, command.PageNumber - 1, command.PageSize);
            model.Products = products.Select(x =>
            {
                var m = x.ToModel();
                //price
                m.ProductPrice = PrepareProductPriceModel(x);
                //picture
                var picture = x.GetDefaultProductPicture(_pictureService);
                if (picture != null)
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.ProductThumbPictureSize, true);
                else
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSetting.ProductThumbPictureSize);
                m.DefaultPictureModel.Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), m.Name);
                m.DefaultPictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), m.Name);
                return m;
            })
            .ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            model.PagingFilteringContext.ViewMode = command.ViewMode;
            return View(model);
        }

        public ActionResult ManufacturerAll()
        {
            var model = new List<ManufacturerModel>();
            var manufacturers = _manufacturerService.GetAllManufacturers();
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = manufacturer.ToModel();
                modelMan.PictureModel.ImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId, _mediaSetting.CategoryThumbPictureSize, true);
                modelMan.PictureModel.Title = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageLinkTitleFormat"), modelMan.Name);
                modelMan.PictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageAlternateTextFormat"), modelMan.Name);
                model.Add(modelMan);
            }

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ManufacturerNavigation(int currentManufacturerId)
        {
            var currentManufacturer = _manufacturerService.GetManufacturerById(currentManufacturerId);

            var model = new List<ManufacturerNavigationModel>();
            foreach (var manufacturer in _manufacturerService.GetAllManufacturers())
            {
                var modelMan = new ManufacturerNavigationModel()
                {
                    Id = manufacturer.Id,
                    Name = manufacturer.GetLocalized(x => x.Name),
                    SeName = manufacturer.GetSeName(),
                    IsActive = currentManufacturer != null && currentManufacturer.Id == manufacturer.Id,
                };
                model.Add(modelMan);
            }

            return PartialView(model);
        }

        #endregion

        #region Products

        public ActionResult Product(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published)
                return RedirectToAction("Index", "Home");

            var model = PrepareProductDetailsPageModel(product);
            return View(model);
        }

        [HttpPost, ActionName("Product")]
        [ValidateInput(false)]
        public ActionResult AddToCartProduct(int productId, FormCollection form)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published)
                return RedirectToAction("Index", "Home");

            //manually process form
            int productVariantId = 0;
            ShoppingCartType cartType = ShoppingCartType.ShoppingCart;
            foreach (string formKey in form.AllKeys)
            {
                if (formKey.StartsWith("addtocart-"))
                {
                    productVariantId = Convert.ToInt32(formKey.Substring(("addtocart-").Length));
                    cartType = ShoppingCartType.ShoppingCart;
                }
                else if (formKey.StartsWith("addtowishlist-"))
                {
                    productVariantId = Convert.ToInt32(formKey.Substring(("addtowishlist-").Length));
                    cartType = ShoppingCartType.Wishlist;
                }
            }

            var productVariant = _productService.GetProductVariantById(productVariantId);
            if (productVariant == null)
                return RedirectToAction("Index", "Home");

            #region Customer entered price
            decimal customerEnteredPriceConverted = decimal.Zero;
            if (productVariant.CustomerEntersPrice)
            {
                foreach (string formKey in form.AllKeys)
                    if (formKey.Equals(string.Format("price_{0}.CustomerEnteredPrice", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        decimal customerEnteredPrice = decimal.Zero;
                        if (decimal.TryParse(form[formKey], out customerEnteredPrice))
                            customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
                        break;
                    }
            }
            #endregion

            #region Quantity

            int quantity = 1;
            foreach (string formKey in form.AllKeys)
                if (formKey.Equals(string.Format("price_{0}.EnteredQuantity", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out quantity);
                    break;
                }

            #endregion
            
            string attributes = "";

            #region Product attributes
            string selectedAttributes = string.Empty;
            var productVariantAttributes = _productAttributeService.GetProductVariantAttributesByProductVariantId(productVariant.Id);
            foreach (var attribute in productVariantAttributes)
            {
                string controlId = string.Format("product_attribute_{0}_{1}_{2}", attribute.ProductVariantId, attribute.ProductAttributeId, attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                        {
                            var ddlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ddlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ddlAttributes);
                                if (selectedAttributeId > 0)
                                    selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.RadioList:
                        {
                            var rblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(rblAttributes))
                            {
                                int selectedAttributeId = int.Parse(rblAttributes);
                                if (selectedAttributeId > 0)
                                    selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId =  int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                        {
                            var txtAttribute = form[controlId];
                            if (!String.IsNullOrEmpty(txtAttribute))
                            {
                                string enteredText = txtAttribute.Trim();
                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.MultilineTextbox:
                        {
                            var txtAttribute = form[controlId];
                            if (!String.IsNullOrEmpty(txtAttribute))
                            {
                                string enteredText = txtAttribute.Trim();
                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            //TODO date picker
                        }
                        break;
                    default:
                        break;
                }
            }
            attributes = selectedAttributes;

            #endregion

            #region Gift cards

            if (productVariant.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributes = _productAttributeParser.AddGiftCardAttribute(attributes,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            //save item
            var addToCartWarnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                productVariant, cartType, attributes, customerEnteredPriceConverted, quantity);
            if (addToCartWarnings.Count == 0)
            {
                switch (cartType)
                {
                    case ShoppingCartType.ShoppingCart:
                        return RedirectToRoute("ShoppingCart");
                    case ShoppingCartType.Wishlist:
                        return RedirectToRoute("Wishlist");
                    default:
                        return RedirectToRoute("ShoppingCart");
                        break;
                }
            }
            else
            {
                //Errors
                foreach (string error in addToCartWarnings)
                    ModelState.AddModelError("", error);

                //If we got this far, something failed, redisplay form
                //UNDONE set already entered values (quantity, customer entered price, gift card attributes, product attributes
                var model = PrepareProductDetailsPageModel(product);
                return View(model);
            }
        }

        [ChildActionOnly]
        public ActionResult ProductBreadcrumb(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            var model = new ProductModel.ProductBreadcrumbModel()
            {
                DisplayBreadcrumb = _catalogSettings.CategoryBreadcrumbEnabled,
                ProductId = product.Id,
                ProductName = product.GetLocalized(x => x.Name),
                ProductSeName = product.GetSeName()
            };
            if (model.DisplayBreadcrumb)
            {
                var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);
                if (productCategories.Count > 0)
                {
                    var category = productCategories[0].Category;
                    if (category != null)
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
                }
            }
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductManufacturers(int productId)
        {
            var model = _manufacturerService.GetProductManufacturersByProductId(productId)
                .Select(x =>
                {
                    var m = x.Manufacturer.ToModel();
                    m.PictureModel.ImageUrl = _pictureService.GetPictureUrl(x.Manufacturer.PictureId, _mediaSetting.CategoryThumbPictureSize, true);
                    m.PictureModel.Title = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageLinkTitleFormat"), m.Name);
                    m.PictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageAlternateTextFormat"), m.Name);
                    return m;
                })
                .ToList();
            
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductReviewOverview(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            var model = new ProductReviewOverviewModel()
            {
                ProductId = product.Id,
                AllowAnonymousUsersToReviewProduct = _catalogSettings.AllowAnonymousUsersToReviewProduct,
                CustomerIsRegistered = _workContext.CurrentCustomer.IsRegistered(),
                RatingSum = product.ApprovedRatingSum,
                TotalReviews = product.ApprovedTotalReviews,
                AllowCustomerReviews = product.AllowCustomerReviews
            };
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductSpecifications(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            var model= _specificationAttributeService.GetProductSpecificationAttributesByProductId(product.Id, null, true)
                .Select(psa =>
                {
                    return new ProductSpecificationModel()
                    {
                        SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute.GetLocalized(x => x.Name),
                        SpecificationAttributeOption = psa.SpecificationAttributeOption.GetLocalized(x => x.Name)
                    };
                })
                .ToList();
            
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductTierPrices(int productVariantId)
        {
            var variant = _productService.GetProductVariantById(productVariantId);
            if (variant == null)
                throw new ArgumentException("No product variant found with the specified id");

            var model = _productService.GetTierPricesByProductVariantId(productVariantId)
                .FilterForCustomer(_workContext.CurrentCustomer)
                .Select(tierPrice =>
                {
                    var m  = new ProductModel.ProductVariantModel.TierPriceModel()
                    {
                        Quantity = tierPrice.Quantity,
                    };
                    decimal taxRate = decimal.Zero;
                    decimal priceBase = _taxService.GetProductPrice(variant, tierPrice.Price, out taxRate);
                    decimal price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase, _workContext.WorkingCurrency);
                    m.Price = _priceFormatter.FormatPrice(price, false, false);

                    return m;
                })
                .ToList();

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult RelatedProducts(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");
            
            var products = _productService.SearchProducts(0,
                0, null, null, null, productId, 0, null, false,
                0, null, ProductSortingEnum.Position, 0, int.MaxValue);
            var model = products.Select(x =>
            {
                var m = x.ToModel();
                //price
                m.ProductPrice = PrepareProductPriceModel(x);
                //picture
                var picture = x.GetDefaultProductPicture(_pictureService);
                if (picture != null)
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.ProductThumbPictureSize, true);
                else
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSetting.ProductThumbPictureSize);
                m.DefaultPictureModel.Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), m.Name);
                m.DefaultPictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), m.Name);
                return m;
            })
            .ToList();

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ShareButton()
        {
            var shareCode = _catalogSettings.PageShareCode;
            if (_webHelper.IsCurrentConnectionSecured())
            {
                //need to change the addthis link to be https linked when the page is, so that the page doesnt ask about mixed mode when viewed in https...
                shareCode = shareCode.Replace("http://", "https://");
            }

            return PartialView("ShareButton", shareCode);
        }

        public ActionResult ProductReviews(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToAction("Index", "Home");

            var model = new ProductReviewsModel();
            model = PrepareProductReviewsModel(model, product);
            //default value
            model.AddProductReview.Rating = 4;
            return View(model);
        }

        [HttpPost, ActionName("ProductReviews")]
        [FormValueRequired("add-review")]
        public ActionResult ProductReviewsAdd(int productId, ProductReviewsModel model)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
                }
                else
                {
                    //save review
                    int rating = model.AddProductReview.Rating;
                    if (rating < 1 || rating > 5)
                        rating = 4;
                    bool isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

                    _customerContentService.InsertCustomerContent(new ProductReview()
                    {
                        ProductId = product.Id,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        IpAddress = _webHelper.GetCurrentIpAddress(),
                        Title = model.AddProductReview.Title,
                        ReviewText = model.AddProductReview.ReviewText,
                        Rating = rating,
                        HelpfulYesTotal = 0,
                        HelpfulNoTotal = 0,
                        IsApproved = isApproved,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow,
                    });

                    //update product totals
                    _productService.UpdateProductReviewTotals(product);

                    //notify store owner
                    if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    {
                        //UNDONE notification email
                        //IoC.Resolve<IMessageService>().SendProductReviewNotificationMessage(productReview, IoC.Resolve<ILocalizationManager>().DefaultAdminLanguage.LanguageId);
                    }


                    model = PrepareProductReviewsModel(model, product);
                    model.AddProductReview.Title = null;
                    model.AddProductReview.ReviewText = null;

                    model.AddProductReview.SuccessfullyAdded = true;
                    if (!isApproved)
                        model.AddProductReview.Result = _localizationService.GetResource("Reviews.SeeAfterApproving");
                    else
                        model.AddProductReview.Result = _localizationService.GetResource("Reviews.SuccessfullyAdded");

                    return View(model);
                }
            }

            //If we got this far, something failed, redisplay form
            model = PrepareProductReviewsModel(model, product);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult CrossSellProducts()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            var products = _productService.GetCrosssellProductsByShoppingCart(cart, _shoppingCartSettings.CrossSellsNumber);
            var model = products.Select(x =>
            {
                var m = x.ToModel();
                //price
                m.ProductPrice = PrepareProductPriceModel(x);
                //picture
                var picture = x.GetDefaultProductPicture(_pictureService);
                if (picture != null)
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.ProductThumbPictureSize, true);
                else
                    m.DefaultPictureModel.ImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSetting.ProductThumbPictureSize);
                m.DefaultPictureModel.Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), m.Name);
                m.DefaultPictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), m.Name);
                return m;
            })
            .ToList();

            return PartialView(model);
        }

		#endregion
    }
}
