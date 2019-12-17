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
using Nop.Plugin.Api.Models.ProductSpecificationAttributes;
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
    public class ProductSpecificationAttributesController : BaseApiController
    {
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ISpecificationAttributeApiService _specificationAttributeApiService;
        private readonly IDTOHelper _dtoHelper;

        public ProductSpecificationAttributesController(IJsonFieldsSerializer jsonFieldsSerializer, 
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
        /// Receive a list of all product specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productspecificationattributes")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductSpecificationAttributes(ProductSpecifcationAttributesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var productSpecificationAttribtues = _specificationAttributeApiService.GetProductSpecificationAttributes(productId: parameters.ProductId, specificationAttributeOptionId: parameters.SpecificationAttributeOptionId, allowFiltering: parameters.AllowFiltering, showOnProductPage: parameters.ShowOnProductPage, limit: parameters.Limit, page: parameters.Page, sinceId: parameters.SinceId);

            var productSpecificationAttributeDtos = productSpecificationAttribtues.Select(x => _dtoHelper.PrepareProductSpecificationAttributeDto(x)).ToList();

            var productSpecificationAttributesRootObject = new ProductSpecificationAttributesRootObjectDto()
            {
                ProductSpecificationAttributes = productSpecificationAttributeDtos
            };

            var json = JsonFieldsSerializer.Serialize(productSpecificationAttributesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all product specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productspecificationattributes/count")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesCountRootObject), (int)HttpStatusCode.OK)]        
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductSpecificationAttributesCount(ProductSpecifcationAttributesCountParametersModel parameters)
        {
            var productSpecificationAttributesCount = _specificationAttributeService.GetProductSpecificationAttributeCount(productId: parameters.ProductId, specificationAttributeOptionId: parameters.SpecificationAttributeOptionId);

            var productSpecificationAttributesCountRootObject = new ProductSpecificationAttributesCountRootObject()
            {
                Count = productSpecificationAttributesCount
            };

            return Ok(productSpecificationAttributesCountRootObject);
        }

        /// <summary>
        /// Retrieve product specification attribute by spcified id
        /// </summary>
        /// <param name="id">Id of the product specification  attribute</param>
        /// <param name="fields">Fields from the product specification attribute you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productspecificationattributes/{id}")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductSpecificationAttributeById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productSpecificationAttribute = _specificationAttributeService.GetProductSpecificationAttributeById(id);

            if (productSpecificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product specification attribute", "not found");
            }

            var productSpecificationAttributeDto = _dtoHelper.PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

            var productSpecificationAttributesRootObject = new ProductSpecificationAttributesRootObjectDto();
            productSpecificationAttributesRootObject.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productSpecificationAttributesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/productspecificationattributes")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult CreateProductSpecificationAttribute([ModelBinder(typeof(JsonModelBinder<ProductSpecificationAttributeDto>))] Delta<ProductSpecificationAttributeDto> productSpecificaitonAttributeDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            var productSpecificationAttribute = new ProductSpecificationAttribute();
            productSpecificaitonAttributeDelta.Merge(productSpecificationAttribute);

            _specificationAttributeService.InsertProductSpecificationAttribute(productSpecificationAttribute);

            CustomerActivityService.InsertActivity("AddNewProductSpecificationAttribute", productSpecificationAttribute.Id.ToString());

            // Preparing the result dto of the new product
            var productSpecificationAttributeDto = _dtoHelper.PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

            var productSpecificationAttributesRootObjectDto = new ProductSpecificationAttributesRootObjectDto();
            productSpecificationAttributesRootObjectDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productSpecificationAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/productspecificationattributes/{id}")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult UpdateProductSpecificationAttribute([ModelBinder(typeof(JsonModelBinder<ProductSpecificationAttributeDto>))] Delta<ProductSpecificationAttributeDto> productSpecificationAttributeDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We do not need to validate the product attribute id, because this will happen in the model binder using the dto validator.
            int productSpecificationAttributeId = productSpecificationAttributeDelta.Dto.Id;

            var productSpecificationAttribute = _specificationAttributeService.GetProductSpecificationAttributeById(productSpecificationAttributeId);
            if (productSpecificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product specification attribute", "not found");
            }

            productSpecificationAttributeDelta.Merge(productSpecificationAttribute);

            _specificationAttributeService.UpdateProductSpecificationAttribute(productSpecificationAttribute);
          
            CustomerActivityService.InsertActivity("EditProductSpecificationAttribute", productSpecificationAttribute.Id.ToString());

            // Preparing the result dto of the new product attribute
            var productSpecificationAttributeDto = _dtoHelper.PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

            var productSpecificatoinAttributesRootObjectDto = new ProductSpecificationAttributesRootObjectDto();
            productSpecificatoinAttributesRootObjectDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productSpecificatoinAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/productspecificationattributes/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult DeleteProductSpecificationAttribute(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productSpecificationAttribute = _specificationAttributeService.GetProductSpecificationAttributeById(id);
            if (productSpecificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product specification attribute", "not found");
            }

            _specificationAttributeService.DeleteProductSpecificationAttribute(productSpecificationAttribute);

            //activity log
            CustomerActivityService.InsertActivity("DeleteProductSpecificationAttribute", LocalizationService.GetResource("ActivityLog.DeleteProductSpecificationAttribute"), productSpecificationAttribute);

            return new RawJsonActionResult("{}");
        }
    }
}