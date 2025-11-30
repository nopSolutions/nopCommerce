using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Authorization.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.SpecificationAttributes;
using Nop.Plugin.Api.DTOs.Taxes;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.ModelBinders;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;

namespace Nop.Plugin.Api.Controllers
{
    [AuthorizePermission("ManageTaxSettings")]
    public class TaxesController : BaseApiController
    {
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IDTOHelper _dtoHelper;

        public TaxesController(
           IJsonFieldsSerializer jsonFieldsSerializer,
           IAclService aclService,
           ICustomerService customerService,
           IStoreMappingService storeMappingService,
           IStoreService storeService,
           IDiscountService discountService,
           ICustomerActivityService customerActivityService,
           ILocalizationService localizationService,
           IPictureService pictureService,
           ITaxCategoryService taxCategoryService, IDTOHelper dtoHelper) : base(jsonFieldsSerializer,
                  aclService,
                  customerService,
                  storeMappingService,
                  storeService,
                  discountService,
                  customerActivityService,
                  localizationService,
                  pictureService)
        {

            _taxCategoryService = taxCategoryService;
            _dtoHelper = dtoHelper;
        }

        [HttpGet]
        [Route("/api/taxcategories", Name = "getTaxCategories")]
        [ProducesResponseType(typeof(TaxCategoriesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> getAllTaxCategories([FromQuery] string fields = "", [FromQuery] int? limit = 250)
        {
            var allTaxes = await _taxCategoryService.GetAllTaxCategoriesAsync();

            IList<TaxCategoryDto> taxCategoryDtos = new List<TaxCategoryDto>();

            foreach (var tax in allTaxes)
            {
                var taxDto = _dtoHelper.prepareTaxCategoryDto(tax);
                taxCategoryDtos.Add(taxDto);
            }

            var taxRoot = new TaxCategoriesRootObject
            {
                Taxes = taxCategoryDtos
            };

            var json = JsonFieldsSerializer.Serialize(taxRoot, fields);
            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/taxCategories", Name = "createTaxCategory")]
        [ProducesResponseType(typeof(TaxCategoriesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateTaxCategory(
            [FromBody]
            TaxCategoryDto dto)
        {
            var jsonString = JsonConvert.SerializeObject(dto);
            var jsonProperties = JObject.Parse(jsonString).Properties().ToList();
            var objectPropertyNameValuePairs = jsonProperties.ToDictionary<JProperty, string, object>(jsonProperty => jsonProperty.Name, jsonProperty => jsonProperty.Value);

            var taxCategoryDelta = new Delta<TaxCategoryDto>(objectPropertyNameValuePairs);
            taxCategoryDelta.Dto = dto;
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            var taxCategory = new TaxCategory();
            taxCategory.Name = taxCategoryDelta.Dto.Name;
            taxCategory.DisplayOrder = taxCategoryDelta.Dto.DisplayOrder;

            await _taxCategoryService.InsertTaxCategoryAsync(taxCategory);

            var newTaxCategoryDto =  _dtoHelper.prepareTaxCategoryDto(taxCategory);

            var root = new TaxCategoriesRootObject();
            root.Taxes.Add(newTaxCategoryDto);
            var json = JsonFieldsSerializer.Serialize(root, string.Empty);
            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/taxCategories/{id}", Name = "DeleteTaxCategory")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteTax([FromRoute] int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var taxToDelete = await _taxCategoryService.GetTaxCategoryByIdAsync(id);

            if (taxToDelete == null)
            {
                return Error(HttpStatusCode.NotFound, "tax", "tax not found");
            }

            await _taxCategoryService.DeleteTaxCategoryAsync(taxToDelete);

            await CustomerActivityService.InsertActivityAsync("DeleteTaxCategory", await LocalizationService.GetResourceAsync("ActivityLog.DeleteTaxCategory"), taxToDelete);

            return new RawJsonActionResult("{}");
        }

    }
}
