using System.Collections.Generic;
using System.Linq;
using System.Net;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.ProductManufacturerMappings;
using Nop.Plugin.Api.JSON.ActionResults;
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
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc;
    using DTOs.Errors;
    using JSON.Serializers;

    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductManufacturerMappingsController : BaseApiController
    {
        private readonly IProductManufacturerMappingsApiService _productManufacturerMappingsService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IManufacturerApiService _manufacturerApiService;
        private readonly IProductApiService _productApiService;

        public ProductManufacturerMappingsController(IProductManufacturerMappingsApiService productManufacturerMappingsService,
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
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService,pictureService)
        {
            _productManufacturerMappingsService = productManufacturerMappingsService;
            _manufacturerService = manufacturerService;
            _manufacturerApiService = manufacturerApiService;
            _productApiService = productApiService;
        }

        /// <summary>
        /// Receive a list of all Product-Manufacturer mappings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_manufacturer_mappings")]
        [ProducesResponseType(typeof(ProductManufacturerMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappings(ProductManufacturerMappingsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            IList<ProductManufacturerMappingsDto> mappingsAsDtos =
                _productManufacturerMappingsService.GetMappings(parameters.ProductId,
                    parameters.ManufacturerId,
                    parameters.Limit,
                    parameters.Page,
                    parameters.SinceId).Select(x => x.ToDto()).ToList();

            var productManufacturerMappingRootObject = new ProductManufacturerMappingsRootObject()
            {
                ProductManufacturerMappingsDtos = mappingsAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(productManufacturerMappingRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Product-Manufacturer mappings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_manufacturer_mappings/count")]
        [ProducesResponseType(typeof(ProductManufacturerMappingsCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappingsCount(ProductManufacturerMappingsCountParametersModel parameters)
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

            var productManufacturerMappingsCountRootObject = new ProductManufacturerMappingsCountRootObject()
            {
                Count = mappingsCount
            };

            return Ok(productManufacturerMappingsCountRootObject);
        }

        /// <summary>
        /// Retrieve Product-Manufacturer mappings by spcified id
        /// </summary>
        ///   /// <param name="id">Id of the Product-Manufacturer mapping</param>
        /// <param name="fields">Fields from the Product-Manufacturer mapping you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_manufacturer_mappings/{id}")]
        [ProducesResponseType(typeof(ProductManufacturerMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappingById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var mapping = _productManufacturerMappingsService.GetById(id);

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
        [Route("/api/product_manufacturer_mappings")]
        [ProducesResponseType(typeof(ProductManufacturerMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult CreateProductManufacturerMapping([ModelBinder(typeof(JsonModelBinder<ProductManufacturerMappingsDto>))] Delta<ProductManufacturerMappingsDto> productManufacturerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var Manufacturer = _manufacturerApiService.GetManufacturerById(productManufacturerDelta.Dto.ManufacturerId.Value);
            if (Manufacturer == null)
            {
                return Error(HttpStatusCode.NotFound, "manufacturer_id", "not found");
            }

            var product = _productApiService.GetProductById(productManufacturerDelta.Dto.ProductId.Value);
            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product_id", "not found");
            }

            var mappingsCount = _productManufacturerMappingsService.GetMappingsCount(product.Id, Manufacturer.Id);

            if (mappingsCount > 0)
            {
                return Error(HttpStatusCode.BadRequest, "product_manufacturer_mapping", "already exist");
            }

            var newProductManufacturer = new ProductManufacturer();
            productManufacturerDelta.Merge(newProductManufacturer);

            //inserting new Manufacturer
            _manufacturerService.InsertProductManufacturer(newProductManufacturer);

            // Preparing the result dto of the new product Manufacturer mapping
            var newProductManufacturerMappingDto = newProductManufacturer.ToDto();

            var productManufacturerMappingsRootObject = new ProductManufacturerMappingsRootObject();

            productManufacturerMappingsRootObject.ProductManufacturerMappingsDtos.Add(newProductManufacturerMappingDto);

            var json = JsonFieldsSerializer.Serialize(productManufacturerMappingsRootObject, string.Empty);

            //activity log 
            CustomerActivityService.InsertActivity("AddNewProductManufacturerMapping", LocalizationService.GetResource("ActivityLog.AddNewProductManufacturerMapping"), newProductManufacturer);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/product_manufacturer_mappings/{id}")]
        [ProducesResponseType(typeof(ProductManufacturerMappingsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult UpdateProductManufacturerMapping([ModelBinder(typeof(JsonModelBinder<ProductManufacturerMappingsDto>))] Delta<ProductManufacturerMappingsDto> productManufacturerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            if (productManufacturerDelta.Dto.ManufacturerId.HasValue)
            {
                var Manufacturer = _manufacturerApiService.GetManufacturerById(productManufacturerDelta.Dto.ManufacturerId.Value);
                if (Manufacturer == null)
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

            var productManufacturerEntityToUpdate = _manufacturerService.GetProductManufacturerById(updateProductManufacturerId);

            if (productManufacturerEntityToUpdate == null)
            {
                return Error(HttpStatusCode.NotFound, "product_manufacturer_mapping", "not found");
            }

            productManufacturerDelta.Merge(productManufacturerEntityToUpdate);

            _manufacturerService.UpdateProductManufacturer(productManufacturerEntityToUpdate);

            //activity log
            CustomerActivityService.InsertActivity("UpdateProdutManufacturerMapping",
                LocalizationService.GetResource("ActivityLog.UpdateProdutManufacturerMapping"), productManufacturerEntityToUpdate);

            var updatedProductManufacturerDto = productManufacturerEntityToUpdate.ToDto();

            var productManufacturersRootObject = new ProductManufacturerMappingsRootObject();

            productManufacturersRootObject.ProductManufacturerMappingsDtos.Add(updatedProductManufacturerDto);

            var json = JsonFieldsSerializer.Serialize(productManufacturersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/product_manufacturer_mappings/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteProductManufacturerMapping(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productManufacturer = _manufacturerService.GetProductManufacturerById(id);

            if (productManufacturer == null)
            {
                return Error(HttpStatusCode.NotFound, "product_manufacturer_mapping", "not found");
            }

            _manufacturerService.DeleteProductManufacturer(productManufacturer);

            //activity log 
            CustomerActivityService.InsertActivity("DeleteProductManufacturerMapping", LocalizationService.GetResource("ActivityLog.DeleteProductManufacturerMapping"), productManufacturer);

            return new RawJsonActionResult("{}");
        }
    }
}