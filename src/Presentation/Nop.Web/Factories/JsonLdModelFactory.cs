using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.JsonLD;

namespace Nop.Web.Factories
{
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
        /// Prepare JsonLD breadcrumb list
        /// </summary>
        /// <param name="categoryBreadcrumb">List CategorySimpleModel</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result JsonLD Breadbrumb list
        /// </returns>
        protected async Task<JsonLdBreadcrumbListModel> PrepareJsonLdBreadcrumbListAsync(IList<CategorySimpleModel> categoryBreadcrumb)
        {
            var breadcrumbList = new JsonLdBreadcrumbListModel();
            var position = 1;

            foreach (var cat in categoryBreadcrumb)
            {
                var breadcrumbListItem = new JsonLdBreadcrumbListItemModel()
                {
                    Position = position,
                    Item = new JsonLdBreadcrumbItemModel()
                    {
                        Id = await _nopUrlHelper.RouteGenericUrlAsync<Category>(new { SeName = cat.SeName }),
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
        /// Prepare category breadcrumb JsonLD
        /// </summary>
        /// <param name="categoryBreadcrumb">List CategorySimpleModel</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result JsonLD Breadbrumb list
        /// </returns>
        public async Task<JsonLdBreadcrumbListModel> PrepareJsonLdBreadcrumbCategoryAsync(IList<CategorySimpleModel> categoryBreadcrumb)
        {
            var breadcrumbList = await PrepareJsonLdBreadcrumbListAsync(categoryBreadcrumb);
            await _eventPublisher.PublishAsync(new JsonLdCreatedEvent(breadcrumbList));

            return breadcrumbList;
        }

        /// <summary>
        /// Prepare product breadcrumb JsonLD
        /// </summary>
        /// <param name="breadcrumbModel">Product breadcrumb model</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result JsonLD breadcrumb list
        /// </returns>
        public async Task<JsonLdBreadcrumbListModel> PrepareJsonLdBreadcrumbProductAsync(ProductDetailsModel.ProductBreadcrumbModel breadcrumbModel)
        {
            var breadcrumbList = await PrepareJsonLdBreadcrumbListAsync(breadcrumbModel.CategoryBreadcrumb);
            breadcrumbList.ItemListElement.Add(
                new JsonLdBreadcrumbListItemModel()
                {
                    Position = breadcrumbList.ItemListElement.Count + 1,
                    Item = new JsonLdBreadcrumbItemModel()
                    {
                        Id = await _nopUrlHelper.RouteGenericUrlAsync<Category>(new { SeName = breadcrumbModel.ProductSeName }),
                        Name = breadcrumbModel.ProductName,
                    }
                });

            await _eventPublisher.PublishAsync(new JsonLdCreatedEvent(breadcrumbList));

            return breadcrumbList;
        }

        /// <summary>
        /// Prepare JsonLD product
        /// </summary>
        /// <param name="model">Product details model</param>
        /// <returns>A task that represents the asynchronous operation
        /// The task result JsonLD product
        /// </returns>
        public virtual async Task<JsonLdProductModel> PrepareJsonLdProductAsync(ProductDetailsModel model)
        {
            var productUrl = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = model.SeName });
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
                Image = model.DefaultPictureModel.ImageUrl
            };

            foreach (var manufacturer in model.ProductManufacturers)
            {
                product.Brand.Add(new JsonLdBrandModel() { Name = manufacturer.Name });
            }

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
                    RatingValue = ratingValue.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture),
                    ReviewCount = model.ProductReviewOverview.TotalReviews
                };
            }
            product.Offer = new JsonLdOfferModel()
            {
                Url = productUrl.ToString(),
                Price = model.ProductPrice.CallForPrice ? null : productPrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                PriceCurrency = model.ProductPrice.CurrencyCode,
                PriceValidUntil = model.AvailableEndDate,
                Availability = @"https://schema.org/" + (model.InStock ? "InStock" : "OutOfStock")
            };

            foreach (var associatedProduct in model.AssociatedProducts)
                product.IsAccessoryOrSparePartFor.Add(await PrepareJsonLdProductAsync(associatedProduct));


            if (model.ProductReviewOverview.TotalReviews > 0)
            {
                foreach (var review in model.ProductReviews.Items)
                {
                    product.Review.Add(new JsonLdReviewModel()
                    {
                        Name = review.Title,
                        ReviewBody = review.ReviewText,
                        ReviewRating = new JsonLdRatingModel()
                        {
                            RatingValue = review.Rating
                        },
                        Author = new JsonLdPersonModel() { Name = review.CustomerName },
                        DatePublished = review.WrittenOnStr
                    });
                }
            }

            await _eventPublisher.PublishAsync(new JsonLdCreatedEvent(product));

            return product;
        }

        #endregion
    }
}
