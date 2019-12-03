//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Nop.Plugin.Api.Attributes;
//using Nop.Plugin.Api.JSON.Serializers;
//using Nop.Services.Customers;
//using Nop.Services.Discounts;
//using Nop.Services.Localization;
//using Nop.Services.Logging;
//using Nop.Services.Media;
//using Nop.Services.Security;
//using Nop.Services.Stores;

//namespace Nop.Plugin.Api.Controllers
//{
//    using System.Net;
//    using Microsoft.AspNet.WebHooks;
//    using Microsoft.AspNetCore.Authentication.JwtBearer;
//    using Microsoft.AspNetCore.Authorization;
//    using Nop.Plugin.Api.Services;

//    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//    public class WebHookFiltersController : BaseApiController
//    {
//        private readonly IWebHookFilterManager _filterManager;

//        public WebHookFiltersController(IJsonFieldsSerializer jsonFieldsSerializer,
//            IAclService aclService,
//            ICustomerService customerService,
//            IStoreMappingService storeMappingService,
//            IStoreService storeService,
//            IDiscountService discountService,
//            ICustomerActivityService customerActivityService,
//            ILocalizationService localizationService,
//            IPictureService pictureService,
//            IWebHookService webHookService) :
//            base(jsonFieldsSerializer, 
//                aclService, 
//                customerService, 
//                storeMappingService, 
//                storeService, 
//                discountService, 
//                customerActivityService, 
//                localizationService,
//                pictureService)
//        {
//            _filterManager = webHookService.GetWebHookFilterManager();
//        }

//        [HttpGet]
//        [Route("/api/webhooks/filters")]
//        [ProducesResponseType(typeof(IEnumerable<WebHookFilter>), (int)HttpStatusCode.OK)]
//        [GetRequestsErrorInterceptorActionFilter]
//        public async Task<IEnumerable<WebHookFilter>> GetWebHookFilters()
//        {
//            IDictionary<string, WebHookFilter> filters = await _filterManager.GetAllWebHookFiltersAsync();
//            return filters.Values;
//        }
//    }
//}
