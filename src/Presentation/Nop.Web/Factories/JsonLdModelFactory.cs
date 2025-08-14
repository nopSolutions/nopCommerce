using System.Globalization;
using System.Text.Encodings.Web;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Forums;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Boards;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.JsonLD;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the JSON-LD model factory implementation
/// </summary>
public partial class JsonLdModelFactory : IJsonLdModelFactory
{
    #region Fields

    protected readonly ForumSettings _forumSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IForumService _forumService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public JsonLdModelFactory(ForumSettings forumSettings,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IEventPublisher eventPublisher,
        IForumService forumService,
        INopUrlHelper nopUrlHelper,
        IWebHelper webHelper)
    {
        _forumSettings = forumSettings;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _eventPublisher = eventPublisher;
        _forumService = forumService;
        _nopUrlHelper = nopUrlHelper;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Convert datetime to an ISO 8601 string
    /// </summary>
    /// <param name="dateTime">Datetime object to convert</param>
    /// <returns>Datetime string in the ISO 8601 format</returns>
    protected virtual string ConvertDateTimeToIso8601String(DateTime? dateTime)
    {
        return dateTime == null ? null : new DateTimeOffset(dateTime.Value).ToString("O", CultureInfo.InvariantCulture);
    }

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

        await _eventPublisher.PublishAsync(new JsonLdCreatedEvent<JsonLdBreadcrumbListModel>(breadcrumbList));

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

        await _eventPublisher.PublishAsync(new JsonLdCreatedEvent<JsonLdBreadcrumbListModel>(breadcrumbList));

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
                Price = model.ProductPrice.CallForPrice ? null : productPrice?.ToString("0.00", CultureInfo.InvariantCulture),
                PriceCurrency = model.ProductPrice.CurrencyCode,
                PriceValidUntil = ConvertDateTimeToIso8601String(model.AvailableEndDate),
                Availability = @"https://schema.org/" + (model.InStock ? "InStock" : "OutOfStock")
            },
            Brand = model.ProductManufacturers?.Select(manufacturer => new JsonLdBrandModel { Name = manufacturer.Name }).ToList()
        };

        if (model.ProductReviewOverview.TotalReviews > 0)
        {
            var ratingPercent = model.ProductReviewOverview.RatingSum * 100 / model.ProductReviewOverview.TotalReviews / 5;

            var ratingValue = ratingPercent / (decimal)20;

            product.AggregateRating = new JsonLdAggregateRatingModel
            {
                RatingValue = ratingValue.ToString("0.0", CultureInfo.InvariantCulture),
                ReviewCount = model.ProductReviewOverview.TotalReviews
            };

            product.Review = model.ProductReviews.Items?.Select(review => new JsonLdReviewModel
            {
                Name = JavaScriptEncoder.Default.Encode(review.Title),
                ReviewBody = JavaScriptEncoder.Default.Encode(review.ReviewText),
                ReviewRating = new JsonLdRatingModel
                {
                    RatingValue = review.Rating
                },
                Author = new JsonLdPersonModel { Name = JavaScriptEncoder.Default.Encode(review.CustomerName) },
                DatePublished = ConvertDateTimeToIso8601String(review.WrittenOn)
            }).ToList();
        }

        foreach (var associatedProduct in model.AssociatedProducts)
        {
            var parentUrl = !associatedProduct.VisibleIndividually ? productUrl : null;
            product.HasVariant.Add(await PrepareJsonLdProductAsync(associatedProduct, parentUrl));
        }

        await _eventPublisher.PublishAsync(new JsonLdCreatedEvent<JsonLdProductModel>(product));

        return product;
    }

    /// <summary>
    /// Prepare JSON-LD forum topic model
    /// </summary>
    /// <param name="forumTopic">Forum topic</param>
    /// <param name="firstPost">The first post on forum topic</param>
    /// <param name="model">Forum topic page model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains JSON-LD forum topic model
    /// </returns>
    public virtual async Task<JsonLdForumTopicModel> PrepareJsonLdForumTopicAsync(ForumTopic forumTopic,
        ForumPost firstPost,
        ForumTopicPageModel model)
    {
        var forumTopicCustomer = await _customerService.GetCustomerByIdAsync(forumTopic.CustomerId);
        var customerName = await _customerService.FormatUsernameAsync(forumTopicCustomer);
        var createdOn = await _dateTimeHelper.ConvertToUserTimeAsync(forumTopic.CreatedOnUtc, DateTimeKind.Utc);

        var forumTopicModel = new JsonLdForumTopicModel
        {
            Author = new()
            {
                Name = JavaScriptEncoder.Default.Encode(customerName),
                Url =
                    _nopUrlHelper.RouteUrl(NopRouteNames.Standard.CUSTOMER_PROFILE, new { id = forumTopic.CustomerId },
                        _webHelper.GetCurrentRequestProtocol()),
            },
            DatePublished = ConvertDateTimeToIso8601String(createdOn),
            Subject = JavaScriptEncoder.Default.Encode(model.Subject),
            Text = _forumService.FormatPostText(firstPost),
            Url = _nopUrlHelper.RouteUrl(NopRouteNames.Standard.TOPIC_SLUG, new { id = model.Id, slug = model.SeName },
                _webHelper.GetCurrentRequestProtocol()),
            Comments = model.ForumPostModels.Where(pm => pm.Id != firstPost.Id).Select(postModel =>
            {
                var commentModel = new JsonLdForumTopicCommentModel
                {
                    Author = new()
                    {
                        Name = JavaScriptEncoder.Default.Encode(postModel.CustomerName),
                        Url = _nopUrlHelper.RouteUrl(NopRouteNames.Standard.CUSTOMER_PROFILE, new { id = postModel.CustomerId },
                            _webHelper.GetCurrentRequestProtocol()),
                    },
                    DatePublished = ConvertDateTimeToIso8601String(postModel.PostCreatedOn),
                    Url = postModel.CurrentTopicPage > 1
                        ? _nopUrlHelper.RouteUrl(NopRouteNames.Standard.TOPIC_SLUG_PAGED, new { id = model.Id, slug = model.SeName, pageNumber = postModel.CurrentTopicPage }, _webHelper.GetCurrentRequestProtocol(), null, postModel.Id.ToString())
                        : _nopUrlHelper.RouteUrl( NopRouteNames.Standard.TOPIC_SLUG, new { id = model.Id, slug = model.SeName }, _webHelper.GetCurrentRequestProtocol(), null, postModel.Id.ToString()),
                    Text = postModel.FormattedText
                };

                if (_forumSettings.AllowPostVoting)
                {
                    commentModel.InteractionStatistic = new()
                    {
                        InteractionType =
                            postModel.VoteCount >= 0
                                ? "https://schema.org/LikeAction"
                                : "https://schema.org/DislikeAction",
                        UserInteractionCount = Math.Abs(postModel.VoteCount)
                    };
                }

                return commentModel;
            }).ToList(),
            InteractionStatistic = new List<JsonLdInteractionStatisticModel>
            {
                new()
                {
                    InteractionType = "https://schema.org/CommentAction", UserInteractionCount = Math.Max(forumTopic.NumPosts - 1, 0)
                },
                new()
                {
                    InteractionType = "https://schema.org/ViewAction", UserInteractionCount = forumTopic.Views
                }
            }
        };

        await _eventPublisher.PublishAsync(new JsonLdCreatedEvent<JsonLdForumTopicModel>(forumTopicModel));

        return forumTopicModel;
    }

    #endregion
}