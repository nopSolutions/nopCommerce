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
using Nop.Plugin.Api.DTO.ProductWarehouseIventories;
using Nop.Plugin.Api.DTO.SpecificationAttributes;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductWarehouseInventoryParameters;
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
    public class ProductWarehouseInventoryController : BaseApiController
    {
        private readonly IProductApiService _productApiService;
        private readonly IProductService _productService;
        private readonly IWarehouseApiService _warehouseApiService;
        private readonly IProductWarehouseInventoriesApiService _productWarehouseInventoriesService;

        public ProductWarehouseInventoryController(
            IProductApiService productApiService,
            IProductService productService,
            IWarehouseApiService warehouseApiService,
            IProductWarehouseInventoriesApiService productWarehouseInventoriesService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService,
                discountService, customerActivityService, localizationService, pictureService)
        {
            _productApiService = productApiService;
            _productService = productService;
            _warehouseApiService = warehouseApiService;
            _productWarehouseInventoriesService = productWarehouseInventoriesService;
        }

        /// <summary>
        ///     Receive a list of all Product-Warehouse inventory
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_warehouse_inventories", Name = "GetProductCategoryInventories")]
        [ProducesResponseType(typeof(ProductWarehouseInventoryRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetInventories([FromQuery] ProductWarehouseInventoryParametersModel parameters)
        {
            if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Constants.Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            IList<ProductWarehouseInventoryDto> inventoryDtos =
                _productWarehouseInventoriesService.GetMappings(parameters.ProductId,
                    parameters.WarehouseId,
                    parameters.Limit,
                    parameters.Page,
                    parameters.SinceId).Select(x => x.ToDto()).ToList();

            var productWarehouseInventoryRootObject = new ProductWarehouseInventoryRootObject()
            {
                ProductWarehouseInventoryDtos = inventoryDtos
            };

            var json = JsonFieldsSerializer.Serialize(productWarehouseInventoryRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Retrieve Product-Warehouse inventories by specified id
        /// </summary>
        /// ///
        /// <param name="id">Id of the Product-Warehouse inventory</param>
        /// <param name="fields">Fields from the Product-Warehouse inventory you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_warehouse_inventories/{id}", Name = "GetProductWarehouseInventoriesById")]
        [ProducesResponseType(typeof(ProductWarehouseInventoryRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetInventoryById([FromRoute] int id, [FromQuery] string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var mapping = await _productWarehouseInventoriesService.GetByIdAsync(id);

            if (mapping == null)
            {
                return Error(HttpStatusCode.NotFound, "product_warehouse_inventories", "not found");
            }

            var productWarehouseInventoryRootObject = new ProductWarehouseInventoryRootObject();
            productWarehouseInventoryRootObject.ProductWarehouseInventoryDtos.Add(mapping.ToDto());

            var json = JsonFieldsSerializer.Serialize(productWarehouseInventoryRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/product_warehouse_inventories", Name = "CreateProductWarehouseInventory")]
        [ProducesResponseType(typeof(ProductWarehouseInventoryRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateProductWarehouseInventory(
            [FromBody]
            ProductWarehouseInventoryDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var productWarehouseInventoryDelta = new Delta<ProductWarehouseInventoryDto>(objectPropertyNameValuePairs);
            productWarehouseInventoryDelta.Dto = dto;

            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var warehouse = _warehouseApiService.GetWarehouseById(productWarehouseInventoryDelta.Dto.WarehouseId.Value);
            if (warehouse == null)
            {
                return Error(HttpStatusCode.NotFound, "warehouse_id", "not found");
            }

            var product = _productApiService.GetProductById(productWarehouseInventoryDelta.Dto.ProductId.Value);
            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product_id", "not found");
            }

            var ivnentoriesCount = _productWarehouseInventoriesService.GetIvnentoriesCount(product.Id, warehouse.Id);

            if (ivnentoriesCount > 0)
            {
                return Error(HttpStatusCode.BadRequest, "product_warehouse_inventories", "already exist");
            }

            var newProductWarehouseInventory = new ProductWarehouseInventory();
            productWarehouseInventoryDelta.Merge(newProductWarehouseInventory);

            //inserting new category
            await _productService.InsertProductWarehouseInventoryAsync(newProductWarehouseInventory);

            // Preparing the result dto of the new product category mapping
            var newProductWarehouseInventoryDto = newProductWarehouseInventory.ToDto();

            var productWarehouseInventoryRootObject = new ProductWarehouseInventoryRootObject();

            productWarehouseInventoryRootObject.ProductWarehouseInventoryDtos.Add(newProductWarehouseInventoryDto);

            var json = JsonFieldsSerializer.Serialize(productWarehouseInventoryRootObject, string.Empty);

            //activity log 
            await CustomerActivityService.InsertActivityAsync("AddNewProductWarehouseInventory", await LocalizationService.GetResourceAsync("ActivityLog.AddNewProductWarehouseInventory"),
                                                   newProductWarehouseInventory);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/product_warehouse_inventories/{id}", Name = "UpdateProductWarehouseInventory")]
        [ProducesResponseType(typeof(ProductWarehouseInventoryRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProductWarehouseInventory(
            [FromBody]
            ProductWarehouseInventoryDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var productWarehouseInventoryDelta = new Delta<ProductWarehouseInventoryDto>(objectPropertyNameValuePairs);
            productWarehouseInventoryDelta.Dto = dto;
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            if (productWarehouseInventoryDelta.Dto.WarehouseId.HasValue)
            {
                var warehouse = _warehouseApiService.GetWarehouseById(productWarehouseInventoryDelta.Dto.WarehouseId.Value);
                if (warehouse == null)
                {
                    return Error(HttpStatusCode.NotFound, "warehouse_id", "not found");
                }
            }

            if (productWarehouseInventoryDelta.Dto.ProductId.HasValue)
            {
                var product = _productApiService.GetProductById(productWarehouseInventoryDelta.Dto.ProductId.Value);
                if (product == null)
                {
                    return Error(HttpStatusCode.NotFound, "product_id", "not found");
                }
            }

            // We do not need to validate the category id, because this will happen in the model binder using the dto validator.
            var updateProductWarehouseInventoryId = productWarehouseInventoryDelta.Dto.Id;

            var productWarehouseInventoryEntityToUpdate =
                await _productWarehouseInventoriesService.GetByIdAsync(updateProductWarehouseInventoryId);

            if (productWarehouseInventoryEntityToUpdate == null)
            {
                return Error(HttpStatusCode.NotFound, "product_warehouse_inventory", "not found");
            }

            productWarehouseInventoryDelta.Merge(productWarehouseInventoryEntityToUpdate);

            await _productService.UpdateProductWarehouseInventoryAsync(productWarehouseInventoryEntityToUpdate);

            //activity log
            await CustomerActivityService.InsertActivityAsync("UpdateProductWarehouseInventory", await LocalizationService.GetResourceAsync("ActivityLog.UpdateProductWarehouseInventory"), productWarehouseInventoryEntityToUpdate);

            var updateProductWarehouseInventoryDto = productWarehouseInventoryEntityToUpdate.ToDto();

            var productWarehouseInventoryRootObject = new ProductWarehouseInventoryRootObject();

            productWarehouseInventoryRootObject.ProductWarehouseInventoryDtos.Add(updateProductWarehouseInventoryDto);

            var json = JsonFieldsSerializer.Serialize(productWarehouseInventoryRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/product_warehouse_inventories/{id}", Name = "DeleteProductWarehouseInventory")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteProductWarehouseInventory([FromRoute] int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productWarehouseInventory = await _productWarehouseInventoriesService.GetByIdAsync(id);

            if (productWarehouseInventory == null)
            {
                return Error(HttpStatusCode.NotFound, "product_warehouse_inventory", "not found");
            }

            await _productService.DeleteProductWarehouseInventoryAsync(productWarehouseInventory);

            //activity log 
            await CustomerActivityService.InsertActivityAsync("DeleteProductWarehouseInventory", await LocalizationService.GetResourceAsync("ActivityLog.DeleteProductWarehouseInventory"), productWarehouseInventory);

            return new RawJsonActionResult("{}");
        }
    }
}