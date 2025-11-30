using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Authorization.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.ShoppingCarts;
using Nop.Plugin.Api.DTO.SpecificationAttributes;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.SpecificationAttributesParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [AuthorizePermission("ManageAttributes")]
    public class SpecificationAttributesController : BaseApiController
    {
        private readonly IDTOHelper _dtoHelper;
        private readonly ISpecificationAttributeApiService _specificationAttributeApiService;
        private readonly ISpecificationAttributeService _specificationAttributeService;

        public SpecificationAttributesController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ICustomerService customerService,
            IDiscountService discountService,
            IPictureService pictureService,
            ISpecificationAttributeService specificationAttributeService,
            ISpecificationAttributeApiService specificationAttributesApiService,
            IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService,
                                         customerActivityService, localizationService, pictureService)
        {
            _specificationAttributeService = specificationAttributeService;
            _specificationAttributeApiService = specificationAttributesApiService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        ///     Receive a list of all specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/specificationattributes", Name = "GetSpecificationAttributes")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetSpecificationAttributes([FromQuery] SpecificationAttributesParametersModel parameters)
        {
            if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Constants.Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var specificationAttribtues = _specificationAttributeApiService.GetSpecificationAttributes(parameters.Limit, parameters.Page, parameters.SinceId);

            var specificationAttributeDtos = specificationAttribtues.Select(x => _dtoHelper.PrepareSpecificationAttributeDto(x)).ToList();

            var specificationAttributesRootObject = new SpecificationAttributesRootObjectDto
            {
                SpecificationAttributes = specificationAttributeDtos
            };

            var json = JsonFieldsSerializer.Serialize(specificationAttributesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/specificationattributes/count", Name = "GetSpecificationAttributesCount")]
        [ProducesResponseType(typeof(SpecificationAttributesCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetSpecificationAttributesCount([FromQuery] SpecificationAttributesCountParametersModel parameters)
        {
            var specificationAttributesCount = _specificationAttributeApiService
                .GetSpecificationAttributes(limit: int.MaxValue, page: 1, sinceId: Constants.Configurations.DefaultSinceId)
                .Count;

            var specificationAttributesCountRootObject = new SpecificationAttributesCountRootObject
            {
                Count = specificationAttributesCount
            };

            return Ok(specificationAttributesCountRootObject);
        }

        /// <summary>
        ///     Retrieve specification attribute by spcified id
        /// </summary>
        /// <param name="id">Id of the specification  attribute</param>
        /// <param name="fields">Fields from the specification attribute you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/specificationattributes/{id}", Name = "GetSpecificationAttributeById")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetSpecificationAttributeById([FromRoute] int id, [FromQuery] string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(id);

            if (specificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "specification attribute", "not found");
            }

            var specificationAttributeDto = _dtoHelper.PrepareSpecificationAttributeDto(specificationAttribute);

            var specificationAttributesRootObject = new SpecificationAttributesRootObjectDto();
            specificationAttributesRootObject.SpecificationAttributes.Add(specificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(specificationAttributesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/specificationattributes", Name = "CreateSpecificationAttribute")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateSpecificationAttribute(
            SpecificationAttributeDto dto)
        {
            
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var specificaitonAttributeDelta = new Delta<SpecificationAttributeDto>(objectPropertyNameValuePairs);
            specificaitonAttributeDelta.Dto = dto;


            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            var specificationAttribute = new SpecificationAttribute();
            specificaitonAttributeDelta.Merge(specificationAttribute);

            await _specificationAttributeService.InsertSpecificationAttributeAsync(specificationAttribute);

            await CustomerActivityService.InsertActivityAsync("AddNewSpecAttribute", await LocalizationService.GetResourceAsync("ActivityLog.AddNewSpecAttribute"), specificationAttribute);

            // Preparing the result dto of the new product
            var specificationAttributeDto = _dtoHelper.PrepareSpecificationAttributeDto(specificationAttribute);

            var specificationAttributesRootObjectDto = new SpecificationAttributesRootObjectDto();
            specificationAttributesRootObjectDto.SpecificationAttributes.Add(specificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(specificationAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/specificationattributes/{id}", Name = "UpdateSpecificationAttribute")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> UpdateSpecificationAttribute(
            [FromBody]
            SpecificationAttributeDto dto)
        {
                       
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var specificationAttributeDelta = new Delta<SpecificationAttributeDto>(objectPropertyNameValuePairs);
            specificationAttributeDelta.Dto = dto;
            
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We do not need to validate the product attribute id, because this will happen in the model binder using the dto validator.
            var specificationAttributeId = specificationAttributeDelta.Dto.Id;

            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(specificationAttributeId);
            if (specificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "specification attribute", "not found");
            }

            specificationAttributeDelta.Merge(specificationAttribute);

            await _specificationAttributeService.UpdateSpecificationAttributeAsync(specificationAttribute);

            await CustomerActivityService.InsertActivityAsync("EditSpecAttribute", await LocalizationService.GetResourceAsync("ActivityLog.EditSpecAttribute"), specificationAttribute);

            // Preparing the result dto of the new product attribute
            var specificationAttributeDto = _dtoHelper.PrepareSpecificationAttributeDto(specificationAttribute);

            var specificatoinAttributesRootObjectDto = new SpecificationAttributesRootObjectDto();
            specificatoinAttributesRootObjectDto.SpecificationAttributes.Add(specificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(specificatoinAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/specificationattributes/{id}", Name = "DeleteSpecificationAttribute")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> DeleteSpecificationAttribute([FromRoute] int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var specificationAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(id);
            if (specificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "specification attribute", "not found");
            }

            await _specificationAttributeService.DeleteSpecificationAttributeAsync(specificationAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteSpecAttribute", await LocalizationService.GetResourceAsync("ActivityLog.DeleteSpecAttribute"), specificationAttribute);

            return new RawJsonActionResult("{}");
        }
    }
}
