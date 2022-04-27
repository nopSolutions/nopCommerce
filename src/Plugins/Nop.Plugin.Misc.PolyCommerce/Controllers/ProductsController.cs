using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.PolyCommerce.Models;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Seo;
using Dapper;
using Nop.Plugin.Misc.PolyCommerce.Models.Dto;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISpecificationAttributeService _specificationAttributeService;


        public ProductsController(IRepository<Product> productRepository,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IProductAttributeService productAttributeService,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            ISpecificationAttributeService specificationAttributeService)
        {
            _productRepository = productRepository;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _categoryService = categoryService;
            _productAttributeService = productAttributeService;
            _specificationAttributeService = specificationAttributeService;
        }

        [Route("api/polycommerce/products")]
        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 100, string minModifiedDateStr = null, string maxModifiedDateStr = null)
        {
            var dataSettings = DataSettingsManager.LoadSettings();
            var storeToken = Request.Headers.TryGetValue("Store-Token", out var values) ? values.First() : null;

            var store = await PolyCommerceHelper.GetPolyCommerceStoreByToken(storeToken);

            if (store == null)
            {
                return Unauthorized();
            }

            var minModifiedDate = DateTime.TryParseExact(minModifiedDateStr, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out var minModifiedDateVal) ? minModifiedDateVal.ToUniversalTime() : (DateTime?)null;
            var maxModifiedDate = DateTime.TryParseExact(maxModifiedDateStr, "o", CultureInfo.InvariantCulture, DateTimeStyles.None, out var maxModifiedDateVal) ? maxModifiedDateVal.ToUniversalTime() : (DateTime?)null;

            var skipRecords = (page - 1) * pageSize;

            var products = new List<Product>();
            int count = 0;

            if (minModifiedDate.HasValue && maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                .Where(x => !x.Deleted && x.UpdatedOnUtc >= minModifiedDate.Value && x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct)
                .Skip(skipRecords)
                .Take(pageSize)
                .ToListAsync();

                count = await _productRepository.Table
                .CountAsync(x => !x.Deleted && x.UpdatedOnUtc >= minModifiedDate.Value && x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct);
            }
            else if (!minModifiedDate.HasValue && !maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                .Where(x => !x.Deleted && x.ProductTypeId == (int)ProductType.SimpleProduct)
                .Skip(skipRecords)
                .Take(pageSize)
                .ToListAsync();

                count = await _productRepository.Table.CountAsync(x => !x.Deleted && x.ProductTypeId == (int)ProductType.SimpleProduct);
            }
            else if (minModifiedDate.HasValue && !maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                    .Where(x => !x.Deleted && x.UpdatedOnUtc >= minModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct)
                    .Skip(skipRecords)
                    .Take(pageSize)
                    .ToListAsync();

                count = await _productRepository.Table
                .CountAsync(x => !x.Deleted && x.UpdatedOnUtc >= minModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct);
            }
            else if (!minModifiedDate.HasValue && maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                    .Where(x => !x.Deleted && x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct)
                    .Skip(skipRecords)
                    .Take(pageSize)
                    .ToListAsync();

                count = await _productRepository.Table
                .CountAsync(x => !x.Deleted && x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct);
            }

            var productIds = products.Select(x => x.Id);

            Dictionary<int, List<SpecificationAttributeMappingDto>> specificationAttributeMappings = new Dictionary<int, List<SpecificationAttributeMappingDto>>();

            using (var conn = new SqlConnection(dataSettings.ConnectionString))
            {
                var attributeMappings = await conn.QueryAsync<SpecificationAttributeMappingDto>(@"select 
                                                                                                p.Id as ProductId,
                                                                                                sa.[Name] as [Label], 
                                                                                                sao.[Name] as [Value]
                                                                                                from Product p
                                                                                                join [dbo].[Product_SpecificationAttribute_Mapping] psm on psm.ProductId = p.Id
                                                                                                join SpecificationAttributeOption sao on sao.Id = psm.SpecificationAttributeOptionId
                                                                                                join SpecificationAttribute sa on sa.Id = sao.SpecificationAttributeId
                                                                                                where p.Id in @ProductIds", new { ProductIds = productIds });

                specificationAttributeMappings = attributeMappings.GroupBy(x => x.ProductId).ToDictionary(x => x.Key, v => v.ToList());
            }

            List<PolyCommerceProduct> mappedProducts = new List<PolyCommerceProduct>();

            foreach(var product in products)
            {
                var model = await PreparePolyCommerceModel(product, specificationAttributeMappings);
                mappedProducts.Add(model);
            }

            var response = new PolyCommerceApiResponse<PolyCommerceProduct>
            {
                Results = mappedProducts,
                TotalCount = count,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [Route("api/polycommerce/decrease_stock")]
        [HttpPost]
        public async Task<IActionResult> DecreaseStock([FromBody]PolyCommerceDecreaseStockModel model)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId);
            await _productService.AdjustInventoryAsync(product, model.Quantity * -1);
            return Ok(new { Success = true });
        }

        private async Task<PolyCommerceProduct> PreparePolyCommerceModel(Product product, Dictionary<int, List<SpecificationAttributeMappingDto>> specificationAttributeMappings)
        {
            var categories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);
            var productPictures = await _pictureService.GetPicturesByProductIdAsync(product.Id);
            var productAttributeCombinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);

            var specificationAttributes = specificationAttributeMappings.TryGetValue(product.Id, out var vals) ? vals : new List<SpecificationAttributeMappingDto>();

            List<string> images = new List<string>();

            foreach(var p in productPictures)
            {
                var imageUrl = await _pictureService.GetPictureUrlAsync(p.Id);
                images.Add(imageUrl);
            }

            var mappedProduct = new PolyCommerceProduct
            {
                CategoryId = categories.FirstOrDefault()?.CategoryId,
                CostPrice = product.ProductCost,
                Description = product.FullDescription,
                Height = product.Height,
                Weight = product.Weight,
                Mpn = product.ManufacturerPartNumber,
                InventoryLevel = product.StockQuantity,
                BrandId = product.VendorId,
                ExternalProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Sku = product.Sku,
                Depth = product.Length,
                Width = product.Width,
                Slug = await _urlRecordService.GetSeNameAsync(product),
                RetailPrice = product.OldPrice,
                IsDownload = product.IsDownload,
                MinInventoryLevel = product.MinStockQuantity,
                Condition = "NEW",
                Gtin = product.Gtin,
                Images = string.Join(",", images),
                ProductAttributes = specificationAttributes.Select(x => new PolyCommerceProductAttribute { Name = x.Label, Value = x.Value }).ToList()
            };

            if (productAttributeCombinations != null && productAttributeCombinations.Any())
            {
                try
                {
                    List<PolyCommerceProductVariation> variations = new List<PolyCommerceProductVariation>();

                    foreach (var productAttrComb in productAttributeCombinations)
                    {
                        var attributes = await _productAttributeParser.ParseProductAttributeMappingsAsync(productAttrComb.AttributesXml);

                        var imageUrl = productAttrComb.PictureId != 0 ? await _pictureService.GetPictureUrlAsync(productAttrComb.PictureId) : null;

                        var variation = new PolyCommerceProductVariation
                        {
                            InventoryLevel = productAttrComb.StockQuantity,
                            Sku = productAttrComb.Sku,
                            Gtin = productAttrComb.Gtin,
                            Mpn = productAttrComb.ManufacturerPartNumber,
                            ExternalProductId = productAttrComb.Id,
                            Price = productAttrComb.OverriddenPrice ?? product.Price,
                            ProductVariationOptionValues = new List<PolyCommerceProductVariationOptionValue>(),
                            Images = imageUrl
                        };

                        var attributeIds = attributes.Select(x => x.ProductAttributeId);
                        var attributeLookup = await _productAttributeService.GetProductAttributeByIdsAsync(attributeIds.ToArray());

                        foreach (var attribute in attributes)
                        {
                            // only RadioList and DropdownList are supported in PolyCommerce.
                            if (attribute.AttributeControlType != AttributeControlType.RadioList && attribute.AttributeControlType != AttributeControlType.DropdownList)
                            {
                                AddProductVariationError(mappedProduct, "NopCommerce product is currently incompatible with PolyCommerce. Only RadioList and DropdownList product attribute combinations are supported.");
                                return mappedProduct;
                            }

                            var attributeValues = _productAttributeParser.ParseValues(productAttrComb.AttributesXml, attribute.Id);

                            // handle unexpcted scenarios...
                            if (attributeValues == null || !attributeValues.Any())
                            {
                                AddProductVariationError(mappedProduct, "attributeValues can't be null or empty. Please contact PolyCommerce support.");
                                return mappedProduct;
                            }

                            if (attributeValues.Count > 1)
                            {
                                AddProductVariationError(mappedProduct, "attributeValues can't contain more than 1 value. Please contact PolyCommerce support.");
                                return mappedProduct;
                            }

                            var attributeValueId = attributeValues.First();

                            if (!int.TryParse(attributeValueId, out var attributeValueIdVal))
                            {
                                AddProductVariationError(mappedProduct, "attributeValueId could not be parsed as an int. Please contact PolyCommerce support.");
                                return mappedProduct;
                            }
                            
                            var values = await _productAttributeParser.ParseProductAttributeValuesAsync(productAttrComb.AttributesXml);
                            var value = values.FirstOrDefault(x => x.Id == attributeValueIdVal);

                            var option = attributeLookup.FirstOrDefault(x => x.Id == attribute.ProductAttributeId);

                            if (option != null && value != null)
                            {
                                variation.ProductVariationOptionValues.Add(new PolyCommerceProductVariationOptionValue
                                {
                                    Option = option.Name,
                                    Value = value.Name
                                });
                            }
                        }

                        mappedProduct.ProductVariations.Add(variation);
                    }
                }
                catch (Exception ex)
                {
                    AddProductVariationError(mappedProduct, $"Could not process product attribute combinations. {ex.Message}");
                    return mappedProduct;
                }
            }

            if (product.ParentGroupedProductId > 0 && string.IsNullOrWhiteSpace(mappedProduct.Description))
            {
                Product parentProduct = await _productRepository.GetByIdAsync(product.ParentGroupedProductId);
                mappedProduct.Description = parentProduct.FullDescription ?? parentProduct.ShortDescription;
            }

            return mappedProduct;
        }

        private void AddProductVariationError(PolyCommerceProduct mappedProduct, string errorMsg)
        {
            mappedProduct.Errors.Add(errorMsg);
            mappedProduct.ProductVariations.Clear();
        }
    }
}
