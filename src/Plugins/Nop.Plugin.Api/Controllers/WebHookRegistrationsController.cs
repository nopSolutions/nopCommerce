//using Nop.Plugin.Api.Attributes;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Nop.Plugin.Api.JSON.Serializers;
//using Nop.Services.Customers;
//using Nop.Services.Discounts;
//using Nop.Services.Localization;
//using Nop.Services.Logging;
//using Nop.Services.Security;
//using Nop.Services.Stores;
//using Nop.Core.Domain.Stores;
//using System.Net.Http;
//using System.Net;
//using System.Globalization;
//using Nop.Core;
//using Nop.Plugin.Api.Constants;
//using Nop.Services.Media;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNet.WebHooks;
//using Nop.Plugin.Api.Services;

//namespace Nop.Plugin.Api.Controllers
//{
//    using System.Security;
//    using System.Security.Claims;
//    using IdentityServer4.EntityFramework.Entities;
//    using IdentityServer4.Stores;
//    using Microsoft.AspNetCore.Authentication.JwtBearer;
//    using Microsoft.AspNetCore.Authorization;
//    using Microsoft.AspNetCore.Http;
//    using Nop.Plugin.Api.JSON.Serializers;

//    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//    public class WebHookRegistrationsController : BaseApiController
//    {
//        private const string ErrorPropertyKey = "webhook";
//        private const string PRIVATE_FILTER_PREFIX = "MS_Private_";

//        private readonly IWebHookManager _manager;
//        private readonly IWebHookStore _store;
//        private readonly IWebHookFilterManager _filterManager;
//        private readonly IStoreContext _storeContext;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly IClientStore _clientStore;

//        public WebHookRegistrationsController(IJsonFieldsSerializer jsonFieldsSerializer,
//            IAclService aclService,
//            ICustomerService customerService,
//            IStoreMappingService storeMappingService,
//            IStoreService storeService,
//            IDiscountService discountService,
//            ICustomerActivityService customerActivityService,
//            ILocalizationService localizationService,
//            IPictureService pictureService,
//            IStoreContext storeContext,
//            IWebHookService webHookService,
//            IHttpContextAccessor httpContextAccessor,
//            IClientStore clientStore)
//            : base(jsonFieldsSerializer,
//                  aclService, customerService,
//                  storeMappingService,
//                  storeService,
//                  discountService,
//                  customerActivityService,
//                  localizationService,
//                  pictureService)
//        {
//            _storeContext = storeContext;
//            _manager = webHookService.GetWebHookManager();
//            _store = webHookService.GetWebHookStore();
//            _filterManager = webHookService.GetWebHookFilterManager();
//            _httpContextAccessor = httpContextAccessor;
//            _clientStore = clientStore;
//        }

//        /// <summary>
//        /// Gets all registered WebHooks for a given user.
//        /// </summary>
//        /// <returns>A collection containing the registered <see cref="WebHook"/> instances for a given user.</returns>
//        [HttpGet]
//        [Route("/api/webhooks/registrations")]
//        [ProducesResponseType(typeof(IEnumerable<WebHook>), (int)HttpStatusCode.OK)]
//        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
//        [GetRequestsErrorInterceptorActionFilter]
//        public async Task<IEnumerable<WebHook>> GetAllWebHooks()
//        {
//            string userId = GetUserId();
//            IEnumerable<WebHook> webHooks = await _store.GetAllWebHooksAsync(userId);
//            RemovePrivateFilters(webHooks);
//            return webHooks;
//        }

