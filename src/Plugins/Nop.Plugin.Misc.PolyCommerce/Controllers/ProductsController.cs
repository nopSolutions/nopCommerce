using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.PolyCommerce.Models;
using Nop.Services.Catalog;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{

    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;

        public ProductsController(IRepository<Product> productRepository, IPictureService pictureService, IProductAttributeParser productAttributeParser)
        {
            _productRepository = productRepository;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
        }

        [Route("api/polycommerce/products")]
        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 100, string minModifiedDateStr = null, string maxModifiedDateStr = null)
        {

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

            var mappedProducts = products.ConvertAll(PreparePolyCommerceModel);

            var response = new PolyCommerceApiResponse<PolyCommerceProduct>
            {
                Results = mappedProducts,
                TotalCount = count,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        private PolyCommerceProduct PreparePolyCommerceModel(Product product)
        {

            var mappedProduct = new PolyCommerceProduct
            {
                CategoryId = product.ProductCategories?.FirstOrDefault()?.CategoryId,
                CostPrice = product.ProductCost,
                Description = product.FullDescription,
                Height = product.Height,
                Weight = product.Weight,
                Mpn = product.ManufacturerPartNumber,
                InventoryLevel = product.StockQuantity,
                BrandId = product.VendorId,
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Sku = product.Sku,
                Depth = product.Length,
                Width = product.Width,
                RetailPrice = product.OldPrice,
                IsDownload = product.IsDownload,
                MinInventoryLevel = product.MinStockQuantity,
                Condition = "NEW",
                Gtin = product.Gtin,
                Images = string.Join(",", product.ProductPictures.Select(x => _pictureService.GetPictureUrl(x.Picture))),
                ProductAttributes = product.ProductSpecificationAttributes.Select(x => new PolyCommerceProductAttribute { Name = x.SpecificationAttributeOption.SpecificationAttribute.Name, Value = x.SpecificationAttributeOption.Name }).ToList()
            };

            if (product.ProductAttributeCombinations != null && product.ProductAttributeCombinations.Any())
            {
                try
                {
                    List<PolyCommerceProductVariation> variations = new List<PolyCommerceProductVariation>();

                    foreach (var productAttrComb in product.ProductAttributeCombinations)
                    {
                        var attributes = _productAttributeParser.ParseProductAttributeMappings(productAttrComb.AttributesXml);

                        var externalProductId = long.Parse(product.Id.ToString() + productAttrComb.Id.ToString());

                        var imageUrl = productAttrComb.PictureId != 0 ? _pictureService.GetPictureUrl(productAttrComb.PictureId) : null;

                        var variation = new PolyCommerceProductVariation
                        {
                            InventoryLevel = productAttrComb.StockQuantity,
                            Sku = productAttrComb.Sku,
                            Gtin = productAttrComb.Gtin,
                            Mpn = productAttrComb.ManufacturerPartNumber,
                            ExternalProductId = externalProductId,
                            Price = productAttrComb.OverriddenPrice ?? product.Price,
                            ProductVariationOptionValues = new List<PolyCommerceProductVariationOptionValue>(),
                            Images = imageUrl
                        };

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

                            if (attribute.ProductAttributeValues == null || !attribute.ProductAttributeValues.Any())
                            {
                                AddProductVariationError(mappedProduct, $"ProductAttributeValues can't be null or empty. Please contact PolyCommerce support.");
                                return mappedProduct;
                            }

                            var combinationValue = attribute.ProductAttributeValues.FirstOrDefault(x => x.Id == attributeValueIdVal);

                            if (combinationValue == null)
                            {
                                AddProductVariationError(mappedProduct, $"combinationValue could not be found via AttributeValueTypeId {attributeValueIdVal}. Please contact PolyCommerce support.");
                                return mappedProduct;
                            }

                            variation.ProductVariationOptionValues.Add(new PolyCommerceProductVariationOptionValue
                            {
                                Option = attribute.ProductAttribute.Name,
                                Value = combinationValue.Name
                            });
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
                Product parentProduct = _productRepository.GetById(product.ParentGroupedProductId);
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
