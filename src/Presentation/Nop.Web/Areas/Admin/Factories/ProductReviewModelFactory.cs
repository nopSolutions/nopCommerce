using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Html;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the product review model factory implementation
    /// </summary>
    public partial class ProductReviewModelFactory : IProductReviewModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ProductReviewModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IProductService productService,
            IReviewTypeService reviewTypeService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _productService = productService;
            _reviewTypeService = reviewTypeService;
            _storeService = storeService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare product review search model
        /// </summary>
        /// <param name="searchModel">Product review search model</param>
        /// <returns>Product review search model</returns>
        public virtual ProductReviewSearchModel PrepareProductReviewSearchModel(ProductReviewSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.ProductReviews.List.SearchApproved.All"),
                Value = "0"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.ProductReviews.List.SearchApproved.ApprovedOnly"),
                Value = "1"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.ProductReviews.List.SearchApproved.DisapprovedOnly"),
                Value = "2"
            });

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product review list model
        /// </summary>
        /// <param name="searchModel">Product review search model</param>
        /// <returns>Product review list model</returns>
        public virtual ProductReviewListModel PrepareProductReviewListModel(ProductReviewSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter reviews
            var createdOnFromValue = !searchModel.CreatedOnFrom.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone);
            var createdToFromValue = !searchModel.CreatedOnTo.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;
            var vendorId = _workContext.CurrentVendor?.Id ?? 0;

            //get product reviews
            var productReviews = _productService.GetAllProductReviews(showHidden: true,
                customerId: 0,
                approved: isApprovedOnly,
                fromUtc: createdOnFromValue,
                toUtc: createdToFromValue,
                message: searchModel.SearchText,
                storeId: searchModel.SearchStoreId,
                productId: searchModel.SearchProductId,
                vendorId: vendorId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new ProductReviewListModel().PrepareToGrid(searchModel, productReviews, () =>
            {
                return productReviews.Select(productReview =>
                {
                    //fill in model values from the entity
                    var productReviewModel = productReview.ToModel<ProductReviewModel>();

                    //convert dates to the user time
                    productReviewModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(productReview.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    productReviewModel.StoreName = _storeService.GetStoreById(productReview.StoreId)?.Name;
                    productReviewModel.ProductName = _productService.GetProductById(productReview.ProductId)?.Name;
                    productReviewModel.CustomerInfo = _customerService.GetCustomerById(productReview.CustomerId) is Customer customer && _customerService.IsRegistered(customer)
                        ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    productReviewModel.ReviewText = HtmlHelper.FormatText(productReview.ReviewText, false, true, false, false, false, false);
                    productReviewModel.ReplyText = HtmlHelper.FormatText(productReview.ReplyText, false, true, false, false, false, false);

                    return productReviewModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare product review model
        /// </summary>
        /// <param name="model">Product review model</param>
        /// <param name="productReview">Product review</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Product review model</returns>
        public virtual ProductReviewModel PrepareProductReviewModel(ProductReviewModel model,
            ProductReview productReview, bool excludeProperties = false)
        {
            if (productReview != null)
            {
                //fill in model values from the entity
                model ??= new ProductReviewModel
                {
                    Id = productReview.Id,
                    StoreName = _storeService.GetStoreById(productReview.StoreId)?.Name,
                    ProductId = productReview.ProductId,
                    ProductName = _productService.GetProductById(productReview.ProductId)?.Name,
                    CustomerId = productReview.CustomerId,
                    Rating = productReview.Rating
                };

                model.CustomerInfo = _customerService.GetCustomerById(productReview.CustomerId) is Customer customer && _customerService.IsRegistered(customer)
                    ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");

                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(productReview.CreatedOnUtc, DateTimeKind.Utc);

                if (!excludeProperties)
                {
                    model.Title = productReview.Title;
                    model.ReviewText = productReview.ReviewText;
                    model.ReplyText = productReview.ReplyText;
                    model.IsApproved = productReview.IsApproved;
                }

                //prepare nested search model
                PrepareProductReviewReviewTypeMappingSearchModel(model.ProductReviewReviewTypeMappingSearchModel, productReview);
            }

            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            return model;
        }

        /// <summary>
        /// Prepare product review mapping search model
        /// </summary>
        /// <param name="searchModel">Product review mapping search model</param>
        /// <param name="productReview">Product</param>
        /// <returns>Product review mapping search model</returns>
        public virtual ProductReviewReviewTypeMappingSearchModel PrepareProductReviewReviewTypeMappingSearchModel(ProductReviewReviewTypeMappingSearchModel searchModel,
            ProductReview productReview)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            searchModel.ProductReviewId = productReview.Id;

            searchModel.IsAnyReviewTypes = _reviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewId(productReview.Id).Any();

            //prepare page parameters
            searchModel.SetGridPageSize();            

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product reviews mapping list model
        /// </summary>
        /// <param name="searchModel">Product review and review type mapping search model</param>
        /// <param name="productReview">Product review</param>
        /// <returns>Product review and review type mapping list model</returns>
        public virtual ProductReviewReviewTypeMappingListModel PrepareProductReviewReviewTypeMappingListModel(ProductReviewReviewTypeMappingSearchModel searchModel, ProductReview productReview)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            //get product review and review type mappings
            var productReviewReviewTypeMappings = _reviewTypeService
                .GetProductReviewReviewTypeMappingsByProductReviewId(productReview.Id).ToPagedList(searchModel);

            //prepare grid model
            var model = new ProductReviewReviewTypeMappingListModel().PrepareToGrid(searchModel, productReviewReviewTypeMappings, () =>
            {
                return productReviewReviewTypeMappings.Select(productReviewReviewTypeMapping =>
                {
                    //fill in model values from the entity
                    var productReviewReviewTypeMappingModel = productReviewReviewTypeMapping
                        .ToModel<ProductReviewReviewTypeMappingModel>();

                    //fill in additional values (not existing in the entity)
                    var reviewType =
                        _reviewTypeService.GetReviewTypeById(productReviewReviewTypeMapping.ReviewTypeId);

                    productReviewReviewTypeMappingModel.Name =
                        _localizationService.GetLocalized(reviewType, entity => entity.Name);
                    productReviewReviewTypeMappingModel.Description =
                        _localizationService.GetLocalized(reviewType, entity => entity.Description);
                    productReviewReviewTypeMappingModel.VisibleToAllCustomers =
                        reviewType.VisibleToAllCustomers;

                    return productReviewReviewTypeMappingModel;
                });
            });

            return model;
        }

        #endregion
    }
}