//        /// <summary>
//        /// Looks up a registered WebHook with the given <paramref name="id"/> for a given user.
//        /// </summary>
//        /// <returns>The registered <see cref="WebHook"/> instance for a given user.</returns>
//        [HttpGet]
//        [Route("/api/webhooks/registrations/{id}",Name = WebHookNames.GetWebhookByIdAction)]
//        [ProducesResponseType(typeof(WebHook), (int)HttpStatusCode.OK)]
//        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
//        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
//        [GetRequestsErrorInterceptorActionFilter]
//        public async Task<IActionResult> GetWebHookById(string id)
//        {
//            string userId = GetUserId();
//            WebHook webHook = await _store.LookupWebHookAsync(userId, id);
//            if (webHook != null)
//            {
//                RemovePrivateFilters(new[] { webHook });
//                return Ok(webHook);
//            }

//            return NotFound();
//        }

//        /// <summary>
//        /// Registers a new WebHook for a given user.
//        /// </summary>
//        /// <param name="webHook">The <see cref="WebHook"/> to create.</param>
//        [HttpPost]
//        [Route("/api/webhooks/registrations")]
//        [ProducesResponseType(typeof(StoreResult), (int)HttpStatusCode.OK)]
//        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
//        [ProducesResponseType(typeof(HttpResponseMessage), (int)HttpStatusCode.InternalServerError)]
//        [ProducesResponseType(typeof(HttpResponseMessage), (int)HttpStatusCode.Conflict)]
//        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
//        public async Task<IActionResult> RegisterWebHook([FromBody]WebHook webHook)
//        {
//            if (!ModelState.IsValid)
//            {
//                return Error();
//            }

//            if (webHook == null)
//            {
//                return BadRequest();
//            }

//            string userId = GetUserId();

//            try
//            {
//                await VerifyFilters(webHook);
//                await VerifyWebHook(webHook);
//            }
//            catch (VerificationException ex)
//            {
//                return BadRequest(ex.Message);
//            }

//            // In order to ensure that a web hook filter is not registered multiple times for the same uri
//            // we remove the already registered filters from the current web hook.
//            // If the same filters are registered multiple times with the same uri, the web hook event will be
//            // sent for each registration.
//            IEnumerable<WebHook> existingWebhooks = await GetAllWebHooks();
//            IEnumerable<WebHook> existingWebhooksForTheSameUri = existingWebhooks.Where(wh => wh.WebHookUri == webHook.WebHookUri);

//            foreach (var existingWebHook in existingWebhooksForTheSameUri)
//            {
//                webHook.Filters.ExceptWith(existingWebHook.Filters);

//                if (!webHook.Filters.Any())
//                {
//                    string msg = _localizationService.GetResource("Api.WebHooks.CouldNotRegisterDuplicateWebhook");
//                    return Error(HttpStatusCode.Conflict, ErrorPropertyKey, msg);
//                }
//            }

//            try
//            {
//                // Validate the provided WebHook ID (or force one to be created on server side)
//                if (Request == null)
//                {
//                    throw new ArgumentNullException(nameof(Request));
//                }

//                // Ensure we have a normalized ID for the WebHook
//                webHook.Id = null;

//                // Add WebHook for this user.
//                StoreResult result = await _store.InsertWebHookAsync(userId, webHook);

//                if (result == StoreResult.Success)
//                {
//                    return CreatedAtRoute(WebHookNames.GetWebhookByIdAction, new { id = webHook.Id }, webHook);
//                }
//                return CreateHttpResult(result);
//            }
//            catch (Exception ex)
//            {
//                string msg = string.Format(CultureInfo.InvariantCulture, _localizationService.GetResource("Api.WebHooks.CouldNotRegisterWebhook"), ex.Message);
//                //Configuration.DependencyResolver.GetLogger().Error(msg, ex);
//                return Error(HttpStatusCode.Conflict, ErrorPropertyKey, msg);
//            }
//        }

