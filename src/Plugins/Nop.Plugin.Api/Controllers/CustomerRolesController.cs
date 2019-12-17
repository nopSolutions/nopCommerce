using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.CustomerRoles;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    using System.Net;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc;
    using DTOs.Errors;
    using JSON.Serializers;

    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomerRolesController : BaseApiController
    {
        public CustomerRolesController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService, 
            ICustomerService customerService, 
            IStoreMappingService storeMappingService, 
            IStoreService storeService, 
            IDiscountService discountService,
            ICustomerActivityService customerActivityService, 
            ILocalizationService localizationService,
            IPictureService pictureService) 
            : base(jsonFieldsSerializer, 
                  aclService, 
                  customerService, 
                  storeMappingService, 
                  storeService, 
                  discountService,
                  customerActivityService,
                  localizationService, 
                  pictureService)
        {
        }

        /// <summary>
        /// Retrieve all customer roles
        /// </summary>
        /// <param name="fields">Fields from the customer role you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/customer_roles")]
        [ProducesResponseType(typeof(CustomerRolesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetAllCustomerRoles(string fields = "")
        {
            var allCustomerRoles = CustomerService.GetAllCustomerRoles();

            IList<CustomerRoleDto> customerRolesAsDto = allCustomerRoles.Select(role => role.ToDto()).ToList();

            var customerRolesRootObject = new CustomerRolesRootObject()
            {
                CustomerRoles = customerRolesAsDto
            };

            var json = JsonFieldsSerializer.Serialize(customerRolesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}
