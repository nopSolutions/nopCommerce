using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.DTOs.SpecificationAttributes;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.SpecificationAttributes;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using System.Linq;
using System.Net;

namespace Nop.Plugin.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SpecificationAttributesController : BaseApiController
    {
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ISpecificationAttributeApiService _specificationAttributeApiService;
        private readonly IDTOHelper _dtoHelper;

        public SpecificationAttributesController(IJsonFieldsSerializer jsonFieldsSerializer, 
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
                                  IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _specificationAttributeService = specificationAttributeService;
            _specificationAttributeApiService = specificationAttributesApiService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        /// Receive a list of all specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/specificationattributes")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetSpecificationAttributes(SpecifcationAttributesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var specificationAttribtues = _specificationAttributeApiService.GetSpecificationAttributes(limit: parameters.Limit, page: parameters.Page, sinceId: parameters.SinceId);

            var specificationAttributeDtos = specificationAttribtues.Select(x => _dtoHelper.PrepareSpecificationAttributeDto(x)).ToList();

            var specificationAttributesRootObject = new SpecificationAttributesRootObjectDto()
            {
                SpecificationAttributes = specificationAttributeDtos
            };

            var json = JsonFieldsSerializer.Serialize(specificationAttributesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/specificationattributes/count")]
        [ProducesResponseType(typeof(SpecificationAttributesCountRootObject), (int)HttpStatusCode.OK)]        
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetSpecificationAttributesCount(SpecifcationAttributesCountParametersModel parameters)
        {
            var specificationAttributesCount = _specificationAttributeService.GetSpecificationAttributes().Count();

            var specificationAttributesCountRootObject = new SpecificationAttributesCountRootObject()
            {
                Count = specificationAttributesCount
            };

            return Ok(specificationAttributesCountRootObject);
        }

        /// <summary>
        /// Retrieve specification attribute by spcified id
        /// </summary>
        /// <param name="id">Id of the specification  attribute</param>
        /// <param name="fields">Fields from the specification attribute you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/specificationattributes/{id}")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetSpecificationAttributeById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);

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
        [Route("/api/specificationattributes")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult CreateSpecificationAttribute([ModelBinder(typeof(JsonModelBinder<SpecificationAttributeDto>))] Delta<SpecificationAttributeDto> specificaitonAttributeDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            var specificationAttribute = new SpecificationAttribute();
            specificaitonAttributeDelta.Merge(specificationAttribute);

            _specificationAttributeService.InsertSpecificationAttribute(specificationAttribute);

            CustomerActivityService.InsertActivity("AddNewSpecAttribute", LocalizationService.GetResource("ActivityLog.AddNewSpecAttribute"), specificationAttribute);

            // Preparing the result dto of the new product
            var specificationAttributeDto = _dtoHelper.PrepareSpecificationAttributeDto(specificationAttribute);

            var specificationAttributesRootObjectDto = new SpecificationAttributesRootObjectDto();
            specificationAttributesRootObjectDto.SpecificationAttributes.Add(specificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(specificationAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/specificationattributes/{id}")]
        [ProducesResponseType(typeof(SpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult UpdateSpecificationAttribute([ModelBinder(typeof(JsonModelBinder<SpecificationAttributeDto>))] Delta<SpecificationAttributeDto> specificationAttributeDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We do not need to validate the product attribute id, because this will happen in the model binder using the dto validator.
            int specificationAttributeId = specificationAttributeDelta.Dto.Id;

            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(specificationAttributeId);
            if (specificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "specification attribute", "not found");
            }

            specificationAttributeDelta.Merge(specificationAttribute);

            _specificationAttributeService.UpdateSpecificationAttribute(specificationAttribute);
          
            CustomerActivityService.InsertActivity("EditSpecAttribute", LocalizationService.GetResource("ActivityLog.EditSpecAttribute"), specificationAttribute);

            // Preparing the result dto of the new product attribute
            var specificationAttributeDto = _dtoHelper.PrepareSpecificationAttributeDto(specificationAttribute);

            var specificatoinAttributesRootObjectDto = new SpecificationAttributesRootObjectDto();
            specificatoinAttributesRootObjectDto.SpecificationAttributes.Add(specificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(specificatoinAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/specificationattributes/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult DeleteSpecificationAttribute(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var specificationAttribute = _specificationAttributeService.GetSpecificationAttributeById(id);
            if (specificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "specification attribute", "not found");
            }

            _specificationAttributeService.DeleteSpecificationAttribute(specificationAttribute);

            //activity log
            CustomerActivityService.InsertActivity("DeleteSpecAttribute", LocalizationService.GetResource("ActivityLog.DeleteSpecAttribute"), specificationAttribute);

            return new RawJsonActionResult("{}");
        }
    }
}