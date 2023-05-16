using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.JsonLD;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the JSON-LD model factory implementation
    /// </summary>
    public partial class JsonLdModelFactory : IJsonLdModelFactory
    {
        #region Fields

        protected readonly IEventPublisher _eventPublisher;
        protected readonly INopUrlHelper _nopUrlHelper;
        protected readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public JsonLdModelFactory(IEventPublisher eventPublisher,
            INopUrlHelper nopUrlHelper,
            IWebHelper webHelper)
        {
            _eventPublisher = eventPublisher;
            _nopUrlHelper = nopUrlHelper;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare JSON-LD category breadcrumb model
        /// </summary>
        /// <param name="categoryModels">List of category models</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains JSON-LD category breadcrumb model
        /// </returns>
        protected virtual async Task<JsonLdBreadcrumbListModel> PrepareJsonLdBreadcrumbListAsync(IList<CategorySimpleModel> categoryModels)
        {
            var breadcrumbList = new JsonLdBreadcrumbListModel();
            var position = 1;

            foreach (var cat in categoryModels)
            {
                var breadcrumbListItem = new JsonLdBreadcrumbListItemModel
                {
                    Position = position,
                    Item = new JsonLdBreadcrumbItemModel
                    {
                        Id = await _nopUrlHelper.RouteGenericUrlAsync<Category>(new { SeName = cat.SeName }, _webHelper.GetCurrentRequestProtocol()),
                        Name = cat.Name
                    }
                };
                breadcrumbList.ItemListElement.Add(breadcrumbListItem);
                position++;
            }

            return breadcrumbList;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare JSON-LD category breadcrumb model
        /// </summary>
        /// <param name="categoryModels">List of category models</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains JSON-LD category breadcrumb model
        /// </returns>
        public virtual async Task<JsonLdBreadcrumbListModel> PrepareJsonLdCategoryBreadcrumbAsync(IList<CategorySimpleModel> categoryModels)
        {
            var breadcrumbList = await PrepareJsonLdBreadcrumbListAsync(categoryModels);

            await _eventPublisher.PublishAsync(new JsonLdCreatedEvent(breadcrumbList));

            return breadcrumbList;
        }

        /// <summary>
        /// Prepare JSON-LD product breadcrumb model
        /// </summary>
        /// <param name="breadcrumbModel">Product breadcrumb model</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains JSON-LD product breadcrumb model
        /// </returns>
        public virtual async Task<JsonLdBreadcrumbListModel> PrepareJsonLdProductBreadcrumbAsync(ProductDetailsModel.ProductBreadcrumbModel breadcrumbModel)
        {
            var breadcrumbList = await PrepareJsonLdBreadcrumbListAsync(breadcrumbModel.CategoryBreadcrumb);

            breadcrumbList.ItemListElement.Add(new JsonLdBreadcrumbListItemModel
            {
                Position = breadcrumbList.ItemListElement.Count + 1,
                Item = new JsonLdBreadcrumbItemModel
                {
                    Id = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = breadcrumbModel.ProductSeName }, _webHelper.GetCurrentRequestProtocol()),
                    Name = breadcrumbModel.ProductName,
                }
            });

            await _eventPublisher.PublishAsync(new JsonLdCreatedEvent(breadcrumbList));

            return breadcrumbList;
        }

        /// <summary>
        /// Prepare JSON-LD product model
        /// </summary>
        /// <param name="model">Product details model</param>
        /// <param name="productUrl">Product URL</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result contains JSON-LD product model
        /// </returns>
        public virtual async Task<JsonLdProductModel> PrepareJsonLdProductAsync(ProductDetailsModel model, string productUrl = null)
        {
            productUrl ??= await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = model.SeName }, _webHelper.GetCurrentRequestProtocol());

            var productPrice = model.AssociatedProducts.Any()
                ? model.AssociatedProducts.Min(associatedProduct => associatedProduct.ProductPrice.PriceValue)
                : model.ProductPrice.PriceValue;

            var product = new JsonLdProductModel
            {
                Name = model.Name,
                Sku = model.Sku,
                Gtin = model.Gtin,
                Mpn = model.ManufacturerPartNumber,
                Description = model.ShortDescription,
                Image = model.DefaultPictureModel.ImageUrl,
                Offer = new JsonLdOfferModel
                {
                    Url = productUrl.ToLowerInvariant(),
                    Price = model.ProductPrice.CallForPrice ? null : productPrice.ToString("0.00", CultureInfo.InvariantCulture),
                    PriceCurrency = model.ProductPrice.CurrencyCode,
                    PriceValidUntil = model.AvailableEndDate,
                    Availability = @"https://schema.org/" + (model.InStock ? "InStock" : "OutOfStock")
                },
                Brand = model.ProductManufacturers?.Select(manufacturer => new JsonLdBrandModel { Name = manufacturer.Name }).ToList()
            };

            if (model.ProductReviewOverview.TotalReviews > 0)
            {
                var ratingPercent = 0;
                if (model.ProductReviewOverview.TotalReviews != 0)
                {
                    ratingPercent = ((model.ProductReviewOverview.RatingSum * 100) / model.ProductReviewOverview.TotalReviews) / 5;
                }
                var ratingValue = ratingPercent / (decimal)20;

                product.AggregateRating = new JsonLdAggregateRatingModel
                {
                    RatingValue = ratingValue.ToString("0.0", CultureInfo.InvariantCulture),
                    ReviewCount = model.ProductReviewOverview.TotalReviews
                };

                product.Review = model.ProductReviews.Items?.Select(review => new JsonLdReviewModel
                {
                    Name = review.Title,
                    ReviewBody = review.ReviewText,
                    ReviewRating = new JsonLdRatingModel
                    {
                        RatingValue = review.Rating
                    },
                    Author = new JsonLdPersonModel { Name = review.CustomerName },
                    DatePublished = review.WrittenOnStr
                }).ToList();
            }

            foreach (var associatedProduct in model.AssociatedProducts)
            {
                var parentUrl = !associatedProduct.VisibleIndividually ? productUrl : null;
                product.HasVariant.Add(await PrepareJsonLdProductAsync(associatedProduct, parentUrl));
            }

            await _eventPublisher.PublishAsync(new JsonLdCreatedEvent(product));

            return product;
        }

        #endregion
    }
}