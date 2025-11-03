using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Authorization.Attributes;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.NewsLetterSubscriptions;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.NewsLetterSubscriptionsParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [AuthorizePermission("ManageNewsletterSubscribers")]
    public class NewsLetterSubscriptionController : BaseApiController
    {
        private readonly INewsLetterSubscriptionApiService _newsLetterSubscriptionApiService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IStoreContext _storeContext;

        public NewsLetterSubscriptionController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            INewsLetterSubscriptionApiService newsLetterSubscriptionApiService,
            IStoreContext storeContext,
            INewsLetterSubscriptionService newsLetterSubscriptionService) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                                                                                 storeService, discountService, customerActivityService, localizationService,
                                                                                 pictureService)
        {
            _newsLetterSubscriptionApiService = newsLetterSubscriptionApiService;
            _storeContext = storeContext;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
        }

        /// <summary>
        ///     Receive a list of all NewsLetters
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/news_letter_subscriptions", Name = "GetNewsLetterSubscriptions")]
        [ProducesResponseType(typeof(NewsLetterSubscriptionsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetNewsLetterSubscriptions([FromQuery] NewsLetterSubscriptionsParametersModel parameters)
        {
            if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Constants.Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }

            var newsLetterSubscriptions = _newsLetterSubscriptionApiService.GetNewsLetterSubscriptions(parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                                                       parameters.Limit, parameters.Page, parameters.SinceId,
                                                                                                       parameters.OnlyActive);

            var newsLetterSubscriptionsDtos = newsLetterSubscriptions.Select(nls => nls.ToDto()).ToList();

            var newsLetterSubscriptionsRootObject = new NewsLetterSubscriptionsRootObject
            {
                NewsLetterSubscriptions = newsLetterSubscriptionsDtos
            };

            var json = JsonFieldsSerializer.Serialize(newsLetterSubscriptionsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Deactivate a NewsLetter subscriber by email
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/api/news_letter_subscriptions/{email}/deactivate", Name = "DeactivateNewsLetterSubscription")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeactivateNewsLetterSubscription([FromRoute] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Error(HttpStatusCode.BadRequest, "The email parameter could not be empty.");
            }

            var existingSubscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(email, _storeContext.GetCurrentStore().Id);

            if (existingSubscription == null)
            {
                return Error(HttpStatusCode.BadRequest, "There is no news letter subscription with the specified email.");
            }

            existingSubscription.Active = false;

            await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(existingSubscription);

            return Ok();
        }
    }
}
