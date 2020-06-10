using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.PolyCommerce.Models;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{

    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IPictureService _pictureService;

        public ProductsController(IRepository<Product> productRepository, IPictureService pictureService)
        {
            _productRepository = productRepository;
            _pictureService = pictureService;
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
                .Where(x => !x.Deleted && x.UpdatedOnUtc >= minModifiedDate.Value && x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0)
                .Skip(skipRecords)
                .Take(pageSize)
                .ToListAsync();

                count = await _productRepository.Table
                .CountAsync(x => !x.Deleted && x.UpdatedOnUtc >= minModifiedDate.Value && x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0);
            }
            else if (!minModifiedDate.HasValue && !maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                .Where(x => !x.Deleted && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0)
                .Skip(skipRecords)
                .Take(pageSize)
                .ToListAsync();

                count = await _productRepository.Table.CountAsync(x => !x.Deleted  && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0);
            }
            else if (minModifiedDate.HasValue && !maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                    .Where(x => !x.Deleted && x.UpdatedOnUtc >= minModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0)
                    .Skip(skipRecords)
                    .Take(pageSize)
                    .ToListAsync();

                count = await _productRepository.Table
                .CountAsync(x => !x.Deleted  && x.UpdatedOnUtc >= minModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0);
            }
            else if (!minModifiedDate.HasValue && maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                    .Where(x => !x.Deleted && x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0)
                    .Skip(skipRecords)
                    .Take(pageSize)
                    .ToListAsync();

                count = await _productRepository.Table
                .CountAsync(x => !x.Deleted && x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0);
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

        private PolyCommerceProduct PreparePolyCommerceModel(Product product) =>
         new PolyCommerceProduct
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
             ProductAttributes = product.ProductSpecificationAttributes.Select(x => new PolyCommerceProductAttribute { Name = x.SpecificationAttributeOption.SpecificationAttribute.Name, Value = x.SpecificationAttributeOption.Name })
         };
    }
}