//        /// <summary>
//        /// Updates an existing WebHook registration.
//        /// </summary>
//        /// <param name="id">The WebHook ID.</param>
//        /// <param name="webHook">The new <see cref="WebHook"/> to use.</param>
//        [HttpPut]
//        [Route("/api/webhooks/registrations/{id}")]
//        [ProducesResponseType(typeof(StoreResult), (int)HttpStatusCode.OK)]
//        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
//        [ProducesResponseType(typeof(HttpResponseMessage), (int)HttpStatusCode.InternalServerError)]
//        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
//        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
//        public async Task<IActionResult> UpdateWebHook(string id, WebHook webHook)
//        {
//            if (webHook == null)
//            {
//                return BadRequest();
//            }
//            if (!string.Equals(id, webHook.Id, StringComparison.OrdinalIgnoreCase))
//            {
//                return BadRequest();
//            }

//            string userId = GetUserId();
//            await VerifyFilters(webHook);
//            await VerifyWebHook(webHook);

//            try
//            {
//                StoreResult result = await _store.UpdateWebHookAsync(userId, webHook);
//                return CreateHttpResult(result);
//            }
//            catch (Exception ex)
//            {
//                string msg = string.Format(CultureInfo.InvariantCulture, _localizationService.GetResource("Api.WebHooks.CouldNotUpdateWebhook"), ex.Message);
//               // Configuration.DependencyResolver.GetLogger().Error(msg, ex);
//                return Error(HttpStatusCode.InternalServerError, ErrorPropertyKey, msg);
//            }
//        }

//        /// <summary>
//        /// Deletes an existing WebHook registration.
//        /// </summary>
//        /// <param name="id">The WebHook ID.</param>
//        [HttpDelete]
//        [Route("/api/webhooks/registrations/{id}")]
//        [ProducesResponseType(typeof(StoreResult), (int)HttpStatusCode.OK)]
//        [ProducesResponseType(typeof(HttpResponseMessage), (int)HttpStatusCode.InternalServerError)]
//        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
//        public async Task<IActionResult> DeleteWebHook(string id)
//        {
//            string userId = GetUserId();

//            try
//            {
//                StoreResult result = await _store.DeleteWebHookAsync(userId, id);
//                return CreateHttpResult(result);
//            }
//            catch (Exception ex)
//            {
//                string msg = string.Format(CultureInfo.InvariantCulture, _localizationService.GetResource("Api.WebHooks.CouldNotDeleteWebhook"), ex.Message);
//                //Configuration.DependencyResolver.GetLogger().Error(msg, ex);
//                return Error(HttpStatusCode.InternalServerError, ErrorPropertyKey, msg);
//            }
//        }

//        /// <summary>
//        /// Deletes all existing WebHook registrations.
//        /// </summary>
//        [HttpDelete]
//        [Route("/api/webhooks/registrations")]
//        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
//        [ProducesResponseType(typeof(HttpResponseMessage), (int)HttpStatusCode.InternalServerError)]
//        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
//        public async Task<IActionResult> DeleteAllWebHooks()
//        {
//            string userId = GetUserId();

//            try
//            {
//                await _store.DeleteAllWebHooksAsync(userId);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                string msg = string.Format(CultureInfo.InvariantCulture, _localizationService.GetResource("Api.WebHooks.CouldNotDeleteWebhooks"), ex.Message);
//               // Configuration.DependencyResolver.GetLogger().Error(msg, ex);
//                return Error(HttpStatusCode.InternalServerError, ErrorPropertyKey, msg);
//            }
//        }

//        /// <summary>
//        /// Ensure that the provided <paramref name="webHook"/> only has registered filters.
//        /// </summary>
//        protected virtual async Task VerifyFilters(WebHook webHook)
//        {
//            if (webHook == null)
//            {
//                throw new ArgumentNullException(nameof(webHook));
//            }

//            // If there are no filters then add our wildcard filter.
//            if (webHook.Filters.Count == 0)
//            {
//                webHook.Filters.Add(WildcardWebHookFilterProvider.Name);
//                return;
//            }
            
