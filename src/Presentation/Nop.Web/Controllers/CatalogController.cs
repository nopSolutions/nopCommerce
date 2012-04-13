using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
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
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.UI.Captcha;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System.Diagnostics;

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
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;

        private readonly MediaSettings _mediaSetting;
        private readonly CatalogSettings _catalogSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ICacheManager _cacheManager;
        private readonly CaptchaSettings _captchaSettings;
        
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
            IBackInStockSubscriptionService backInStockSubscriptionService,
            IPermissionService permissionService, IDownloadService downloadService,
            MediaSettings mediaSetting, CatalogSettings catalogSettings,
            ShoppingCartSettings shoppingCartSettings, StoreInformationSettings storeInformationSettings,
            LocalizationSettings localizationSettings, CustomerSettings customerSettings, 
            CaptchaSettings captchaSettings,
            ICacheManager cacheManager)
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
            this._backInStockSubscriptionService = backInStockSubscriptionService;
            this._permissionService = permissionService;
            this._downloadService = downloadService;


            this._mediaSetting = mediaSetting;
            this._catalogSettings = catalogSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._localizationSettings = localizationSettings;
            this._customerSettings = customerSettings;
            this._captchaSettings = captchaSettings;

            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected List<int> GetChildCategoryIds(int parentCategoryId, bool showHidden = false)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY, parentCategoryId, showHidden);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, showHidden);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id, showHidden));
                }
                return categoriesIds;
            });
            return cacheModel;
        }
        
        [NonAction]
        protected IList<Category> GetCategoryBreadCrumb(Category category)
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
        protected ProductModel.ProductPriceModel PrepareProductPriceModel(Product product)
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
                default:
                    {
                        
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            //calculate for the maximum quantity (in case if we have tier prices)
                            decimal? minimalPrice = null;
                            var productVariant = _priceCalculationService.GetProductVariantWithMinimalPrice(productVariants, _workContext.CurrentCustomer, true, int.MaxValue, out minimalPrice);

                            if (!productVariant.CustomerEntersPrice)
                            {
                                if (productVariant.CallForPrice)
                                {
                                    model.OldPrice = null;
                                    model.Price = _localizationService.GetResource("Products.CallForPrice");
                                }
                                else if (minimalPrice.HasValue)
                                {
                                    //calculate prices
                                    decimal taxRate = decimal.Zero;
                                    decimal oldPriceBase = _taxService.GetProductPrice(productVariant, productVariant.OldPrice, out taxRate);
                                    decimal finalPriceBase = _taxService.GetProductPrice(productVariant, minimalPrice.Value, out taxRate);

                                    decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                                    decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                                    //do we have tier prices configured?
                                    var tierPrices = productVariant.TierPrices
                                        .OrderBy(tp => tp.Quantity)
                                        .ToList()
                                        .FilterForCustomer(_workContext.CurrentCustomer)
                                        .RemoveDuplicatedQuantities();
                                    bool displayFromMessage =
                                        //When there is just one tier (with  qty 1), there are no actual savings in the list.
                                        (tierPrices.Count > 0 && !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1)) ||
                                        //we have more than one variant
                                        (productVariants.Count > 1);
                                    if (displayFromMessage)
                                    {
                                        
                                        model.OldPrice = null;
                                        model.Price = String.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));
                                    }
                                    else
                                    {
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
                                else
                                {
                                    //Actually it's not possible (we presume that minimalPrice always has a value)
                                    //We never should get here
                                    Debug.WriteLine(string.Format("PrepareProductPriceModel with minPrice not set. Variant ID = {0}", productVariant.Id));
                                }
                            }
                        }
                        else
                        {
                            //hide prices
                            model.OldPrice = null;
                            model.Price = null;
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
                        model.AvailableForPreOrder = false;
                    }
                    break;
                case 1:
                    {

                        //only one variant
                        var productVariant = productVariants[0];
                        model.DisableBuyButton = productVariant.DisableBuyButton || !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
                        if (!_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            model.DisableBuyButton = true;
                        }
                        model.AvailableForPreOrder = productVariant.AvailableForPreOrder;
                    }
                    break;
                default:
                    {
                        //multiple variants
                        model.DisableBuyButton = true;
                        model.AvailableForPreOrder = false;
                    }
                    break;
            }
            
            return model;
        }

        [NonAction]
        protected ProductModel PrepareProductOverviewModel(Product product, bool preparePriceModel = true, bool preparePictureModel = true, int? productThumbPictureSize = null)
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
                //If a size has been set in the view, we use it in priority
                int pictureSize = productThumbPictureSize.HasValue ? productThumbPictureSize.Value : _mediaSetting.ProductThumbPictureSize;
                //prepare picture model
                var defaultProductPictureCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_DEFAULTPICTURE_MODEL_KEY, product.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured());
                model.DefaultPictureModel = _cacheManager.Get(defaultProductPictureCacheKey, () =>
                {
                    var picture = product.GetDefaultProductPicture(_pictureService);
                    var pictureModel = new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name)
                    };
                    return pictureModel;
                });
            }
            return model;
        }
        
        [NonAction]
        protected IList<CategoryNavigationModel> GetChildCategoryNavigationModel(IList<Category> breadCrumb, int rootCategoryId, Category currentCategory, int level)
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
                    var categoryIds = new List<int>();
                    categoryIds.Add(category.Id);
                    if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                    {
                        //include subcategories
                        categoryIds.AddRange(GetChildCategoryIds(category.Id));
                    }
                    IList<int> filterableSpecificationAttributeOptionIds = null;
                    model.NumberOfProducts = _productService.SearchProducts(categoryIds,
                        0, null, null, null, 0, string.Empty, false, 0, null,
                        ProductSortingEnum.Position, 0, 1,
                        false, out filterableSpecificationAttributeOptionIds).TotalCount;
                }
                result.Add(model);

                for (int i = 0; i <= breadCrumb.Count - 1; i++)
                    if (breadCrumb[i].Id == category.Id)
                        result.AddRange(GetChildCategoryNavigationModel(breadCrumb, category.Id, currentCategory, level + 1));
            }

            return result;
        }
        
        [NonAction]
        protected ProductModel PrepareProductDetailsPageModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            var model = product.ToModel();

            //template

            var templateCacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_TEMPLATE_MODEL_KEY, product.ProductTemplateId);
            model.ProductTemplateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _productTemplateService.GetProductTemplateById(product.ProductTemplateId);
                if (template == null)
                    template = _productTemplateService.GetAllProductTemplates().FirstOrDefault();
                return template.ViewPath;
            });

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
                        ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.ProductThumbPictureSizeOnProductDetailsPage),
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
        protected void PrepareProductReviewsModel(ProductReviewsModel model, Product product)
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
        protected ProductModel.ProductVariantModel PrepareProductVariantModel(ProductModel.ProductVariantModel model, ProductVariant productVariant)
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
            model.ShowGtin = _catalogSettings.ShowGtin;
            model.Gtin = productVariant.Gtin;
            model.StockAvailablity = productVariant.FormatStockMessage(_localizationService);
            model.PictureModel.FullSizeImageUrl = _pictureService.GetPictureUrl(productVariant.PictureId, 0, false);
            model.PictureModel.ImageUrl = _pictureService.GetPictureUrl(productVariant.PictureId, _mediaSetting.ProductVariantPictureSize, false);
            model.PictureModel.Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name);
            model.PictureModel.AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name);
            if (productVariant.IsDownload && productVariant.HasSampleDownload)
            {
                model.DownloadSampleUrl = Url.Action("Sample", "Download", new { productVariantId = productVariant.Id });
            }
            model.IsCurrentCustomerRegistered = _workContext.CurrentCustomer.IsRegistered();
            //back in stock subscriptions)
            if (productVariant.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                productVariant.BackorderMode == BackorderMode.NoBackorders &&
                productVariant.AllowBackInStockSubscriptions &&
                productVariant.StockQuantity <= 0)
            {
                //out of stock
                model.DisplayBackInStockSubscription = true;
                model.BackInStockAlreadySubscribed = _backInStockSubscriptionService.FindSubscription(_workContext.CurrentCustomer.Id, productVariant.Id) != null;
            }

            #endregion

            #region Product variant price
            model.ProductVariantPrice.ProductVariantId = productVariant.Id;
            model.ProductVariantPrice.DynamicPriceUpdate = _catalogSettings.EnableDynamicPriceUpdate;
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
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
            model.AddToCart.DisableBuyButton = productVariant.DisableBuyButton || !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.AddToCart.DisableWishlistButton = productVariant.DisableWishlistButton || !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist);
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.AddToCart.DisableBuyButton = true;
                model.AddToCart.DisableWishlistButton = true;
            }
            //pre-order
            model.AddToCart.AvailableForPreOrder = productVariant.AvailableForPreOrder;

            //customer entered price
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
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
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

            if (command.PageNumber <= 0) command.PageNumber = 1;

            var model = category.ToModel();
            



            //sorting
            model.PagingFilteringContext.AllowProductSorting = _catalogSettings.AllowProductSorting;
            if (model.PagingFilteringContext.AllowProductSorting)
            {
                foreach (ProductSortingEnum enumValue in Enum.GetValues(typeof(ProductSortingEnum)))
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + ((int)enumValue).ToString(), null);
                    
                    var sortValue = enumValue.GetLocalizedEnum(_localizationService, _workContext);
                    model.PagingFilteringContext.AvailableSortOptions.Add(new SelectListItem()
                        {
                            Text = sortValue,
                            Value = sortUrl,
                            Selected = enumValue == (ProductSortingEnum)command.OrderBy
                        });
                }
            }



            //view mode
            model.PagingFilteringContext.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;
            var viewMode = !string.IsNullOrEmpty(command.ViewMode) 
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            if (model.PagingFilteringContext.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                model.PagingFilteringContext.AvailableViewModes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("Categories.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = viewMode == "grid"
                });
                //list
                model.PagingFilteringContext.AvailableViewModes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("Categories.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = viewMode == "list"
                });
            }
                        
            //page size
            model.PagingFilteringContext.AllowCustomersToSelectPageSize = false;
            if (category.AllowCustomersToSelectPageSize && category.PageSizeOptions != null)
            {
                var pageSizes = category.PageSizeOptions.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        int temp = 0;

                        if (int.TryParse(pageSizes.FirstOrDefault(), out temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "pagesize={0}", null);
                    sortUrl = _webHelper.RemoveQueryString(sortUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        int temp = 0;
                        if (!int.TryParse(pageSize, out temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        model.PagingFilteringContext.PageSizeOptions.Add(new SelectListItem()
                        {
                            Text = pageSize,
                            Value = String.Format(sortUrl, pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (model.PagingFilteringContext.PageSizeOptions.Any())
                    {
                        model.PagingFilteringContext.PageSizeOptions = model.PagingFilteringContext.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        model.PagingFilteringContext.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(model.PagingFilteringContext.PageSizeOptions.FirstOrDefault().Text);
                        }
                    }
                }
            }

            if (command.PageSize <= 0) command.PageSize = category.PageSize;


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
                    var subCatName = x.GetLocalized(y => y.Name);
                    var subCatModel = new CategoryModel.SubCategoryModel()
                    {
                        Id = x.Id,
                        Name = subCatName,
                        SeName = x.GetSeName(),
                    };

                    //prepare picture model
                    int pictureSize = _mediaSetting.CategoryThumbPictureSize;
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured());
                    subCatModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var pictureModel = new PictureModel()
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(x.PictureId),
                            ImageUrl = _pictureService.GetPictureUrl(x.PictureId, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), subCatName),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), subCatName)
                        };
                        return pictureModel;
                    });

                    return subCatModel;
                })
                .ToList();




            //featured products
            //Question: should we use '_catalogSettings.ShowProductsFromSubcategories' setting for displaying featured products?
            if (!_catalogSettings.IgnoreFeaturedProducts && _categoryService.GetTotalNumberOfFeaturedProducts(categoryId) > 0)
            {
                //We use the fast GetTotalNumberOfFeaturedProducts before invoking of the slow SearchProducts
                //to ensure that we have at least one featured product
                IList<int> filterableSpecificationAttributeOptionIdsFeatured = null;
                var featuredProducts = _productService.SearchProducts(category.Id,
                    0, true, null, null, 0, null, false,
                    _workContext.WorkingLanguage.Id, null,
                    ProductSortingEnum.Position, 0, int.MaxValue,
                    false, out filterableSpecificationAttributeOptionIdsFeatured);
                model.FeaturedProducts = featuredProducts.Select(x => PrepareProductOverviewModel(x)).ToList();
            }


            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }
            //products
            IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            IList<int> filterableSpecificationAttributeOptionIds = null;
            var products = _productService.SearchProducts(categoryIds, 0, 
                _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false, 
                minPriceConverted, maxPriceConverted,
                0, string.Empty, false, _workContext.WorkingLanguage.Id, alreadyFilteredSpecOptionIds,
                (ProductSortingEnum)command.OrderBy, command.PageNumber - 1, command.PageSize,
                true, out filterableSpecificationAttributeOptionIds);
            model.Products = products.Select(x => PrepareProductOverviewModel(x)).ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            model.PagingFilteringContext.ViewMode = viewMode;

            //specs
            model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds,
                filterableSpecificationAttributeOptionIds, 
                _specificationAttributeService, _webHelper, _workContext);
            

            //template
            var templateCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_TEMPLATE_MODEL_KEY, category.CategoryTemplateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
                {
                    var template = _categoryTemplateService.GetCategoryTemplateById(category.CategoryTemplateId);
                    if (template == null)
                        template = _categoryTemplateService.GetAllCategoryTemplates().FirstOrDefault();
                    return template.ViewPath;
                });

            return View(templateViewPath, model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult CategoryNavigation(int currentCategoryId, int currentProductId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NAVIGATION_MODEL_KEY, currentCategoryId, currentProductId, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var currentCategory = _categoryService.GetCategoryById(currentCategoryId);
                if (currentCategory == null && currentProductId > 0)
                {
                    var productCategories = _categoryService.GetProductCategoriesByProductId(currentProductId);
                    if (productCategories.Count > 0)
                        currentCategory = productCategories[0].Category;
                }
                var breadCrumb = currentCategory != null ? GetCategoryBreadCrumb(currentCategory) : new List<Category>();
                var model = GetChildCategoryNavigationModel(breadCrumb, 0, currentCategory, 0);
                return model;
            });

            return PartialView(cacheModel);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult HomepageCategories()
        {
            var listModel = _categoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(x =>
                {
                    var catModel = x.ToModel();

                    //prepare picture model
                    int pictureSize = _mediaSetting.CategoryThumbPictureSize;
                    var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured());
                    catModel.PictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                    {
                        var pictureModel = new PictureModel()
                        {
                            FullSizeImageUrl = _pictureService.GetPictureUrl(x.PictureId),
                            ImageUrl = _pictureService.GetPictureUrl(x.PictureId, pictureSize),
                            Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), catModel.Name),
                            AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), catModel.Name)
                        };
                        return pictureModel;
                    });

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

            if (command.PageNumber <= 0) command.PageNumber = 1;

            var model = manufacturer.ToModel();




            //sorting
            model.PagingFilteringContext.AllowProductSorting = _catalogSettings.AllowProductSorting;
            if (model.PagingFilteringContext.AllowProductSorting)
            {
                foreach (ProductSortingEnum enumValue in Enum.GetValues(typeof(ProductSortingEnum)))
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + ((int)enumValue).ToString(), null);

                    var sortValue = enumValue.GetLocalizedEnum(_localizationService, _workContext);
                    model.PagingFilteringContext.AvailableSortOptions.Add(new SelectListItem()
                    {
                        Text = sortValue,
                        Value = sortUrl,
                        Selected = enumValue == (ProductSortingEnum)command.OrderBy
                    });
                }
            }



            //view mode
            model.PagingFilteringContext.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;
            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            if (model.PagingFilteringContext.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                model.PagingFilteringContext.AvailableViewModes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("Manufacturers.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = viewMode == "grid"
                });
                //list
                model.PagingFilteringContext.AvailableViewModes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("Manufacturers.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = viewMode == "list"
                });
            }
                        
            //page size
            model.PagingFilteringContext.AllowCustomersToSelectPageSize = false;
            if (manufacturer.AllowCustomersToSelectPageSize && manufacturer.PageSizeOptions != null)
            {
                var pageSizes = manufacturer.PageSizeOptions.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (manufacturer page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        int temp = 0;

                        if (int.TryParse(pageSizes.FirstOrDefault(), out temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "pagesize={0}", null);
                    sortUrl = _webHelper.RemoveQueryString(sortUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        int temp = 0;
                        if (!int.TryParse(pageSize, out temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        model.PagingFilteringContext.PageSizeOptions.Add(new SelectListItem()
                        {
                            Text = pageSize,
                            Value = String.Format(sortUrl, pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    model.PagingFilteringContext.PageSizeOptions = model.PagingFilteringContext.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();

                    if (model.PagingFilteringContext.PageSizeOptions.Any())
                    {
                        model.PagingFilteringContext.PageSizeOptions = model.PagingFilteringContext.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        model.PagingFilteringContext.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(model.PagingFilteringContext.PageSizeOptions.FirstOrDefault().Text);
                        }
                    }
                }
            }

            if (command.PageSize <= 0) command.PageSize = manufacturer.PageSize;


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
                IList<int> filterableSpecificationAttributeOptionIdsFeatured = null;
                var featuredProducts = _productService.SearchProducts(0,
                    manufacturer.Id, true, null, null, 0, null,
                    false, _workContext.WorkingLanguage.Id, null,
                    ProductSortingEnum.Position, 0, int.MaxValue,
                    false, out filterableSpecificationAttributeOptionIdsFeatured);
                model.FeaturedProducts = featuredProducts.Select(x => PrepareProductOverviewModel(x)).ToList();
            }



            //products
            IList<int> filterableSpecificationAttributeOptionIds = null;
            var products = _productService.SearchProducts(0, manufacturer.Id, 
                _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false, 
                minPriceConverted, maxPriceConverted,
                0, string.Empty, false, _workContext.WorkingLanguage.Id, null,
                (ProductSortingEnum)command.OrderBy, command.PageNumber - 1, command.PageSize,
                false, out filterableSpecificationAttributeOptionIds);
            model.Products = products.Select(x => PrepareProductOverviewModel(x)).ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            model.PagingFilteringContext.ViewMode = viewMode;


            //template
            var templateCacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_TEMPLATE_MODEL_KEY, manufacturer.ManufacturerTemplateId);
            var templateViewPath = _cacheManager.Get(templateCacheKey, () =>
            {
                var template = _manufacturerTemplateService.GetManufacturerTemplateById(manufacturer.ManufacturerTemplateId);
                if (template == null)
                    template = _manufacturerTemplateService.GetAllManufacturerTemplates().FirstOrDefault();
                return template.ViewPath;
            });

            return View(templateViewPath, model);
        }

        public ActionResult ManufacturerAll()
        {
            var model = new List<ManufacturerModel>();
            var manufacturers = _manufacturerService.GetAllManufacturers();
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = manufacturer.ToModel();
                
                //prepare picture model
                int pictureSize = _mediaSetting.ManufacturerThumbPictureSize;
                var manufacturerPictureCacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_PICTURE_MODEL_KEY, manufacturer.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured());
                modelMan.PictureModel = _cacheManager.Get(manufacturerPictureCacheKey, () =>
                {
                    var pictureModel = new PictureModel()
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId),
                        ImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageLinkTitleFormat"), modelMan.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageAlternateTextFormat"), modelMan.Name)
                    };
                    return pictureModel;
                });
                model.Add(modelMan);
            }

            return View(model);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult ManufacturerNavigation(int currentManufacturerId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.MANUFACTURER_NAVIGATION_MODEL_KEY, currentManufacturerId, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
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
                    return model;
                });

            return PartialView(cacheModel);
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
            decimal customerEnteredPrice = decimal.Zero;
            decimal customerEnteredPriceConverted = decimal.Zero;
            if (productVariant.CustomerEntersPrice)
            {
                foreach (string formKey in form.AllKeys)
                    if (formKey.Equals(string.Format("price_{0}.CustomerEnteredPrice", productVariantId), StringComparison.InvariantCultureIgnoreCase))
                    {
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

            var addToCartWarnings = new List<string>();
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
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch {}
                            if (selectedDate.HasValue)
                            {
                                selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            var httpPostedFile = this.Request.Files[controlId];
                            if ((httpPostedFile != null) && (!String.IsNullOrEmpty(httpPostedFile.FileName)))
                            {
                                int fileMaxSize = _catalogSettings.FileUploadMaximumSizeBytes;
                                if (httpPostedFile.ContentLength > fileMaxSize)
                                {
                                    addToCartWarnings.Add(string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), (int)(fileMaxSize / 1024)));
                                }
                                else
                                {
                                    //save an uploaded file
                                    var download = new Download()
                                    {
                                        DownloadGuid = Guid.NewGuid(),
                                        UseDownloadUrl = false,
                                        DownloadUrl = "",
                                        DownloadBinary = httpPostedFile.GetDownloadBits(),
                                        ContentType = httpPostedFile.ContentType,
                                        Filename = System.IO.Path.GetFileNameWithoutExtension(httpPostedFile.FileName),
                                        Extension = System.IO.Path.GetExtension(httpPostedFile.FileName),
                                        IsNew = true
                                    };
                                    _downloadService.InsertDownload(download);
                                    //save attribute
                                    selectedAttributes = _productAttributeParser.AddProductAttribute(selectedAttributes,
                                        attribute, download.DownloadGuid.ToString());
                                }
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

            string recipientName = "";
            string recipientEmail = "";
            string senderName = "";
            string senderEmail = "";
            string giftCardMessage = "";
            if (productVariant.IsGiftCard)
            {
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
            addToCartWarnings.AddRange(_shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                productVariant, cartType, attributes, customerEnteredPriceConverted, quantity, true));

            #region Set already entered values

            //set already entered values (quantity, customer entered price, gift card attributes, product attributes
            //we do it manually because views do not use HTML helpers for rendering controls
            
            Action<ProductModel> setEnteredValues = (productModel) =>
                {
                    //find product variant model
                    var productVariantModel = productModel
                        .ProductVariantModels
                        .Where(x => x.Id == productVariant.Id)
                        .FirstOrDefault();
                    if (productVariantModel == null)
                        return;

                    #region 'Add to cart' model

                    //entered quantity
                    productVariantModel.AddToCart.EnteredQuantity = quantity;
                    //customer entered price
                    if (productVariantModel.AddToCart.CustomerEntersPrice)
                    {
                        productVariantModel.AddToCart.CustomerEnteredPrice = customerEnteredPrice;
                    }

                    #endregion

                    #region Gift card attributes

                    if (productVariant.IsGiftCard)
                    {
                        productVariantModel.GiftCard.RecipientName = recipientName;
                        productVariantModel.GiftCard.RecipientEmail = recipientEmail;
                        productVariantModel.GiftCard.SenderName = senderName;
                        productVariantModel.GiftCard.SenderEmail = senderEmail;
                        productVariantModel.GiftCard.Message = giftCardMessage;
                    }

                    #endregion

                    #region Product attributes
                    //clear pre-defined values)
                    foreach (var pvaModel in productVariantModel.ProductVariantAttributes)
                    {
                        foreach (var pvavModel in pvaModel.Values)
                            pvavModel.IsPreSelected = false;
                    }
                    //select the previously entered ones
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
                                        {
                                            var pvavModel = productVariantModel.ProductVariantAttributes
                                                .SelectMany(x => x.Values)
                                                .Where(y => y.Id == selectedAttributeId)
                                                .FirstOrDefault();
                                            if (pvavModel != null)
                                                pvavModel.IsPreSelected = true;
                                        }
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
                                        {
                                            var pvavModel = productVariantModel.ProductVariantAttributes
                                                .SelectMany(x => x.Values)
                                                .Where(y => y.Id == selectedAttributeId)
                                                .FirstOrDefault();
                                            if (pvavModel != null)
                                                pvavModel.IsPreSelected = true;
                                        }
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
                                            int selectedAttributeId = int.Parse(item);
                                            if (selectedAttributeId > 0)
                                            {
                                                var pvavModel = productVariantModel.ProductVariantAttributes
                                                   .SelectMany(x => x.Values)
                                                   .Where(y => y.Id == selectedAttributeId)
                                                   .FirstOrDefault();
                                                if (pvavModel != null)
                                                    pvavModel.IsPreSelected = true;
                                            }
                                        }
                                    }
                                }
                                break;
                            case AttributeControlType.TextBox:
                                {
                                    var txtAttribute = form[controlId];
                                    if (!String.IsNullOrEmpty(txtAttribute))
                                    {
                                        var pvaModel = productVariantModel
                                            .ProductVariantAttributes
                                            .Select(x => x)
                                            .Where(y => y.Id == attribute.Id)
                                            .FirstOrDefault();
                                        
                                        if (pvaModel != null)
                                            pvaModel.TextValue = txtAttribute;
                                    }
                                }
                                break;
                            case AttributeControlType.MultilineTextbox:
                                {
                                    var txtAttribute = form[controlId];
                                    if (!String.IsNullOrEmpty(txtAttribute))
                                    {
                                        var pvaModel = productVariantModel
                                            .ProductVariantAttributes
                                            .Select(x => x)
                                            .Where(y => y.Id == attribute.Id)
                                            .FirstOrDefault();

                                        if (pvaModel != null)
                                            pvaModel.TextValue = txtAttribute;
                                    }
                                }
                                break;
                            case AttributeControlType.Datepicker:
                                {
                                    var pvaModel = productVariantModel
                                        .ProductVariantAttributes
                                        .Select(x => x)
                                        .Where(y => y.Id == attribute.Id)
                                        .FirstOrDefault();
                                    if (pvaModel != null)
                                    {
                                        int day, month, year;
                                        if (int.TryParse(form[controlId + "_day"], out day))
                                            pvaModel.SelectedDay = day;
                                        if (int.TryParse(form[controlId + "_month"], out month))
                                            pvaModel.SelectedMonth = month;
                                        if (int.TryParse(form[controlId + "_year"], out year))
                                            pvaModel.SelectedYear = year;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    #endregion
                };

            #endregion

            #region Return the view

            if (addToCartWarnings.Count == 0)
            {
                switch (cartType)
                {
                    case ShoppingCartType.Wishlist:
                        {
                            if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                            {
                                //redirect to the wishlist page
                                return RedirectToRoute("Wishlist");
                            }
                            else
                            {
                                //redisplay the page with "Product has been added to the wishlist" notification message
                                var model = PrepareProductDetailsPageModel(product);
                                this.SuccessNotification(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist"), false);
                                //set already entered values (quantity, customer entered price, gift card attributes, product attributes)
                                setEnteredValues(model);
                                return View(model.ProductTemplateViewPath, model);
                            }
                        }
                    case ShoppingCartType.ShoppingCart:
                    default:
                        {
                            if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                            {
                                //redirect to the shopping cart page
                                return RedirectToRoute("ShoppingCart");
                            }
                            else
                            {
                                //redisplay the page with "Product has been added to the cart" notification message
                                var model = PrepareProductDetailsPageModel(product);
                                this.SuccessNotification(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart"), false);
                                //set already entered values (quantity, customer entered price, gift card attributes, product attributes)
                                setEnteredValues(model);
                                return View(model.ProductTemplateViewPath, model);
                            }
                        }
                }
            }
            else
            {
                //Errors
                foreach (string error in addToCartWarnings)
                    ModelState.AddModelError("", error);

                //If we got this far, something failed, redisplay form
                var model = PrepareProductDetailsPageModel(product);
                //set already entered values (quantity, customer entered price, gift card attributes, product attributes
                setEnteredValues(model);
                return View(model.ProductTemplateViewPath, model);
            }

            #endregion
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult ProductBreadcrumb(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            if (!_catalogSettings.CategoryBreadcrumbEnabled)
                return Content("");

            var cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_BREADCRUMB_MODEL_KEY, product.Id, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
                {
                    var model = new ProductModel.ProductBreadcrumbModel()
                        {
                            ProductId = product.Id,
                            ProductName = product.GetLocalized(x => x.Name),
                            ProductSeName = product.GetSeName()
                        };
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
                    return model;
                });
            
            return PartialView(cacheModel);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult ProductManufacturers(int productId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_MANUFACTURERS_MODEL_KEY, productId, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = _manufacturerService.GetProductManufacturersByProductId(productId)
                    .Select(x =>
                    {
                        var m = x.Manufacturer.ToModel();
                        return m;
                    })
                    .ToList();
                return model;
            });

            return PartialView(cacheModel);
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

            string cacheKey = string.Format(ModelCacheEventConsumer.PRODUCT_SPECS_MODEL_KEY, product.Id, _workContext.WorkingLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = _specificationAttributeService.GetProductSpecificationAttributesByProductId(product.Id, null, true)
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
                return model;
            });

            return PartialView(cacheModel);
        }

        [ChildActionOnly]
        public ActionResult ProductTierPrices(int productVariantId)
        {
            if (_catalogSettings.IgnoreTierPrices)
                return Content(""); //ignore tier prices

            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                return Content(""); //hide prices

            var variant = _productService.GetProductVariantById(productVariantId);
            if (variant == null)
                throw new ArgumentException("No product variant found with the specified id");

            var model = variant.TierPrices
                .OrderBy(x => x.Quantity)
                .ToList()
                .FilterForCustomer(_workContext.CurrentCustomer)
                .RemoveDuplicatedQuantities()
                .Select(tierPrice =>
                            {
                                var m = new ProductModel.ProductVariantModel.TierPriceModel()
                                {
                                    Quantity = tierPrice.Quantity,
                                };
                                decimal taxRate = decimal.Zero;
                                decimal priceBase = _taxService.GetProductPrice(variant, _priceCalculationService.GetFinalPrice(variant, _workContext.CurrentCustomer, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts, tierPrice.Quantity), out taxRate);
                                    //_taxService.GetProductPrice(variant, tierPrice.Price, out taxRate);
                                decimal price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase, _workContext.WorkingCurrency);
                                m.Price = _priceFormatter.FormatPrice(price, false, false);
                                return m;
                            })
                .ToList();

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult RelatedProducts(int productId, int? productThumbPictureSize)
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


            var model = products.Select(x => PrepareProductOverviewModel(x, true, true, productThumbPictureSize)).ToList();
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult ProductsAlsoPurchased(int productId, int? productThumbPictureSize)
        {
            if (!_catalogSettings.ProductsAlsoPurchasedEnabled)
                return Content("");

            var products = _orderReportService.GetProductsAlsoPurchasedById(productId,
                _catalogSettings.ProductsAlsoPurchasedNumber);

            var model = products.Select(x => PrepareProductOverviewModel(x, true, true, productThumbPictureSize)).ToList();

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
        public ActionResult CrossSellProducts(int? productThumbPictureSize)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            var products = _productService.GetCrosssellProductsByShoppingCart(cart, _shoppingCartSettings.CrossSellsNumber);
            var model = products.Select(x => PrepareProductOverviewModel(x, true, true, productThumbPictureSize))
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
        public ActionResult RecentlyViewedProductsBlock(int? productThumbPictureSize)
        {
            var model = new List<ProductModel>();
            if (_catalogSettings.RecentlyViewedProductsEnabled)
            {
                var products = _recentlyViewedProductsService.GetRecentlyViewedProducts(_catalogSettings.RecentlyViewedProductsNumber);
                foreach (var product in products)
                    model.Add(PrepareProductOverviewModel(product, false, false, productThumbPictureSize));
            }
            return PartialView(model);
        }

        //recently added products
        public ActionResult RecentlyAddedProducts()
        {
            var model = new List<ProductModel>();
            if (_catalogSettings.RecentlyAddedProductsEnabled)
            {
                IList<int> filterableSpecificationAttributeOptionIds = null;
                var products = _productService.SearchProducts(0, 0, null, null,
                    null, 0, null, false, _workContext.WorkingLanguage.Id,
                    null, ProductSortingEnum.CreatedOn, 0, _catalogSettings.RecentlyAddedProductsNumber,
                    false, out filterableSpecificationAttributeOptionIds);
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
            IList<int> filterableSpecificationAttributeOptionIds = null;
            var products = _productService.SearchProducts(0, 0, null, null,
                null, 0, null, false, _workContext.WorkingLanguage.Id,
                null, ProductSortingEnum.CreatedOn, 0, _catalogSettings.RecentlyAddedProductsNumber,
                false, out filterableSpecificationAttributeOptionIds);
            foreach (var product in products)
            {
                string productUrl = Url.RouteUrl("Product", new { productId = product.Id, SeName = product.GetSeName() }, "http");
                items.Add(new SyndicationItem(product.GetLocalized(x => x.Name), product.GetLocalized(x => x.ShortDescription), new Uri(productUrl), String.Format("RecentlyAddedProduct:{0}", product.Id), product.CreatedOnUtc));
            }
            feed.Items = items;
            return new RssActionResult() { Feed = feed };
        }

        [ChildActionOnly]
        public ActionResult HomepageBestSellers(int? productThumbPictureSize)
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
                .Select(x => PrepareProductOverviewModel(x, !_catalogSettings.UseSmallProductBoxOnHomePage, true, productThumbPictureSize))
                .ToList();
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult HomepageProducts(int? productThumbPictureSize)
        {
            var model = new HomePageProductsModel()
            {
                UseSmallProductBox = _catalogSettings.UseSmallProductBoxOnHomePage
            };
            model.Products = _productService.GetAllProductsDisplayedOnHomePage()
                .Select(x => PrepareProductOverviewModel(x, !_catalogSettings.UseSmallProductBoxOnHomePage, true, productThumbPictureSize))
                .ToList();

            return PartialView(model);
        }

        public ActionResult BackInStockSubscribePopup(int productVariantId)
        {
            var variant = _productService.GetProductVariantById(productVariantId);
            if (variant == null || variant.Deleted)
                throw new ArgumentException("No product variant found with the specified id");

            var product = variant.Product;

            var model = new BackInStockSubscribeModel();
            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();
            model.ProductVariantId = variant.Id;
            model.IsCurrentCustomerRegistered = _workContext.CurrentCustomer.IsRegistered();
            model.MaximumBackInStockSubscriptions = _catalogSettings.MaximumBackInStockSubscriptions;
            model.CurrentNumberOfBackInStockSubscriptions = _backInStockSubscriptionService.GetAllSubscriptionsByCustomerId(_workContext.CurrentCustomer.Id, 0, 1).TotalCount;
            if (variant.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                variant.BackorderMode == BackorderMode.NoBackorders &&
                variant.AllowBackInStockSubscriptions &&
                variant.StockQuantity <= 0)
            {
                //out of stock
                model.SubscriptionAllowed = true;
                model.AlreadySubscribed = _backInStockSubscriptionService.FindSubscription(_workContext.CurrentCustomer.Id, variant.Id) != null;
            }
            return View(model);
        }

        [HttpPost, ActionName("BackInStockSubscribePopup")]
        public ActionResult BackInStockSubscribePopupPOST(int productVariantId)
        {
            var variant = _productService.GetProductVariantById(productVariantId);
            if (variant == null || variant.Deleted)
                throw new ArgumentException("No product variant found with the specified id");

            if (!_workContext.CurrentCustomer.IsRegistered())
                return Content(_localizationService.GetResource("BackInStockSubscriptions.OnlyRegistered"));
            
            if (variant.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                variant.BackorderMode == BackorderMode.NoBackorders &&
                variant.AllowBackInStockSubscriptions &&
                variant.StockQuantity <= 0)
            {
                //out of stock
                var subscription = _backInStockSubscriptionService.FindSubscription(_workContext.CurrentCustomer.Id,
                                                                                    variant.Id);
                if (subscription != null)
                {
                    //unsubscribe
                    _backInStockSubscriptionService.DeleteSubscription(subscription);
                    return Content("Unsubscribed");
                }
                else
                {
                    if (_backInStockSubscriptionService.GetAllSubscriptionsByCustomerId(_workContext.CurrentCustomer.Id, 0, 1).TotalCount >= _catalogSettings.MaximumBackInStockSubscriptions)
                        return Content(string.Format(_localizationService.GetResource("BackInStockSubscriptions.MaxSubscriptions"), _catalogSettings.MaximumBackInStockSubscriptions));
            
                    //subscribe   
                    subscription = new BackInStockSubscription()
                    {
                        Customer = _workContext.CurrentCustomer,
                        ProductVariant = variant,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    _backInStockSubscriptionService.InsertSubscription(subscription);
                    return Content("Subscribed");
                }
                
            }
            else
            {
                return Content(_localizationService.GetResource("BackInStockSubscriptions.NotAllowed"));
            }
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

            var cacheKey = string.Format(ModelCacheEventConsumer.PRODUCTTAG_BY_PRODUCT_MODEL_KEY, product.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
                {
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
                    return model;
                });

            return PartialView(cacheModel);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "WorkingLanguage")]
        public ActionResult PopularProductTags()
        {
            var cacheKey = ModelCacheEventConsumer.PRODUCTTAG_POPULAR_MODEL_KEY;
            var cacheModel = _cacheManager.Get(cacheKey, () =>
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
                return model;
            });

            return PartialView(cacheModel);
        }

        public ActionResult ProductsByTag(int productTagId, CatalogPagingFilteringModel command)
        {
            var productTag = _productTagService.GetProductById(productTagId);
            if (productTag == null)
                return RedirectToAction("Index", "Home");
                        
            if (command.PageNumber <= 0) command.PageNumber = 1;

            var model = new ProductsByTagModel()
            {
                TagName = productTag.Name
            };


            //sorting
            model.PagingFilteringContext.AllowProductSorting = _catalogSettings.AllowProductSorting;
            if (model.PagingFilteringContext.AllowProductSorting)
            {
                foreach (ProductSortingEnum enumValue in Enum.GetValues(typeof(ProductSortingEnum)))
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + ((int)enumValue).ToString(), null);

                    var sortValue = enumValue.GetLocalizedEnum(_localizationService, _workContext);
                    model.PagingFilteringContext.AvailableSortOptions.Add(new SelectListItem()
                    {
                        Text = sortValue,
                        Value = sortUrl,
                        Selected = enumValue == (ProductSortingEnum)command.OrderBy
                    });
                }
            }


            //view mode
            model.PagingFilteringContext.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;
            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            if (model.PagingFilteringContext.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                model.PagingFilteringContext.AvailableViewModes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("Categories.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = viewMode == "grid"
                });
                //list
                model.PagingFilteringContext.AvailableViewModes.Add(new SelectListItem()
                {
                    Text = _localizationService.GetResource("Categories.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = viewMode == "list"
                });
            }

            //page size
            model.PagingFilteringContext.AllowCustomersToSelectPageSize = false;
            if (_catalogSettings.ProductsByTagAllowCustomersToSelectPageSize && _catalogSettings.ProductsByTagPageSizeOptions != null)
            {
                var pageSizes = _catalogSettings.ProductsByTagPageSizeOptions.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default ('products by tag' page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        int temp = 0;

                        if (int.TryParse(pageSizes.FirstOrDefault(), out temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "pagesize={0}", null);
                    sortUrl = _webHelper.RemoveQueryString(sortUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        int temp = 0;
                        if (!int.TryParse(pageSize, out temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        model.PagingFilteringContext.PageSizeOptions.Add(new SelectListItem()
                        {
                            Text = pageSize,
                            Value = String.Format(sortUrl, pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (model.PagingFilteringContext.PageSizeOptions.Any())
                    {
                        model.PagingFilteringContext.PageSizeOptions = model.PagingFilteringContext.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        model.PagingFilteringContext.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(model.PagingFilteringContext.PageSizeOptions.FirstOrDefault().Text);
                        }
                    }
                }
            }

            if (command.PageSize <= 0) command.PageSize = _catalogSettings.ProductsByTagPageSize;

            //products
            IList<int> filterableSpecificationAttributeOptionIds = null;
            var products = _productService.SearchProducts(0, 0, false, null, null,
                productTag.Id, string.Empty, false, _workContext.WorkingLanguage.Id, null,
                (ProductSortingEnum)command.OrderBy, command.PageNumber - 1, command.PageSize,
                false, out filterableSpecificationAttributeOptionIds);
            model.Products = products.Select(x => PrepareProductOverviewModel(x)).ToList();

            model.PagingFilteringContext.LoadPagedList(products);
            model.PagingFilteringContext.ViewMode = viewMode;
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
            model.AddProductReview.Rating = _catalogSettings.DefaultProductRatingValue;
            return View(model);
        }

        [HttpPost, ActionName("ProductReviews")]
        [FormValueRequired("add-review")]
        public ActionResult ProductReviewsAdd(int productId, ProductReviewsModel model)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews)
                return RedirectToAction("Index", "Home");

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Reviews.OnlyRegisteredUsersCanWriteReviews"));
            }

            if (ModelState.IsValid)
            {
                //save review
                int rating = model.AddProductReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
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
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage;
            return View(model);
        }

        [HttpPost, ActionName("ProductEmailAFriend")]
        [FormValueRequired("send-email")]
        [CaptchaValidator]
        public ActionResult ProductEmailAFriendSend(ProductEmailAFriendModel model, bool captchaValid)
        {
            var product = _productService.GetProductById(model.ProductId);
            if (product == null || product.Deleted || !product.Published || !_catalogSettings.EmailAFriendEnabled)
                return RedirectToAction("Index", "Home");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            if (_workContext.CurrentCustomer.IsGuest() && !_catalogSettings.AllowAnonymousUsersToEmailAFriend)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Products.EmailAFriend.OnlyRegisteredUsers"));
            }

            if (ModelState.IsValid)
            {
                //email
                _workflowMessageService.SendProductEmailAFriendMessage(_workContext.CurrentCustomer,
                        _workContext.WorkingLanguage.Id, product,
                        model.YourEmailAddress, model.FriendEmail, 
                        Core.Html.HtmlHelper.FormatText(model.PersonalMessage, false, true, false, false, false, false));

                model.ProductId = product.Id;
                model.ProductName = product.GetLocalized(x => x.Name);
                model.ProductSeName = product.GetSeName();

                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("Products.EmailAFriend.SuccessfullySent");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model.ProductId = product.Id;
            model.ProductName = product.GetLocalized(x => x.Name);
            model.ProductSeName = product.GetSeName();
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailProductToFriendPage;
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

            //'Continue shopping' URL
            _customerService.SaveCustomerAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.LastContinueShoppingPage, _webHelper.GetThisPageUrl(false));

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
                    var categoryIds = new List<int>();
                    int manufacturerId = 0;
                    decimal? minPriceConverted = null;
                    decimal? maxPriceConverted = null;
                    bool searchInDescriptions = false;
                    if (model.As)
                    {
                        //advanced search
                        var categoryId = model.Cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (model.Isc)
                            {
                                //include subcategories
                                categoryIds.AddRange(GetChildCategoryIds(categoryId));
                            }
                        }


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
                    IList<int> filterableSpecificationAttributeOptionIds = null;
                    products = _productService.SearchProducts(categoryIds, manufacturerId, null,
                        minPriceConverted, maxPriceConverted, 0,
                        model.Q, searchInDescriptions, _workContext.WorkingLanguage.Id, null,
                        ProductSortingEnum.Position, command.PageNumber - 1, command.PageSize,
                        false, out filterableSpecificationAttributeOptionIds);
                    model.Products = products.Select(x => PrepareProductOverviewModel(x)).ToList();

                    model.NoResults = !model.Products.Any();                    
                }
            }

            model.PagingFilteringContext.LoadPagedList(products);
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult SearchBox()
        {
            return PartialView();
        }

        #endregion
    }
}
