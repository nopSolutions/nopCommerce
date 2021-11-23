using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
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

        protected CatalogSettings CatalogSettings { get; }
        protected IBaseAdminModelFactory BaseAdminModelFactory { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected ILocalizationService LocalizationService { get; }
        private readonly INopHtmlHelper _nopHtmlHelper;
        protected IProductService ProductService { get; }
        protected IReviewTypeService ReviewTypeService { get; }
        protected IStoreService StoreService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public ProductReviewModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            INopHtmlHelper nopHtmlHelper,
            IProductService productService,
            IReviewTypeService reviewTypeService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            CatalogSettings = catalogSettings;
            BaseAdminModelFactory = baseAdminModelFactory;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            LocalizationService = localizationService;
            _nopHtmlHelper = nopHtmlHelper;
            ProductService = productService;
            ReviewTypeService = reviewTypeService;
            StoreService = storeService;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare product review search model
        /// </summary>
        /// <param name="searchModel">Product review search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product review search model
        /// </returns>
        public virtual async Task<ProductReviewSearchModel> PrepareProductReviewSearchModelAsync(ProductReviewSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await WorkContext.GetCurrentVendorAsync() != null;

            //prepare available stores
            await BaseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare "approved" property (0 - all; 1 - approved only; 2 - disapproved only)
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.Catalog.ProductReviews.List.SearchApproved.All"),
                Value = "0"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.Catalog.ProductReviews.List.SearchApproved.ApprovedOnly"),
                Value = "1"
            });
            searchModel.AvailableApprovedOptions.Add(new SelectListItem
            {
                Text = await LocalizationService.GetResourceAsync("Admin.Catalog.ProductReviews.List.SearchApproved.DisapprovedOnly"),
                Value = "2"
            });

            searchModel.HideStoresList = CatalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product review list model
        /// </summary>
        /// <param name="searchModel">Product review search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product review list model
        /// </returns>
        public virtual async Task<ProductReviewListModel> PrepareProductReviewListModelAsync(ProductReviewSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter reviews
            var createdOnFromValue = !searchModel.CreatedOnFrom.HasValue ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await DateTimeHelper.GetCurrentTimeZoneAsync());
            var createdToFromValue = !searchModel.CreatedOnTo.HasValue ? null
                : (DateTime?)DateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await DateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var isApprovedOnly = searchModel.SearchApprovedId == 0 ? null : searchModel.SearchApprovedId == 1 ? true : (bool?)false;
            var vendor = await WorkContext.GetCurrentVendorAsync();
            var vendorId = vendor?.Id ?? 0;

            //get product reviews
            var productReviews = await ProductService.GetAllProductReviewsAsync(showHidden: true,
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
            var model = await new ProductReviewListModel().PrepareToGridAsync(searchModel, productReviews, () =>
            {
                return productReviews.SelectAwait(async productReview =>
                {
                    //fill in model values from the entity
                    var productReviewModel = productReview.ToModel<ProductReviewModel>();

                    //convert dates to the user time
                    productReviewModel.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(productReview.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    productReviewModel.StoreName = (await StoreService.GetStoreByIdAsync(productReview.StoreId))?.Name;
                    productReviewModel.ProductName = (await ProductService.GetProductByIdAsync(productReview.ProductId))?.Name;
                    productReviewModel.CustomerInfo = (await CustomerService.GetCustomerByIdAsync(productReview.CustomerId)) is Customer customer && (await CustomerService.IsRegisteredAsync(customer))
                        ? customer.Email
                        : await LocalizationService.GetResourceAsync("Admin.Customers.Guest");

                    productReviewModel.ReviewText = _nopHtmlHelper.FormatText(productReview.ReviewText, false, true, false, false, false, false);
                    productReviewModel.ReplyText = _nopHtmlHelper.FormatText(productReview.ReplyText, false, true, false, false, false, false);

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product review model
        /// </returns>
        public virtual async Task<ProductReviewModel> PrepareProductReviewModelAsync(ProductReviewModel model,
            ProductReview productReview, bool excludeProperties = false)
        {
            if (productReview != null)
            {
                var showStoreName = (await StoreService.GetAllStoresAsync()).Count > 1;

                //fill in model values from the entity
                model ??= new ProductReviewModel
                {
                    Id = productReview.Id,
                    StoreName = showStoreName ? (await StoreService.GetStoreByIdAsync(productReview.StoreId))?.Name : string.Empty,
                    ProductId = productReview.ProductId,
                    ProductName = (await ProductService.GetProductByIdAsync(productReview.ProductId))?.Name,
                    CustomerId = productReview.CustomerId,
                    Rating = productReview.Rating
                };

                model.ShowStoreName = showStoreName;

                model.CustomerInfo = await CustomerService.GetCustomerByIdAsync(productReview.CustomerId) is Customer customer && await CustomerService.IsRegisteredAsync(customer)
                    ? customer.Email : await LocalizationService.GetResourceAsync("Admin.Customers.Guest");

                model.CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(productReview.CreatedOnUtc, DateTimeKind.Utc);

                if (!excludeProperties)
                {
                    model.Title = productReview.Title;
                    model.ReviewText = productReview.ReviewText;
                    model.ReplyText = productReview.ReplyText;
                    model.IsApproved = productReview.IsApproved;
                }

                //prepare nested search model
                await PrepareProductReviewReviewTypeMappingSearchModelAsync(model.ProductReviewReviewTypeMappingSearchModel, productReview);
            }

            model.IsLoggedInAsVendor = await WorkContext.GetCurrentVendorAsync() != null;

            return model;
        }

        /// <summary>
        /// Prepare product review mapping search model
        /// </summary>
        /// <param name="searchModel">Product review mapping search model</param>
        /// <param name="productReview">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product review mapping search model
        /// </returns>
        public virtual async Task<ProductReviewReviewTypeMappingSearchModel> PrepareProductReviewReviewTypeMappingSearchModelAsync(ProductReviewReviewTypeMappingSearchModel searchModel,
            ProductReview productReview)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            searchModel.ProductReviewId = productReview.Id;

            searchModel.IsAnyReviewTypes = (await ReviewTypeService.GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReview.Id)).Any();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product reviews mapping list model
        /// </summary>
        /// <param name="searchModel">Product review and review type mapping search model</param>
        /// <param name="productReview">Product review</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product review and review type mapping list model
        /// </returns>
        public virtual async Task<ProductReviewReviewTypeMappingListModel> PrepareProductReviewReviewTypeMappingListModelAsync(ProductReviewReviewTypeMappingSearchModel searchModel, ProductReview productReview)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            //get product review and review type mappings
            var productReviewReviewTypeMappings = (await ReviewTypeService
                .GetProductReviewReviewTypeMappingsByProductReviewIdAsync(productReview.Id)).ToPagedList(searchModel);

            //prepare grid model
            var model = await new ProductReviewReviewTypeMappingListModel().PrepareToGridAsync(searchModel, productReviewReviewTypeMappings, () =>
            {
                return productReviewReviewTypeMappings.SelectAwait(async productReviewReviewTypeMapping =>
                {
                    //fill in model values from the entity
                    var productReviewReviewTypeMappingModel = productReviewReviewTypeMapping
                        .ToModel<ProductReviewReviewTypeMappingModel>();

                    //fill in additional values (not existing in the entity)
                    var reviewType = await ReviewTypeService.GetReviewTypeByIdAsync(productReviewReviewTypeMapping.ReviewTypeId);

                    productReviewReviewTypeMappingModel.Name = await LocalizationService.GetLocalizedAsync(reviewType, entity => entity.Name);
                    productReviewReviewTypeMappingModel.Description = await LocalizationService.GetLocalizedAsync(reviewType, entity => entity.Description);
                    productReviewReviewTypeMappingModel.VisibleToAllCustomers = reviewType.VisibleToAllCustomers;

                    return productReviewReviewTypeMappingModel;
                });
            });

            return model;
        }

        #endregion
    }
}