using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.DTOs.ProductAttributes;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductAttributes;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Nop.Plugin.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductAttributesController : BaseApiController
    {
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributesApiService _productAttributesApiService;
        private readonly IDTOHelper _dtoHelper;

        public ProductAttributesController(IJsonFieldsSerializer jsonFieldsSerializer, 
                                  ICustomerActivityService customerActivityService,
                                  ILocalizationService localizationService,
                                  IAclService aclService,
                                  IStoreMappingService storeMappingService,
                                  IStoreService storeService,
                                  ICustomerService customerService,
                                  IDiscountService discountService,
                                  IPictureService pictureService,
                                  IProductAttributeService productAttributeService,
                                  IProductAttributesApiService productAttributesApiService,
                                  IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _productAttributeService = productAttributeService;
            _productAttributesApiService = productAttributesApiService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        /// Receive a list of all product attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productattributes")]        
        [ProducesResponseType(typeof(ProductAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductAttributes(ProductAttributesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var allProductAttributes = _productAttributesApiService.GetProductAttributes(parameters.Limit, parameters.Page, parameters.SinceId);

            IList<ProductAttributeDto> productAttributesAsDtos = allProductAttributes.Select(productAttribute => _dtoHelper.PrepareProductAttributeDTO(productAttribute)).ToList();

            var productAttributesRootObject = new ProductAttributesRootObjectDto()
            {
                ProductAttributes = productAttributesAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(productAttributesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all product attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>        
        [HttpGet]
        [Route("/api/productattributes/count")]
        [ProducesResponseType(typeof(ProductAttributesCountRootObject), (int)HttpStatusCode.OK)]        
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductAttributesCount()
        {
            var allProductAttributesCount = _productAttributesApiService.GetProductAttributesCount();

            var productAttributesCountRootObject = new ProductAttributesCountRootObject()
            {
                Count = allProductAttributesCount
            };

            return Ok(productAttributesCountRootObject);
        }

        /// <summary>
        /// Retrieve product attribute by spcified id
        /// </summary>
        /// <param name="id">Id of the product attribute</param>
        /// <param name="fields">Fields from the product attribute you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productattributes/{id}")]
        [ProducesResponseType(typeof(ProductAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductAttributeById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productAttribute = _productAttributesApiService.GetById(id);

            if (productAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product attribute", "not found");
            }

            var productAttributeDto = _dtoHelper.PrepareProductAttributeDTO(productAttribute);

            var productAttributesRootObject = new ProductAttributesRootObjectDto();

            productAttributesRootObject.ProductAttributes.Add(productAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productAttributesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/productattributes")]
        [ProducesResponseType(typeof(ProductAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult CreateProductAttribute([ModelBinder(typeof(JsonModelBinder<ProductAttributeDto>))] Delta<ProductAttributeDto> productAttributeDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            var productAttribute = new ProductAttribute();
            productAttributeDelta.Merge(productAttribute);

            _productAttributeService.InsertProductAttribute(productAttribute);       

            CustomerActivityService.InsertActivity("AddNewProductAttribute",
                LocalizationService.GetResource("ActivityLog.AddNewProductAttribute"), productAttribute);

            // Preparing the result dto of the new product
            var productAttributeDto = _dtoHelper.PrepareProductAttributeDTO(productAttribute);

            var productAttributesRootObjectDto = new ProductAttributesRootObjectDto();

            productAttributesRootObjectDto.ProductAttributes.Add(productAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/productattributes/{id}")]
        [ProducesResponseType(typeof(ProductAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult UpdateProductAttribute([ModelBinder(typeof(JsonModelBinder<ProductAttributeDto>))] Delta<ProductAttributeDto> productAttributeDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var productAttribute = _productAttributesApiService.GetById(productAttributeDelta.Dto.Id);

            if (productAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product attribute", "not found");
            }

            productAttributeDelta.Merge(productAttribute);


            _productAttributeService.UpdateProductAttribute(productAttribute);
          
            CustomerActivityService.InsertActivity("EditProductAttribute",
               LocalizationService.GetResource("ActivityLog.EditProductAttribute"), productAttribute);

            // Preparing the result dto of the new product attribute
            var productAttributeDto = _dtoHelper.PrepareProductAttributeDTO(productAttribute);

            var productAttributesRootObjectDto = new ProductAttributesRootObjectDto();

            productAttributesRootObjectDto.ProductAttributes.Add(productAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/productattributes/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteProductAttribute(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productAttribute = _productAttributesApiService.GetById(id);

            if (productAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product attribute", "not found");
            }

            _productAttributeService.DeleteProductAttribute(productAttribute);

            //activity log
            CustomerActivityService.InsertActivity("DeleteProductAttribute", LocalizationService.GetResource("ActivityLog.DeleteProductAttribute"), productAttribute);

            return new RawJsonActionResult("{}");
        }       
    }
}