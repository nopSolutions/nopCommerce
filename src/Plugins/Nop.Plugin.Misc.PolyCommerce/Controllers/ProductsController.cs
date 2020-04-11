using System;
using System.Collections.Generic;
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
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{

    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IWebHelper _webHelper;

        public ProductsController(IRepository<Product> productRepository, IWebHelper webHelper)
        {
            _productRepository = productRepository;
            _webHelper = webHelper;
        }

        [Route("api/polycommerce/products")]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 100, DateTime? minModifiedDate = null, DateTime? maxModifiedDate = null)
        {
            var storeToken = Request.Headers.TryGetValue("Store-Token", out var values) ? values.First() : null;

            var store = await PolyCommerceHelper.GetPolyCommerceStoreByToken(storeToken);

            if (store == null)
            {
                return Unauthorized();
            }

            var skipRecords = (page - 1) * pageSize;

            var products = new List<Product>();
            int count = 0;

            if (minModifiedDate.HasValue && maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                .Where(x => x.UpdatedOnUtc >= minModifiedDate.Value && x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0)
                .Skip(skipRecords)
                .Take(pageSize)
                .ToListAsync();

                count = await _productRepository.Table
                .CountAsync(x => x.UpdatedOnUtc >= minModifiedDate.Value && x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0);
            }
            else if (!minModifiedDate.HasValue && !maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                .Where(x => x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0)
                .Skip(skipRecords)
                .Take(pageSize)
                .ToListAsync();

                count = await _productRepository.Table.CountAsync(x => x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0);
            }
            else if (minModifiedDate.HasValue && !maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                    .Where(x => x.UpdatedOnUtc >= minModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0)
                    .Skip(skipRecords)
                    .Take(pageSize)
                    .ToListAsync();

                count = await _productRepository.Table
                .CountAsync(x => x.UpdatedOnUtc >= minModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0);
            }
            else if (!minModifiedDate.HasValue && maxModifiedDate.HasValue)
            {
                products = await _productRepository.Table
                    .Where(x => x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0)
                    .Skip(skipRecords)
                    .Take(pageSize)
                    .ToListAsync();

                count = await _productRepository.Table
                .CountAsync(x => x.UpdatedOnUtc <= maxModifiedDate.Value && x.ProductTypeId == (int)ProductType.SimpleProduct && x.ParentGroupedProductId == 0);
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

        private string GetPictureUrl(Picture picture)
        {
            if (picture == null)
                return "";
            var seName = picture.SeoFilename;
            var pictureId = picture.Id;
            var mimeType = picture.MimeType;

            var sb = new StringBuilder();
            sb.Append(_webHelper.GetStoreLocation());
            sb.Append("images/thumbs/");
            sb.Append(pictureId.ToString().PadLeft(7, '0'));
            sb.Append("_" + seName);
            sb.Append("." + mimeType.Replace("image/", ""));

            return sb.ToString();
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
             D
             MinInventoryLevel = product.MinStockQuantity,
             Condition = "NEW",
             Gtin = product.Gtin,
             Images = string.Join(",", product.ProductPictures.Select(x => GetPictureUrl(x.Picture))),
             ProductAttributes = product.ProductSpecificationAttributes.Select(x => new PolyCommerceProductAttribute { Name = x.SpecificationAttributeOption.SpecificationAttribute.Name, Value = x.SpecificationAttributeOption.Name })
         };
    }
}