//            IDictionary<string, WebHookFilter> filters = await _filterManager.GetAllWebHookFiltersAsync();
//            HashSet<string> normalizedFilters = new HashSet<string>();
//            List<string> invalidFilters = new List<string>();
//            foreach (string filter in webHook.Filters)
//            {
//                WebHookFilter hookFilter;
//                if (filters.TryGetValue(filter, out hookFilter))
//                {
//                    normalizedFilters.Add(hookFilter.Name);
//                }
//                else
//                {
//                    invalidFilters.Add(filter);
//                }
//            }

//            if (invalidFilters.Count > 0)
//            {
//                string invalidFiltersMsg = string.Join(", ", invalidFilters);
//                string link = Url.Link(WebHookNames.FiltersGetAction, null);
//                string msg = string.Format(CultureInfo.CurrentCulture, _localizationService.GetResource("Api.WebHooks.InvalidFilters"), invalidFiltersMsg, link);
//                //Configuration.DependencyResolver.GetLogger().Info(msg);
                
//                throw new VerificationException(msg);
//            }
//            else
//            {
//                webHook.Filters.Clear();
//                foreach (string filter in normalizedFilters)
//                {
//                    webHook.Filters.Add(filter);
//                }
//            }
//        }

//        /// <summary>
//        /// Removes all private filters from registered WebHooks.
//        /// </summary>
//        protected virtual void RemovePrivateFilters(IEnumerable<WebHook> webHooks)
//        {
//            if (webHooks == null)
//            {
//                throw new ArgumentNullException(nameof(webHooks));
//            }

//            foreach (WebHook webHook in webHooks)
//            {
//                var filters = webHook.Filters.Where(f => f.StartsWith(PRIVATE_FILTER_PREFIX, StringComparison.OrdinalIgnoreCase)).ToArray();
//                foreach (string filter in filters)
//                {
//                    webHook.Filters.Remove(filter);
//                }
//            }
//        }

//        /// <summary>
//        /// Ensures that the provided <paramref name="webHook"/> has a reachable Web Hook URI unless
//        /// the WebHook URI has a <c>NoEcho</c> query parameter.
//        /// </summary>
//        private async Task VerifyWebHook(WebHook webHook)
//        {
//            if (webHook == null)
//            {
//                throw new ArgumentNullException(nameof(webHook));
//            }

//            // If no secret is provided then we create one here. This allows for scenarios
//            // where the caller may use a secret directly embedded in the WebHook URI, or
//            // has some other way of enforcing security.
//            if (string.IsNullOrEmpty(webHook.Secret))
//            {
//                webHook.Secret = Guid.NewGuid().ToString("N");
//            }

//            try
//            {
//                await _manager.VerifyWebHookAsync(webHook);
//            }
//            catch (Exception ex)
//            {
//                throw new VerificationException(ex.Message);
//            }
//        }

//        /// <summary>
//        /// Gets the user ID for this request.
//        /// </summary>
//        private string GetUserId()
//        {          
//            // If we are here the client is already authorized.
//            // So there is a client ID and the client is active.
//            var clientId =
//                _httpContextAccessor.HttpContext.User.FindFirst("client_id")?.Value;

//            var storeId = _storeContext.CurrentStore.Id;

//            var webHookUser = clientId + "-" + storeId;

//            return webHookUser;
//        }

//        /// <summary>
//        /// Creates an <see cref="IActionResult"/> based on the provided <paramref name="result"/>.
//        /// </summary>
//        /// <param name="result">The result to use when creating the <see cref="IActionResult"/>.</param>
//        /// <returns>An initialized <see cref="IActionResult"/>.</returns>
//        private IActionResult CreateHttpResult(StoreResult result)
//        {
//            switch (result)
//            {
//                case StoreResult.Success:
//                    return Ok();

//                case StoreResult.Conflict:
//                    return Error(HttpStatusCode.Conflict);

//                case StoreResult.NotFound:
//                    return NotFound();

//                case StoreResult.OperationError:
//                    return BadRequest();

//                default:
//                    return Error(HttpStatusCode.InternalServerError);
//            }
//        }
//    }
//}
