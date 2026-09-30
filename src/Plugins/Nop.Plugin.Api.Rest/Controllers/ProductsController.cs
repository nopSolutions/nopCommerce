using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Rest.Infrastructure;
using Nop.Plugin.Api.Rest.Mappings;
using Nop.Plugin.Api.Rest.Models;
using Nop.Plugin.Api.Rest.Models.Requests;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Rest.Controllers
{
    [ApiController]
    [Route("api/rest/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILocalizationService _localizationService;

        public ProductsController(IProductService productService, IUrlRecordService urlRecordService,
            ILocalizationService localizationService)
        {
            _productService = productService;
            _urlRecordService = urlRecordService;
            _localizationService = localizationService;
        }

        // GET api/rest/products?pageIndex=0&pageSize=20&keywords=&categoryId=&manufacturerId=&priceMin=&priceMax=
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = PagingHelper.DEFAULT_PAGE_SIZE,
            [FromQuery] string? keywords = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? manufacturerId = null,
            [FromQuery] decimal? priceMin = null,
            [FromQuery] decimal? priceMax = null,
            [FromQuery] bool searchSku = true,
            [FromQuery] bool searchDescriptions = false)
        {
            var index = PagingHelper.NormalizePageIndex(pageIndex);
            var size = PagingHelper.NormalizePageSize(pageSize);

            //the services take id lists, while a caller narrows a listing to the single category or
            //manufacturer it asked about. Zero means "no filter", matching the rest of the query string.
            var categoryIds = categoryId is > 0 ? new List<int> { categoryId.Value } : null;
            var manufacturerIds = manufacturerId is > 0 ? new List<int> { manufacturerId.Value } : null;

            var products = await _productService.SearchProductsAsync(
                pageIndex: index,
                pageSize: size,
                categoryIds: categoryIds,
                manufacturerIds: manufacturerIds,
                priceMin: priceMin,
                priceMax: priceMax,
                keywords: keywords,
                searchSku: searchSku,
                searchDescriptions: searchDescriptions);

            return Ok(products.ToPagedResult().Map(p => p.ToSummaryDto()));
        }

        // GET api/rest/products/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await GetProductAsync(id);
            if (product == null) return NotFound();

            return Ok(product.ToDto());
        }

        // POST api/rest/products
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddProductRequest request)
        {
            if (request == null) return BadRequest(new { error = "A request body is required." });

            var name = request.Name?.Trim();
            if (string.IsNullOrEmpty(name))
                return BadRequest(new { error = await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Name.Required") });

            if (request.Price < 0 || request.OldPrice < 0 || request.ProductCost < 0)
                return BadRequest(new { error = "Prices cannot be negative." });

            if (!Enum.IsDefined(typeof(ProductType), request.ProductTypeId))
                return BadRequest(new { error = $"Unknown product type identifier '{request.ProductTypeId}'." });

            var product = new Product
            {
                Name = name,
                Sku = request.Sku,
                Gtin = request.Gtin,
                ManufacturerPartNumber = request.ManufacturerPartNumber,
                ShortDescription = request.ShortDescription ?? string.Empty,
                FullDescription = request.FullDescription ?? string.Empty,
                ProductTypeId = request.ProductTypeId,
                ProductTemplateId = request.ProductTemplateId,
                VendorId = request.VendorId,
                VisibleIndividually = request.VisibleIndividually,
                Published = request.Published,
                CallForPrice = request.CallForPrice,
                CustomerEntersPrice = request.CustomerEntersPrice,
                Price = request.Price,
                OldPrice = request.OldPrice,
                ProductCost = request.ProductCost,
                IsShipEnabled = request.IsShipEnabled,
                IsFreeShipping = request.IsFreeShipping,
                IsTaxExempt = request.IsTaxExempt,
                ManageInventoryMethodId = request.ManageInventoryMethodId,
                StockQuantity = request.StockQuantity,
                MinStockQuantity = request.MinStockQuantity,
                BackorderModeId = request.BackorderModeId,
                OrderMinimumQuantity = request.OrderMinimumQuantity,
                OrderMaximumQuantity = request.OrderMaximumQuantity,
                Weight = request.Weight,
                Length = request.Length,
                Width = request.Width,
                Height = request.Height,
                MarkAsNew = request.MarkAsNew,
                DisplayOrder = request.DisplayOrder,
                AllowCustomerReviews = request.AllowCustomerReviews,
                DisableBuyButton = request.DisableBuyButton,
                AvailableStartDateTimeUtc = request.AvailableStartDateTimeUtc,
                AvailableEndDateTimeUtc = request.AvailableEndDateTimeUtc,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                Deleted = false
            };
            await _productService.InsertProductAsync(product);

            await SaveSlugAsync(product, name);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product.ToDto());
        }

        // PATCH api/rest/products/{id}
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateProductRequest request)
        {
            if (request == null) return BadRequest(new { error = "A request body is required." });

            var product = await GetProductAsync(id);
            if (product == null) return NotFound();

            if (request.Price < 0 || request.OldPrice < 0 || request.ProductCost < 0)
                return BadRequest(new { error = "Prices cannot be negative." });

            if (request.Name != null && string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { error = await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Name.Required") });

            if (request.ProductTypeId.HasValue && !Enum.IsDefined(typeof(ProductType), request.ProductTypeId.Value))
                return BadRequest(new { error = $"Unknown product type identifier '{request.ProductTypeId}'." });

            //an explicit null cannot be told apart from an omitted property, so the nullable strings are only
            //written when a value was sent; the caller clears a field by sending an empty string instead
            if (request.Sku != null) product.Sku = request.Sku;
            if (request.Gtin != null) product.Gtin = request.Gtin;
            if (request.ManufacturerPartNumber != null) product.ManufacturerPartNumber = request.ManufacturerPartNumber;
            if (request.ShortDescription != null) product.ShortDescription = request.ShortDescription;
            if (request.FullDescription != null) product.FullDescription = request.FullDescription;
            if (request.ProductTypeId.HasValue) product.ProductTypeId = request.ProductTypeId.Value;
            if (request.ProductTemplateId.HasValue) product.ProductTemplateId = request.ProductTemplateId.Value;
            if (request.VendorId.HasValue) product.VendorId = request.VendorId.Value;
            if (request.VisibleIndividually.HasValue) product.VisibleIndividually = request.VisibleIndividually.Value;
            if (request.Published.HasValue) product.Published = request.Published.Value;
            if (request.CallForPrice.HasValue) product.CallForPrice = request.CallForPrice.Value;
            if (request.CustomerEntersPrice.HasValue) product.CustomerEntersPrice = request.CustomerEntersPrice.Value;
            if (request.Price.HasValue) product.Price = request.Price.Value;
            if (request.OldPrice.HasValue) product.OldPrice = request.OldPrice.Value;
            if (request.ProductCost.HasValue) product.ProductCost = request.ProductCost.Value;
            if (request.IsShipEnabled.HasValue) product.IsShipEnabled = request.IsShipEnabled.Value;
            if (request.IsFreeShipping.HasValue) product.IsFreeShipping = request.IsFreeShipping.Value;
            if (request.IsTaxExempt.HasValue) product.IsTaxExempt = request.IsTaxExempt.Value;
            if (request.ManageInventoryMethodId.HasValue) product.ManageInventoryMethodId = request.ManageInventoryMethodId.Value;
            if (request.StockQuantity.HasValue) product.StockQuantity = request.StockQuantity.Value;
            if (request.MinStockQuantity.HasValue) product.MinStockQuantity = request.MinStockQuantity.Value;
            if (request.BackorderModeId.HasValue) product.BackorderModeId = request.BackorderModeId.Value;
            if (request.OrderMinimumQuantity.HasValue) product.OrderMinimumQuantity = request.OrderMinimumQuantity.Value;
            if (request.OrderMaximumQuantity.HasValue) product.OrderMaximumQuantity = request.OrderMaximumQuantity.Value;
            if (request.Weight.HasValue) product.Weight = request.Weight.Value;
            if (request.Length.HasValue) product.Length = request.Length.Value;
            if (request.Width.HasValue) product.Width = request.Width.Value;
            if (request.Height.HasValue) product.Height = request.Height.Value;
            if (request.MarkAsNew.HasValue) product.MarkAsNew = request.MarkAsNew.Value;
            if (request.DisplayOrder.HasValue) product.DisplayOrder = request.DisplayOrder.Value;
            if (request.AllowCustomerReviews.HasValue) product.AllowCustomerReviews = request.AllowCustomerReviews.Value;
            if (request.DisableBuyButton.HasValue) product.DisableBuyButton = request.DisableBuyButton.Value;
            if (request.AvailableStartDateTimeUtc.HasValue) product.AvailableStartDateTimeUtc = request.AvailableStartDateTimeUtc;
            if (request.AvailableEndDateTimeUtc.HasValue) product.AvailableEndDateTimeUtc = request.AvailableEndDateTimeUtc;

            var name = request.Name?.Trim();
            var nameChanged = name != null && name != product.Name;
            if (name != null) product.Name = name;

            product.UpdatedOnUtc = DateTime.UtcNow;
            await _productService.UpdateProductAsync(product);

            //the slug is derived from the name, so it has to be rebuilt whenever the name changes
            if (nameChanged) await SaveSlugAsync(product, product.Name);

            return Ok(product.ToDto());
        }

        // DELETE api/rest/products/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await GetProductAsync(id);
            if (product == null) return NotFound();

            await _productService.DeleteProductAsync(product);

            return NoContent();
        }

        #region Utilities

        /// <summary>
        /// Gets a product by identifier, but only when it exists and has not been deleted.
        /// The repository returns soft deleted products, so the flag has to be checked here.
        /// </summary>
        protected virtual async Task<Product> GetProductAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            return product == null || product.Deleted ? null : product;
        }

        /// <summary>
        /// Validates and stores the search engine name of a product, so it gets a store URL
        /// </summary>
        protected virtual async Task SaveSlugAsync(Product product, string name)
        {
            var seName = await _urlRecordService.ValidateSeNameAsync(product, string.Empty, name, true);
            await _urlRecordService.SaveSlugAsync(product, seName, 0);
        }

        #endregion
    }
}