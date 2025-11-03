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
using Nop.Plugin.Api.DTO.ProductCategoryMappings;
using Nop.Plugin.Api.DTO.ProductManufacturerMappings;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductManufacturerMappingsParameters;
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
    [AuthorizePermission("ManageManufacturers")]
    public class ProductManufacturerMappingsController : BaseApiController
    {
        private readonly IManufacturerApiService _manufacturerApiService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductApiService _productApiService;
        private readonly IProductManufacturerMappingsApiService _productManufacturerMappingsService;

        public ProductManufacturerMappingsController(
            IProductManufacturerMappingsApiService productManufacturerMappingsService,
            IManufacturerService manufacturerService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IManufacturerApiService manufacturerApiService,
            IProductApiService productApiService,
            IPictureService pictureService)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService,
                   localizationService, pictureService)
        {
            _productManufacturerMappingsService = productManufacturerMappingsService;
            _manufacturerService = manufacturerService;
            _manufacturerApiService = manufacturerApiService;
            _productApiService = productApiService;
        }

        /// <summary>
        ///     Receive a list of all Product-Manufacturer mappings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_manufacturer_mappings", Name = "GetProductManufacturerMappings")]
        [ProducesResponseType(typeof(ProductManufacturerMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappings([FromQuery] ProductManufacturerMappingsParametersModel parameters)
        {
            if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Constants.Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            IList<ProductManufacturerMappingsDto> mappingsAsDtos =
                _productManufacturerMappingsService.GetMappings(parameters.ProductId,
                                                                parameters.ManufacturerId,
                                                                parameters.Limit,
                                                                parameters.Page,
                                                                parameters.SinceId).Select(x => x.ToDto()).ToList();

            var productManufacturerMappingRootObject = new ProductManufacturerMappingsRootObject
            {
                ProductManufacturerMappingsDtos = mappingsAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(productManufacturerMappingRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all Product-Manufacturer mappings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_manufacturer_mappings/count", Name = "GetProductManufacturerMappingsCount")]
        [ProducesResponseType(typeof(ProductManufacturerMappingsCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappingsCount([FromQuery] ProductManufacturerMappingsCountParametersModel parameters)
        {
            if (parameters.ProductId < 0)
            {
                return Error(HttpStatusCode.BadRequest, "product_id", "invalid product_id");
            }

            if (parameters.ManufacturerId < 0)
            {
                return Error(HttpStatusCode.BadRequest, "manufacturer_id", "invalid manufacturer_id");
            }

            var mappingsCount = _productManufacturerMappingsService.GetMappingsCount(parameters.ProductId,
                                                                                     parameters.ManufacturerId);

            var productManufacturerMappingsCountRootObject = new ProductManufacturerMappingsCountRootObject
            {
                Count = mappingsCount
            };

            return Ok(productManufacturerMappingsCountRootObject);
        }

        /// <summary>
        ///     Retrieve Product-Manufacturer mappings by spcified id
        /// </summary>
        /// ///
        /// <param name="id">Id of the Product-Manufacturer mapping</param>
        /// <param name="fields">Fields from the Product-Manufacturer mapping you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_manufacturer_mappings/{id}", Name = "GetProductManufacturerMappingById")]
        [ProducesResponseType(typeof(ProductManufacturerMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetMappingById([FromRoute] int id, [FromQuery] string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var mapping = await _productManufacturerMappingsService.GetByIdAsync(id);

            if (mapping == null)
            {
                return Error(HttpStatusCode.NotFound, "product_manufacturer_mapping", "not found");
            }

            var productManufacturerMappingsRootObject = new ProductManufacturerMappingsRootObject();
            productManufacturerMappingsRootObject.ProductManufacturerMappingsDtos.Add(mapping.ToDto());

            var json = JsonFieldsSerializer.Serialize(productManufacturerMappingsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/product_manufacturer_mappings", Name = "CreateProductManufacturerMapping")]
        [ProducesResponseType(typeof(ProductManufacturerMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateProductManufacturerMapping(
            [FromBody]
            ProductManufacturerMappingsDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var productManufacturerDelta = new Delta<ProductManufacturerMappingsDto>(objectPropertyNameValuePairs);
            productManufacturerDelta.Dto = dto;
            
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var manufacturer = _manufacturerApiService.GetManufacturerById(productManufacturerDelta.Dto.ManufacturerId.Value);
            if (manufacturer == null)
            {
                return Error(HttpStatusCode.NotFound, "manufacturer_id", "not found");
            }

            var product = _productApiService.GetProductById(productManufacturerDelta.Dto.ProductId.Value);
            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product_id", "not found");
            }

            var mappingsCount = _productManufacturerMappingsService.GetMappingsCount(product.Id, manufacturer.Id);

            if (mappingsCount > 0)
            {
                return Error(HttpStatusCode.BadRequest, "product_manufacturer_mapping", "already exist");
            }

            var newProductManufacturer = new ProductManufacturer();
            productManufacturerDelta.Merge(newProductManufacturer);

            //inserting new Manufacturer
            await _manufacturerService.InsertProductManufacturerAsync(newProductManufacturer);

            // Preparing the result dto of the new product Manufacturer mapping
            var newProductManufacturerMappingDto = newProductManufacturer.ToDto();

            var productManufacturerMappingsRootObject = new ProductManufacturerMappingsRootObject();

            productManufacturerMappingsRootObject.ProductManufacturerMappingsDtos.Add(newProductManufacturerMappingDto);

            var json = JsonFieldsSerializer.Serialize(productManufacturerMappingsRootObject, string.Empty);

            //activity log 
            await CustomerActivityService.InsertActivityAsync("AddNewProductManufacturerMapping", await LocalizationService.GetResourceAsync("ActivityLog.AddNewProductManufacturerMapping"), newProductManufacturer);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/product_manufacturer_mappings/{id}", Name = "UpdateProductManufacturerMapping")]
        [ProducesResponseType(typeof(ProductManufacturerMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProductManufacturerMapping(
            [FromBody]
            ProductManufacturerMappingsDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var productManufacturerDelta = new Delta<ProductManufacturerMappingsDto>(objectPropertyNameValuePairs);
            productManufacturerDelta.Dto = dto;

            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            if (productManufacturerDelta.Dto.ManufacturerId.HasValue)
            {
                var manufacturer = _manufacturerApiService.GetManufacturerById(productManufacturerDelta.Dto.ManufacturerId.Value);
                if (manufacturer == null)
                {
                    return Error(HttpStatusCode.NotFound, "manufacturer_id", "not found");
                }
            }

            if (productManufacturerDelta.Dto.ProductId.HasValue)
            {
                var product = _productApiService.GetProductById(productManufacturerDelta.Dto.ProductId.Value);
                if (product == null)
                {
                    return Error(HttpStatusCode.NotFound, "product_id", "not found");
                }
            }

            // We do not need to validate the Manufacturer id, because this will happen in the model binder using the dto validator.
            var updateProductManufacturerId = productManufacturerDelta.Dto.Id;

            var productManufacturerEntityToUpdate = await _manufacturerService.GetProductManufacturerByIdAsync(updateProductManufacturerId);

            if (productManufacturerEntityToUpdate == null)
            {
                return Error(HttpStatusCode.NotFound, "product_manufacturer_mapping", "not found");
            }

            productManufacturerDelta.Merge(productManufacturerEntityToUpdate);

            await _manufacturerService.UpdateProductManufacturerAsync(productManufacturerEntityToUpdate);

            //activity log
            await CustomerActivityService.InsertActivityAsync("UpdateProdutManufacturerMapping", await LocalizationService.GetResourceAsync("ActivityLog.UpdateProdutManufacturerMapping"), productManufacturerEntityToUpdate);

            var updatedProductManufacturerDto = productManufacturerEntityToUpdate.ToDto();

            var productManufacturersRootObject = new ProductManufacturerMappingsRootObject();

            productManufacturersRootObject.ProductManufacturerMappingsDtos.Add(updatedProductManufacturerDto);

            var json = JsonFieldsSerializer.Serialize(productManufacturersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/product_manufacturer_mappings/{id}", Name = "DeleteProductManufacturerMapping")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteProductManufacturerMapping([FromRoute] int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productManufacturer = await _manufacturerService.GetProductManufacturerByIdAsync(id);

            if (productManufacturer == null)
            {
                return Error(HttpStatusCode.NotFound, "product_manufacturer_mapping", "not found");
            }

            await _manufacturerService.DeleteProductManufacturerAsync(productManufacturer);

            //activity log 
            await CustomerActivityService.InsertActivityAsync("DeleteProductManufacturerMapping", await LocalizationService.GetResourceAsync("ActivityLog.DeleteProductManufacturerMapping"), productManufacturer);

            return new RawJsonActionResult("{}");
        }
    }
}
