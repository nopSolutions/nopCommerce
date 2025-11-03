using System.Collections.Generic;
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
using Nop.Plugin.Api.DTO.Orders;
using Nop.Plugin.Api.DTO.ProductAttributes;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductAttributesParameters;
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
    [AuthorizePermission("ManageProducts")]
    [AuthorizePermission("ManageAttributes")]
    public class ProductAttributesController : BaseApiController
    {
        private readonly IDTOHelper _dtoHelper;
        private readonly IProductAttributesApiService _productAttributesApiService;
        private readonly IProductAttributeService _productAttributeService;

        public ProductAttributesController(
            IJsonFieldsSerializer jsonFieldsSerializer,
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
            IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService,
                                         customerActivityService, localizationService, pictureService)
        {
            _productAttributeService = productAttributeService;
            _productAttributesApiService = productAttributesApiService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        ///     Receive a list of all product attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productattributes", Name = "GetProductAttributes")]
        [ProducesResponseType(typeof(ProductAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductAttributes([FromQuery] ProductAttributesParametersModel parameters)
        {
            if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Constants.Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var allProductAttributes = _productAttributesApiService.GetProductAttributes(parameters.Limit, parameters.Page, parameters.SinceId);

            IList<ProductAttributeDto> productAttributesAsDtos =
                allProductAttributes.Select(productAttribute => _dtoHelper.PrepareProductAttributeDTO(productAttribute)).ToList();

            var productAttributesRootObject = new ProductAttributesRootObjectDto
            {
                ProductAttributes = productAttributesAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(productAttributesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all product attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productattributes/count", Name = "GetProductAttributesCount")]
        [ProducesResponseType(typeof(ProductAttributesCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductAttributesCount()
        {
            var allProductAttributesCount = _productAttributesApiService.GetProductAttributesCount();

            var productAttributesCountRootObject = new ProductAttributesCountRootObject
            {
                Count = allProductAttributesCount
            };

            return Ok(productAttributesCountRootObject);
        }

        /// <summary>
        ///     Retrieve product attribute by spcified id
        /// </summary>
        /// <param name="id">Id of the product attribute</param>
        /// <param name="fields">Fields from the product attribute you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productattributes/{id}", Name = "GetProductAttributeById")]
        [ProducesResponseType(typeof(ProductAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetProductAttributeById([FromRoute] int id, [FromQuery] string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productAttribute = await _productAttributesApiService.GetByIdAsync(id);

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
        [Route("/api/productattributes", Name = "CreateProductAttribute")]
        [ProducesResponseType(typeof(ProductAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> CreateProductAttribute(
            [FromBody]
            ProductAttributeDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var productAttributeDelta = new Delta<ProductAttributeDto>(objectPropertyNameValuePairs);
            productAttributeDelta.Dto = dto;
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            var productAttribute = new ProductAttribute();
            productAttributeDelta.Merge(productAttribute);

            await _productAttributeService.InsertProductAttributeAsync(productAttribute);

            await CustomerActivityService.InsertActivityAsync("AddNewProductAttribute", await LocalizationService.GetResourceAsync("ActivityLog.AddNewProductAttribute"), productAttribute);

            // Preparing the result dto of the new product
            var productAttributeDto = _dtoHelper.PrepareProductAttributeDTO(productAttribute);

            var productAttributesRootObjectDto = new ProductAttributesRootObjectDto();

            productAttributesRootObjectDto.ProductAttributes.Add(productAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/productattributes/{id}", Name = "UpdateProductAttribute")]
        [ProducesResponseType(typeof(ProductAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> UpdateProductAttribute(
            [FromBody]
            ProductAttributeDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var productAttributeDelta = new Delta<ProductAttributeDto>(objectPropertyNameValuePairs);
            productAttributeDelta.Dto = dto;
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var productAttribute = await _productAttributesApiService.GetByIdAsync(productAttributeDelta.Dto.Id);

            if (productAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product attribute", "not found");
            }

            productAttributeDelta.Merge(productAttribute);


            await _productAttributeService.UpdateProductAttributeAsync(productAttribute);

            await CustomerActivityService.InsertActivityAsync("EditProductAttribute", await LocalizationService.GetResourceAsync("ActivityLog.EditProductAttribute"), productAttribute);

            // Preparing the result dto of the new product attribute
            var productAttributeDto = _dtoHelper.PrepareProductAttributeDTO(productAttribute);

            var productAttributesRootObjectDto = new ProductAttributesRootObjectDto();

            productAttributesRootObjectDto.ProductAttributes.Add(productAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/productattributes/{id}", Name = "DeleteProductAttribute")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteProductAttribute([FromRoute] int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productAttribute = await _productAttributesApiService.GetByIdAsync(id);

            if (productAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product attribute", "not found");
            }

            await _productAttributeService.DeleteProductAttributeAsync(productAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteProductAttribute", await LocalizationService.GetResourceAsync("ActivityLog.DeleteProductAttribute"),
                                                   productAttribute);

            return new RawJsonActionResult("{}");
        }
    }
}
