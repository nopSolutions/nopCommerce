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
using Nop.Plugin.Api.DTO.Products;
using Nop.Plugin.Api.DTO.SpecificationAttributes;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductSpecificationAttributesParameters;
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
    public class ProductSpecificationAttributesController : BaseApiController
    {
        private readonly IDTOHelper _dtoHelper;
        private readonly ISpecificationAttributeApiService _specificationAttributeApiService;
        private readonly ISpecificationAttributeService _specificationAttributeService;

        public ProductSpecificationAttributesController(
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
        ///     Receive a list of all product specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productspecificationattributes", Name = "GetProductSpecificationAttributes")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductSpecificationAttributes([FromQuery] ProductSpecificationAttributesParametersModel parameters)
        {
            if (parameters.Limit < Constants.Configurations.MinLimit || parameters.Limit > Constants.Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Constants.Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var productSpecificationAttribtues =
                _specificationAttributeApiService.GetProductSpecificationAttributes(parameters.ProductId, parameters.SpecificationAttributeOptionId,
                                                                                    parameters.AllowFiltering, parameters.ShowOnProductPage, parameters.Limit,
                                                                                    parameters.Page, parameters.SinceId);

            var productSpecificationAttributeDtos = productSpecificationAttribtues.Select(x => _dtoHelper.PrepareProductSpecificationAttributeDto(x)).ToList();

            var productSpecificationAttributesRootObject = new ProductSpecificationAttributesRootObjectDto
            {
                ProductSpecificationAttributes = productSpecificationAttributeDtos
            };

            var json = JsonFieldsSerializer.Serialize(productSpecificationAttributesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all product specification attributes
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productspecificationattributes/count", Name = "GetProductSpecificationAttributesCount")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetProductSpecificationAttributesCount([FromQuery] ProductSpecificationAttributesCountParametersModel parameters)
        {
            var productSpecificationAttributesCount = await _specificationAttributeService.GetProductSpecificationAttributeCountAsync(parameters.ProductId, parameters.SpecificationAttributeOptionId);

            var productSpecificationAttributesCountRootObject = new ProductSpecificationAttributesCountRootObject
            {
                Count = productSpecificationAttributesCount
            };

            return Ok(productSpecificationAttributesCountRootObject);
        }

        /// <summary>
        ///     Retrieve product specification attribute by spcified id
        /// </summary>
        /// <param name="id">Id of the product specification  attribute</param>
        /// <param name="fields">Fields from the product specification attribute you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/productspecificationattributes/{id}", Name = "GetProductSpecificationAttributeById")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetProductSpecificationAttributeById([FromRoute] int id, [FromQuery] string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productSpecificationAttribute = await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(id);

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
        [Route("/api/productspecificationattributes", Name = "CreateProductSpecificationAttribute")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateProductSpecificationAttribute(
            [FromBody]
            ProductSpecificationAttributeDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var productSpecificaitonAttributeDelta = new Delta<ProductSpecificationAttributeDto>(objectPropertyNameValuePairs);
            productSpecificaitonAttributeDelta.Dto = dto;

            
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new product
            var productSpecificationAttribute = new ProductSpecificationAttribute();
            productSpecificaitonAttributeDelta.Merge(productSpecificationAttribute);

            await _specificationAttributeService.InsertProductSpecificationAttributeAsync(productSpecificationAttribute);

            await CustomerActivityService.InsertActivityAsync("AddNewProductSpecificationAttribute", productSpecificationAttribute.Id.ToString());

            // Preparing the result dto of the new product
            var productSpecificationAttributeDto = _dtoHelper.PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

            var productSpecificationAttributesRootObjectDto = new ProductSpecificationAttributesRootObjectDto();
            productSpecificationAttributesRootObjectDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productSpecificationAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/productspecificationattributes/{id}", Name = "UpdateProductSpecificationAttribute")]
        [ProducesResponseType(typeof(ProductSpecificationAttributesRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> UpdateProductSpecificationAttribute(
            [FromBody]
            ProductSpecificationAttributeDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var productSpecificationAttributeDelta = new Delta<ProductSpecificationAttributeDto>(objectPropertyNameValuePairs);
            productSpecificationAttributeDelta.Dto = dto;

            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We do not need to validate the product attribute id, because this will happen in the model binder using the dto validator.
            var productSpecificationAttributeId = productSpecificationAttributeDelta.Dto.Id;

            var productSpecificationAttribute = await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(productSpecificationAttributeId);
            if (productSpecificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product specification attribute", "not found");
            }

            productSpecificationAttributeDelta.Merge(productSpecificationAttribute);

            await _specificationAttributeService.UpdateProductSpecificationAttributeAsync(productSpecificationAttribute);

            await CustomerActivityService.InsertActivityAsync("EditProductSpecificationAttribute", productSpecificationAttribute.Id.ToString());

            // Preparing the result dto of the new product attribute
            var productSpecificationAttributeDto = _dtoHelper.PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

            var productSpecificatoinAttributesRootObjectDto = new ProductSpecificationAttributesRootObjectDto();
            productSpecificatoinAttributesRootObjectDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);

            var json = JsonFieldsSerializer.Serialize(productSpecificatoinAttributesRootObjectDto, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/productspecificationattributes/{id}", Name = "DeleteProductSpecificationAttribute")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> DeleteProductSpecificationAttribute([FromRoute] int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var productSpecificationAttribute = await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(id);
            if (productSpecificationAttribute == null)
            {
                return Error(HttpStatusCode.NotFound, "product specification attribute", "not found");
            }

            await _specificationAttributeService.DeleteProductSpecificationAttributeAsync(productSpecificationAttribute);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteProductSpecificationAttribute",
                                                   await LocalizationService.GetResourceAsync("ActivityLog.DeleteProductSpecificationAttribute"),
                                                   productSpecificationAttribute);

            return new RawJsonActionResult("{}");
        }
    }
}
