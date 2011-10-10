using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System.ServiceModel.Syndication;

namespace Nop.Web.Controllers
{
    public class CatalogController : BaseNopController
    {
		#region Fields

        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
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
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IProductTagService _productTagService;
        private readonly IOrderReportService _orderReportService;
        private readonly ICustomerService _customerService;

        private readonly MediaSettings _mediaSetting;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CustomerSettings _customerSettings;
        
        #endregion

		#region Constructors

        public CatalogController(ICategoryService categoryService, 
            IManufacturerService manufacturerService, IProductService productService, 
            IProductTemplateService productTemplateService,
            ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            IProductAttributeService productAttributeService, IProductAttributeParser productAttributeParser, 
            IWorkContext workContext, ITaxService taxService, ICurrencyService currencyService,
            IPictureService pictureService, ILocalizationService localizationService,
            IPriceCalculationService priceCalculationService, IPriceFormatter priceFormatter,
            IWebHelper webHelper, ISpecificationAttributeService specificationAttributeService,
            ICustomerContentService customerContentService, IDateTimeHelper dateTimeHelper,
            IShoppingCartService shoppingCartService,
            IRecentlyViewedProductsService recentlyViewedProductsService, ICompareProductsService compareProductsService,
            IWorkflowMessageService workflowMessageService, IProductTagService productTagService,
            IOrderReportService orderReportService, ICustomerService customerService,
            MediaSettings mediaSetting, CatalogSettings catalogSettings,
            ShoppingCartSettings shoppingCartSettings, StoreInformationSettings storeInformationSettings,
            LocalizationSettings localizationSettings, CustomerSettings customerSettings)
        {
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._productTemplateService = productTemplateService;
            this._categoryTemplateService = categoryTemplateService;
            this._manufacturerTemplateService = manufacturerTemplateService;
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
            this._shoppingCartService = shoppingCartService;
            this._recentlyViewedProductsService = recentlyViewedProductsService;
            this._compareProductsService = compareProductsService;
            this._workflowMessageService = workflowMessageService;
            this._productTagService = productTagService;
            this._orderReportService = orderReportService;
            this._customerService = customerService;


            this._mediaSetting = mediaSetting;
            this._catalogSettings = catalogSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._localizationSettings = localizationSettings;
            this._customerSettings = customerSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        private ProductVariant GetMinimalPriceProductVariant(IList<ProductVariant> variants)
        {
            if (variants == null)
                throw new ArgumentNullException("variants");

            if (variants.Count == 0)
                return null;

            var tmp1 = variants.ToList();
            tmp1.Sort(new GenericComparer<ProductVariant>("Price", GenericComparer<ProductVariant>.SortOrder.Ascending));
            return tmp1.Count > 0 ? tmp1[0] : null;
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
                            !_workContext.CurrentCustomer.IsGuest())
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
                                !_workContext.CurrentCustomer.IsGuest())
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
                            !_workContext.CurrentCustomer.IsGuest())
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
        private ProductModel PrepareProductOverviewModel(Product product, bool preparePriceModel = true, bool preparePictureModel = true)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var model = product.ToModel();
            //price
            if (preparePriceModel)
            {
                model.ProductPrice = PrepareProductPriceModel(product);
            }
            //picture
            if (preparePictureModel)
            {
                var picture = product.GetDefaultProductPicture(_pictureService);
                if (picture != null)
                    model.DefaultPictureModel.ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.ProductThumbPictureSize, true);
                else
                    model.DefaultPictureModel.ImageUrl = _pictureService.GetDefaultPictureUrl(_mediaSetting.ProductThumbPictureSize);
                model.DefaultPictureModel.Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name);
                model.DefaultPictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name);
            }
            return model;
        }

        [NonAction]
        private int GetNumberOfProducts(Category category, bool includeSubCategories)
        {
            var products = _productService.SearchProducts(category.Id,
                        0, null, null, null, 0, string.Empty, false, 0, null,
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

            //template
            var template = _productTemplateService.GetProductTemplateById(product.ProductTemplateId);
            if (template == null)
                template = _productTemplateService.GetAllProductTemplates().FirstOrDefault();
            model.ProductTemplateViewPath = template.ViewPath;

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
        private void PrepareProductReviewsModel(ProductReviewsModel model, Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            if (model == null)
                throw new ArgumentNullException("model");

            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();

            var productReviews = product.ProductReviews.Where(pr => pr.IsApproved).OrderBy(pr => pr.CreatedOnUtc);
            foreach (var pr in productReviews)
            {
                model.Items.Add(new ProductReviewModel()
                {
                    Id = pr.Id,
                    CustomerId = pr.CustomerId,
                    CustomerName = pr.Customer.FormatUserName(),
                    AllowViewingProfiles = _customerSettings.AllowViewingProfiles && pr.Customer != null && !pr.Customer.IsGuest(),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    Rating = pr.Rating,
                    Helpfulness = new ProductReviewHelpfulnessModel()
                    {
                        ProductReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    WrittenOnStr = _dateTimeHelper.ConvertToUserTime(pr.CreatedOnUtc, DateTimeKind.Utc).ToString("g"),
                });
            }

            model.AddProductReview.CanCurrentCustomerLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewProduct || !_workContext.CurrentCustomer.IsGuest();
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
            model.ProductVariantPrice.ProductVariantId = productVariant.Id;
            model.ProductVariantPrice.DynamicPriceUpdate = _catalogSettings.EnableDynamicPriceUpdate;
            if (!_catalogSettings.HidePricesForNonRegistered ||
                !_workContext.CurrentCustomer.IsGuest())
            {
                model.ProductVariantPrice.HidePrices = false;
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

                        model.ProductVariantPrice.PriceValue = finalPriceWithoutDiscount;
                        model.ProductVariantPrice.PriceWithDiscountValue = finalPriceWithDiscount;
                    }
                }
            }
            else
            {
                model.ProductVariantPrice.HidePrices = true;
                model.ProductVariantPrice.OldPrice = null;
                model.ProductVariantPrice.Price = null;
            }
            #endregion

            #region 'Add to cart' model

            model.AddToCart.ProductVariantId = productVariant.Id;

            //quantity
            model.AddToCart.EnteredQuantity = productVariant.OrderMinimumQuantity;

            //'add to cart', 'add to wishlist' buttons
            model.AddToCart.DisableBuyButton = productVariant.DisableBuyButton;
            model.AddToCart.DisableWishlistButton = productVariant.DisableWishlistButton || !_shoppingCartSettings.WishlistEnabled;
            if (!_catalogSettings.HidePricesForNonRegistered ||
                !_workContext.CurrentCustomer.IsGuest())
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
                model.GiftCard.SenderName = _workContext.CurrentCustomer.GetFullName();
                model.GiftCard.SenderEmail = _workContext.CurrentCustomer.Email;
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
                        AttributeControlType = attribute.AttributeControlType,
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
                            IsPreSelected = pvaValue.IsPreSelected,
                        };
                        pvaModel.Values.Add(pvaValueModel);
                        
                        //display price if allowed
                        if (!_catalogSettings.HidePricesForNonRegistered ||
                            !_workContext.CurrentCustomer.IsGuest())
                        {
                            decimal taxRate = decimal.Zero;
                            decimal priceAdjustmentBase = _taxService.GetProductPrice(productVariant, pvaValue.PriceAdjustment, out taxRate);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                pvaValueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment, false, false);
                            else if (priceAdjustmentBase < decimal.Zero)
                                pvaValueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment, false, false);

                            pvaValueModel.PriceAdjustmentValue = priceAdjustment;
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

        public ActionResult Category(int categoryId, CatalogPagingFilteringModel command)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted || !category.Published)
                return RedirectToAction("Index", "Home");

            //'Continue shopping' URL
            _customerService.SaveCustomerAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.LastContinueShoppingPage, _webHelper.GetThisPageUrl(false));

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
                    var subCatModel = new CategoryModel.SubCategoryModel()
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                    };
                    subCatModel.PictureModel.ImageUrl = _pictureService.GetPictureUrl(x.PictureId, _mediaSetting.CategoryThumbPictureSize, true);
                    subCatModel.PictureModel.Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), model.Name);
                    subCatModel.PictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), model.Name);
                    return subCatModel;
                })
                .ToList();




            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts && _categoryService.GetTotalNumberOfFeaturedProducts(categoryId) > 0)
            {
                //We use the fast GetTotalNumberOfFeaturedProducts before invoking of the slow SearchProducts
                //to ensure that we have at least one featured product
                var featuredProducts = _productService.SearchProducts(category.Id,
                    0, true, null, null, 0, null, false,
                    _workContext.WorkingLanguage.Id, null,
                    ProductSortingEnum.Position, 0, int.MaxValue);
                model.FeaturedProducts = featuredProducts.Select(x => PrepareProductOverviewModel(x)).ToList();
            }



            //products
            var products = _productService.SearchProducts(category.Id, 0, false, minPriceConverted, maxPriceConverted,
                0, string.Empty, false, _workContext.WorkingLanguage.Id, selectedSpecs,
                (ProductSortingEnum)command.OrderBy, command.PageNumber - 1, command.PageSize);
            model.Products = products.Select(x => PrepareProductOverviewModel(x)).ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            model.PagingFilteringContext.ViewMode = command.ViewMode;


            //template
            var template = _categoryTemplateService.GetCategoryTemplateById(category.CategoryTemplateId);
            if (template == null)
                template = _categoryTemplateService.GetAllCategoryTemplates().FirstOrDefault();

            return View(template.ViewPath, model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult CategoryNavigation(int currentCategoryId)
        {
            var currentCategory = _categoryService.GetCategoryById(currentCategoryId);
            var breadCrumb = currentCategory != null ? GetCategoryBreadCrumb(currentCategory) : new List<Category>();
            var model = GetChildCategoryNavigationModel(breadCrumb, 0, currentCategory, 0);

            return PartialView(model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult HomepageCategories()
        {
            var listModel = _categoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(x =>
                {
                    var catModel = x.ToModel();
                    catModel.PictureModel.ImageUrl = _pictureService.GetPictureUrl(x.PictureId, _mediaSetting.CategoryThumbPictureSize, true);
                    catModel.PictureModel.Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), catModel.Name);
                    catModel.PictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), catModel.Name);
                    return catModel;
                })
                .ToList();

            return PartialView(listModel);
        }

        #endregion

        #region Manufacturers

        public ActionResult Manufacturer(int manufacturerId, CatalogPagingFilteringModel command)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null || manufacturer.Deleted || !manufacturer.Published)
                return RedirectToAction("Index", "Home");

            //'Continue shopping' URL
            _customerService.SaveCustomerAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.LastContinueShoppingPage, _webHelper.GetThisPageUrl(false));

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
            if (!_catalogSettings.IgnoreFeaturedProducts && _manufacturerService.GetTotalNumberOfFeaturedProducts(manufacturerId) > 0)
            {
                //We use the fast GetTotalNumberOfFeaturedProducts before invoking of the slow SearchProducts
                //to ensure that we have at least one featured product
                var featuredProducts = _productService.SearchProducts(0,
                    manufacturer.Id, true, null, null, 0, null,
                    false, _workContext.WorkingLanguage.Id, null,
                    ProductSortingEnum.Position, 0, int.MaxValue);
                model.FeaturedProducts = featuredProducts.Select(x => PrepareProductOverviewModel(x)).ToList();
            }



            //products
            var products = _productService.SearchProducts(0, manufacturer.Id, false, minPriceConverted, maxPriceConverted,
                0, string.Empty, false, _workContext.WorkingLanguage.Id, null,
                (ProductSortingEnum)command.OrderBy, command.PageNumber - 1, command.PageSize);
            model.Products = products.Select(x => PrepareProductOverviewModel(x)).ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            model.PagingFilteringContext.ViewMode = command.ViewMode;


            //template
            var template = _manufacturerTemplateService.GetManufacturerTemplateById(manufacturer.ManufacturerTemplateId);
            if (template == null)
                template = _manufacturerTemplateService.GetAllManufacturerTemplates().FirstOrDefault();

            return View(template.ViewPath, model);
        }

        public ActionResult ManufacturerAll()
        {
            var model = new List<ManufacturerModel>();
            var manufacturers = _manufacturerService.GetAllManufacturers();
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = manufacturer.ToModel();
                modelMan.PictureModel.ImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId, _mediaSetting.ManufacturerThumbPictureSize, true);
                modelMan.PictureModel.Title = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageLinkTitleFormat"), modelMan.Name);
                modelMan.PictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageAlternateTextFormat"), modelMan.Name);
                model.Add(modelMan);
            }

            return View(model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
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

        //product details page
        public ActionResult Product(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published)
                return RedirectToAction("Index", "Home");

            //prepare the model
            var model = PrepareProductDetailsPageModel(product);

            //check whether we have at leat one variant
            if (model.ProductVariantModels.Count == 0)
                return RedirectToAction("Index", "Home");
            
            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            return View(model.ProductTemplateViewPath, model);
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
                            var date = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                            }
                            catch {}
                            if (selectedDate.HasValue)
                            {
                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
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
                productVariant, cartType, attributes, customerEnteredPriceConverted, quantity, true);
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
                }
            }
            else
            {
                //Errors
                foreach (string error in addToCartWarnings)
                    ModelState.AddModelError("", error);

                //If we got this far, something failed, redisplay form
                //TODO set already entered values (quantity, customer entered price, gift card attributes, product attributes
                var model = PrepareProductDetailsPageModel(product);

                return View(model.ProductTemplateViewPath, model);
            }
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
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
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult ProductManufacturers(int productId)
        {
            var model = _manufacturerService.GetProductManufacturersByProductId(productId)
                .Select(x =>
                {
                    var m = x.Manufacturer.ToModel();
                    m.PictureModel.ImageUrl = _pictureService.GetPictureUrl(x.Manufacturer.PictureId, _mediaSetting.ManufacturerThumbPictureSize, true);
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
                RatingSum = product.ApprovedRatingSum,
                TotalReviews = product.ApprovedTotalReviews,
                AllowCustomerReviews = product.AllowCustomerReviews
            };
            return PartialView(model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
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
                        SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
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
            if (_catalogSettings.IgnoreTierPrices)
                return Content(""); //ignore tier prices
            
            if (_catalogSettings.HidePricesForNonRegistered &&
                _workContext.CurrentCustomer.IsGuest())
                return Content(""); //hide prices

            var variant = _productService.GetProductVariantById(productVariantId);
            if (variant == null)
                throw new ArgumentException("No product variant found with the specified id");

            var model = _productService.GetTierPricesByProductVariantId(productVariantId)
                .FilterForCustomer(_workContext.CurrentCustomer)
                .Select(tierPrice =>
                            {
                                var m = new ProductModel.ProductVariantModel.TierPriceModel()
                                {
                                    Quantity = tierPrice.Quantity,
                                };
                                decimal taxRate = decimal.Zero;
                                decimal priceBase = _taxService.GetProductPrice(variant, tierPrice.Price, out taxRate);
                                decimal price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase,_workContext.WorkingCurrency);
                                m.Price = _priceFormatter.FormatPrice(price, false, false);

                                return m;
                            })
                .ToList();

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult RelatedProducts(int productId)
        {
            var products = new List<Product>();
            foreach (var rp in _productService.GetRelatedProductsByProductId1(productId))
            {
                var product = _productService.GetProductById(rp.ProductId2);
                if (product == null)
                    continue;

                //ensure that a related product has at least one available variant
                var variants = _productService.GetProductVariantsByProductId(product.Id);
                if (variants.Count > 0)
                    products.Add(product);
            }


            var model = products.Select(x => PrepareProductOverviewModel(x, false, true)).ToList();
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductsAlsoPurchased(int productId)
        {
            if (!_catalogSettings.ProductsAlsoPurchasedEnabled)
                return Content("");

            var products = _orderReportService.GetProductsAlsoPurchasedById(productId,
                _catalogSettings.ProductsAlsoPurchasedNumber);

            var model = products.Select(x => PrepareProductOverviewModel(x, false, true)).ToList();

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ShareButton()
        {
            if (_catalogSettings.ShowShareButton && !String.IsNullOrEmpty(_catalogSettings.PageShareCode))
            {
                var shareCode = _catalogSettings.PageShareCode;
                if (_webHelper.IsCurrentConnectionSecured())
                {
                    //need to change the addthis link to be https linked when the page is, so that the page doesnt ask about mixed mode when viewed in https...
                    shareCode = shareCode.Replace("http://", "https://");
                }

                return PartialView("ShareButton", shareCode);
            }

            return Content("");
        }

        [ChildActionOnly]
        public ActionResult CrossSellProducts()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            var products = _productService.GetCrosssellProductsByShoppingCart(cart, _shoppingCartSettings.CrossSellsNumber);
            var model = products.Select(x => PrepareProductOverviewModel(x))
            .ToList();

            return PartialView(model);
        }

        //recently viewed products
        public ActionResult RecentlyViewedProducts()
        {
            var model = new List<ProductModel>();
            if (_catalogSettings.RecentlyViewedProductsEnabled)
            {
                var products = _recentlyViewedProductsService.GetRecentlyViewedProducts(_catalogSettings.RecentlyViewedProductsNumber);
                foreach (var product in products)
                    model.Add(PrepareProductOverviewModel(product));
            }
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult RecentlyViewedProductsBlock()
        {
            var model = new List<ProductModel>();
            if (_catalogSettings.RecentlyViewedProductsEnabled)
            {
                var products = _recentlyViewedProductsService.GetRecentlyViewedProducts(_catalogSettings.RecentlyViewedProductsNumber);
                foreach (var product in products)
                    model.Add(PrepareProductOverviewModel(product, false, false));
            }
            return PartialView(model);
        }

        //recently added products
        public ActionResult RecentlyAddedProducts()
        {
            var model = new List<ProductModel>();
            if (_catalogSettings.RecentlyAddedProductsEnabled)
            {
                var products = _productService.SearchProducts(0, 0, null, null,
                    null, 0, null, false, _workContext.WorkingLanguage.Id,
                    null, ProductSortingEnum.CreatedOn, 0, _catalogSettings.RecentlyAddedProductsNumber);
                foreach (var product in products)
                    model.Add(PrepareProductOverviewModel(product));
            }
            return View(model);
        }

        public ActionResult RecentlyAddedProductsRss()
        {
            var feed = new SyndicationFeed(
                                    string.Format("{0}: Recently added products", _storeInformationSettings.StoreName),
                                    "Information about products",
                                    new Uri(_webHelper.GetStoreLocation(false)),
                                    "RecentlyAddedProductsRSS",
                                    DateTime.UtcNow);

            if (!_catalogSettings.RecentlyAddedProductsEnabled)
                return new RssActionResult() { Feed = feed };
            
            var items = new List<SyndicationItem>();
            var products = _productService.SearchProducts(0, 0, null, null,
                null, 0, null, false, _workContext.WorkingLanguage.Id,
                null, ProductSortingEnum.CreatedOn, 0, _catalogSettings.RecentlyAddedProductsNumber);
            foreach (var product in products)
            {
                string productUrl = Url.RouteUrl("Product", new { productId = product.Id, SeName = product.GetSeName() }, "http");
                items.Add(new SyndicationItem(product.GetLocalized(x => x.Name), product.GetLocalized(x => x.ShortDescription), new Uri(productUrl), String.Format("RecentlyAddedProduct:{0}", product.Id), product.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult() { Feed = feed };
        }

        [ChildActionOnly]
        public ActionResult HomepageBestSellers()
        {
            if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage == 0)
                return Content("");

            var products = new List<Product>();
            var report = _orderReportService.BestSellersReport(null, null, null, null, null,
                _catalogSettings.NumberOfBestsellersOnHomepage);
            foreach (var line in report)
            {
                var productVariant = _productService.GetProductVariantById(line.ProductVariantId);
                if (productVariant != null)
                {
                    var product = productVariant.Product;
                    if (product != null)
                    {
                        bool contains = false;
                        foreach (var p in products)
                        {
                            if (p.Id == product.Id)
                            {
                                contains = true;
                                break;
                            }
                        }
                        if (!contains)
                            products.Add(product);
                    }
                }
            }


            var model = new HomePageBestsellersModel()
            {
                UseSmallProductBox = _catalogSettings.UseSmallProductBoxOnHomePage,
            };
            model.Products = products
                .Select(x => PrepareProductOverviewModel(x, !_catalogSettings.UseSmallProductBoxOnHomePage, true))
                .ToList();
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult HomepageProducts()
        {
            var model = new HomePageProductsModel()
            {
                UseSmallProductBox = _catalogSettings.UseSmallProductBoxOnHomePage
            };
            model.Products = _productService.GetAllProductsDisplayedOnHomePage()
                .Select(x => PrepareProductOverviewModel(x, !_catalogSettings.UseSmallProductBoxOnHomePage, true))
                .ToList();

            return PartialView(model);
        }

		#endregion

        #region Product tags


        //Product tags

        [ChildActionOnly]
        public ActionResult ProductTags(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            var model = product.ProductTags
                .OrderByDescending(x => x.ProductCount)
                .Select(x =>
                {
                    var ptModel = new ProductTagModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ProductCount = x.ProductCount
                    };
                    return ptModel;
                })
                .ToList();

            return PartialView(model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult PopularProductTags()
        {
            var model = new PopularProductTagsModel();

            //get all tags
            var tags = _productTagService.GetAllProductTags()
                .OrderByDescending(x => x.ProductCount)
                .Where(x => x.ProductCount > 0)
                .Take(_catalogSettings.NumberOfProductTags)
                .ToList();
            //sorting
            tags = tags.OrderBy(x => x.Name).ToList();

            foreach (var tag in tags)
                model.Tags.Add(new ProductTagModel()
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    ProductCount = tag.ProductCount
                });

            return PartialView(model);
        }

        public ActionResult ProductsByTag(int productTagId, CatalogPagingFilteringModel command)
        {
            var productTag = _productTagService.GetProductById(productTagId);
            if (productTag == null)
                return RedirectToAction("Index", "Home");

            if (command.PageSize <= 0) command.PageSize = _catalogSettings.ProductsByTagPageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var model = new ProductsByTagModel()
            {
                TagName = productTag.Name
            };


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


            //products
            var products = _productService.SearchProducts(0, 0, false, null, null,
                productTag.Id, string.Empty, false, _workContext.WorkingLanguage.Id, null,
                (ProductSortingEnum)command.OrderBy, command.PageNumber - 1, command.PageSize);
            model.Products = products.Select(x => PrepareProductOverviewModel(x)).ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            model.PagingFilteringContext.ViewMode = command.ViewMode;
            return View(model);
        }


        #endregion

        #region Product reviews

        //products reviews
        public ActionResult ProductReviews(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToAction("Index", "Home");

            var model = new ProductReviewsModel();
            PrepareProductReviewsModel(model, product);
            //only registered users can leave reviews
            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
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

                    var productReview = new ProductReview()
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
                    };
                    _customerContentService.InsertCustomerContent(productReview);

                    //update product totals
                    _productService.UpdateProductReviewTotals(product);

                    //notify store owner
                    if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                        _workflowMessageService.SendProductReviewNotificationMessage(productReview, _localizationSettings.DefaultAdminLanguageId);


                    PrepareProductReviewsModel(model, product);
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
            PrepareProductReviewsModel(model, product);
            return View(model);
        }

        [HttpPost]
        public ActionResult SetProductReviewHelpfulness(int productReviewId, bool washelpful)
        {
            var productReview = _customerContentService.GetCustomerContentById(productReviewId) as ProductReview;
            if (productReview == null)
                throw new ArgumentException("No product review found with the specified id");

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                return Json(new
                {
                    Result = _localizationService.GetResource("Reviews.Helpfulness.OnlyRegistered"),
                    TotalYes = productReview.HelpfulYesTotal,
                    TotalNo = productReview.HelpfulNoTotal
                });
            }

            //delete previous helpfulness
            var oldPrh = (from prh in productReview.ProductReviewHelpfulnessEntries
                          where prh.CustomerId == _workContext.CurrentCustomer.Id
                          select prh).FirstOrDefault();
            if (oldPrh != null)
                _customerContentService.DeleteCustomerContent(oldPrh);

            //insert new helpfulness
            var newPrh = new ProductReviewHelpfulness()
            {
                ProductReviewId = productReview.Id,
                CustomerId = _workContext.CurrentCustomer.Id,
                IpAddress = _webHelper.GetCurrentIpAddress(),
                WasHelpful = washelpful,
                IsApproved = true, //always approved
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
            };
            _customerContentService.InsertCustomerContent(newPrh);

            //new totals
            int helpfulYesTotal = (from prh in productReview.ProductReviewHelpfulnessEntries
                                   where prh.WasHelpful
                                   select prh).Count();
            int helpfulNoTotal = (from prh in productReview.ProductReviewHelpfulnessEntries
                                  where !prh.WasHelpful
                                  select prh).Count();

            productReview.HelpfulYesTotal = helpfulYesTotal;
            productReview.HelpfulNoTotal = helpfulNoTotal;
            _customerContentService.UpdateCustomerContent(productReview);

            return Json(new
            {
                Result = _localizationService.GetResource("Reviews.Helpfulness.SuccessfullyVoted"),
                TotalYes = productReview.HelpfulYesTotal,
                TotalNo = productReview.HelpfulNoTotal
            });
        }

        #endregion

        #region Email a friend

        //products email a friend
        [ChildActionOnly]
        public ActionResult ProductEmailAFriendButton(int productId)
        {
            if (!_catalogSettings.EmailAFriendEnabled)
                return Content("");
            var model = new ProductEmailAFriendModel()
            {
                ProductId = productId
            };

            return PartialView("ProductEmailAFriendButton", model);
        }

        public ActionResult ProductEmailAFriend(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToAction("Index", "Home");

            var model = new ProductEmailAFriendModel();
            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();
            model.YourEmailAddress = _workContext.CurrentCustomer.Email;
            return View(model);
        }

        [HttpPost, ActionName("ProductEmailAFriend")]
        [FormValueRequired("send-email")]
        public ActionResult ProductEmailAFriendSend(int productId, ProductEmailAFriendModel model)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToEmailAFriend)
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Products.EmailAFriend.OnlyRegisteredUsers"));
                }
                else
                {
                    //email
                    _workflowMessageService.SendProductEmailAFriendMessage(_workContext.CurrentCustomer,
                            _workContext.WorkingLanguage.Id, product,
                            model.YourEmailAddress, model.FriendEmail, Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                    model.ProductId = product.Id;
                    model.ProductName = product.GetLocalized(x => x.Name);
                    model.ProductSeName = product.GetSeName();

                    model.SuccessfullySent = true;
                    model.Result = _localizationService.GetResource("Products.EmailAFriend.SuccessfullySent");

                    return View(model);
                }
            }

            //If we got this far, something failed, redisplay form
            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();
            return View(model);
        }

        #endregion

        #region Comparing products

        //compare products
        public ActionResult AddProductToCompareList(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published)
                return RedirectToAction("Index", "Home");

            if (!_catalogSettings.CompareProductsEnabled)
                return RedirectToAction("Index", "Home");

            _compareProductsService.AddProductToCompareList(productId);

            return RedirectToRoute("CompareProducts");
        }

        public ActionResult RemoveProductFromCompareList(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToAction("Index", "Home");

            if (!_catalogSettings.CompareProductsEnabled)
                return RedirectToAction("Index", "Home");

            _compareProductsService.RemoveProductFromCompareList(productId);

            return RedirectToRoute("CompareProducts");
        }

        public ActionResult CompareProducts()
        {
            if (!_catalogSettings.CompareProductsEnabled)
                return RedirectToAction("Index", "Home");

            var model = new List<ProductModel>();
            foreach (var product in _compareProductsService.GetComparedProducts())
            {
                var productModel = PrepareProductOverviewModel(product);
                //specs for comparing
                productModel.SpecificationAttributeModels = _specificationAttributeService.GetProductSpecificationAttributesByProductId(product.Id, null, true)
                    .Select(psa =>
                    {
                        return new ProductSpecificationModel()
                        {
                            SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
                            SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute.GetLocalized(x => x.Name),
                            SpecificationAttributeOption = psa.SpecificationAttributeOption.GetLocalized(x => x.Name)
                        };
                    })
                    .ToList();
                model.Add(productModel);
            }
            return View(model);
        }

        public ActionResult ClearCompareList()
        {
            if (!_catalogSettings.CompareProductsEnabled)
                return RedirectToAction("Index", "Home");

            _compareProductsService.ClearCompareProducts();

            return RedirectToRoute("CompareProducts");
        }

        [ChildActionOnly]
        public ActionResult CompareProductsButton(int productId)
        {
            if (!_catalogSettings.CompareProductsEnabled)
                return Content("");

            var model = new AddToCompareListModel()
            {
                ProductId = productId
            };

            return PartialView("CompareProductsButton", model);
        }

        #endregion

        #region Searching

        public ActionResult Search(SearchModel model, SearchPagingFilteringModel command)
        {
            if (model == null)
                model = new SearchModel();

            if (command.PageSize <= 0) command.PageSize = _catalogSettings.SearchPageProductsPerPage;
            if (command.PageNumber <= 0) command.PageNumber = 1;
            if (model.Q == null)
                model.Q = "";
            model.Q = model.Q.Trim();

            var categories = _categoryService.GetAllCategories();
            if (categories.Count > 0)
            {
                model.AvailableCategories.Add(new SelectListItem()
                    {
                         Value = "0",
                         Text = _localizationService.GetResource("Common.All")
                    });
                foreach(var c in categories)
                    model.AvailableCategories.Add(new SelectListItem()
                        {
                            Value = c.Id.ToString(),
                            Text = c.GetCategoryBreadCrumb(_categoryService),
                            Selected = model.Cid == c.Id
                        });
            }

            var manufacturers = _manufacturerService.GetAllManufacturers();
            if (manufacturers.Count > 0)
            {
                model.AvailableManufacturers.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = _localizationService.GetResource("Common.All")
                });
                foreach (var m in manufacturers)
                    model.AvailableManufacturers.Add(new SelectListItem()
                    {
                        Value = m.Id.ToString(),
                        Text = m.Name,
                        Selected = model.Mid == m.Id
                    });
            }

            IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
            // only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
            if (Request.Params["Q"] != null)
            {
                if (model.Q.Length < _catalogSettings.ProductSearchTermMinimumLength)
                {
                    model.Warning = string.Format(_localizationService.GetResource("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.ProductSearchTermMinimumLength);
                }
                else
                {
                    int categoryId = 0;
                    int manufacturerId = 0;
                    decimal? minPriceConverted = null;
                    decimal? maxPriceConverted = null;
                    bool searchInDescriptions = false;
                    if (model.As)
                    {
                        //advanced search
                        categoryId = model.Cid;
                        manufacturerId = model.Mid;

                        //min price
                        if (!string.IsNullOrEmpty(model.Pf))
                        {
                            decimal minPrice = decimal.Zero;
                            if (decimal.TryParse(model.Pf, out minPrice))
                                minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(minPrice, _workContext.WorkingCurrency);
                        }
                        //max price
                        if (!string.IsNullOrEmpty(model.Pt))
                        {
                            decimal maxPrice = decimal.Zero;
                            if (decimal.TryParse(model.Pt, out maxPrice))
                                maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(maxPrice, _workContext.WorkingCurrency);
                        }

                        searchInDescriptions = model.Sid;
                    }

                    //products
                    products = _productService.SearchProducts(categoryId, manufacturerId, null,
                        minPriceConverted, maxPriceConverted, 0,
                        model.Q, searchInDescriptions, _workContext.WorkingLanguage.Id, null,
                    ProductSortingEnum.Position, command.PageNumber - 1, command.PageSize);
                    model.Products = products.Select(x => PrepareProductOverviewModel(x)).ToList();

                    model.NoResults = !model.Products.Any();                    
                }
            }

            model.PagingFilteringContext.LoadPagedList(products);
            return View(model);
        }

        #endregion
    }
}
