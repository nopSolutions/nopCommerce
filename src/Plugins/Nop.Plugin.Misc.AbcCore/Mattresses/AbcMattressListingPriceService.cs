using Nop.Services.Catalog;
using System.Linq;
using System;
using Nop.Core;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public class AbcMattressListingPriceService : IAbcMattressListingPriceService
    {
        private readonly IAbcMattressEntryService _abcMattressEntryService;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IWebHelper _webHelper;

        public AbcMattressListingPriceService(
            IAbcMattressEntryService abcMattressEntryService,
            IProductService productService,
            IProductAttributeService productAttributeService,
            IWebHelper webHelper
        )
        {
            _abcMattressEntryService = abcMattressEntryService;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _webHelper = webHelper;
        }

        public async Task<(decimal Price, decimal OldPrice)?> GetListingPriceForMattressProductAsync(int productId)
        {
            // only need to do this if we're on the 'shop by size' categories
            // but we're opening this up to be called anywhere
            // including the JSON schema for google crawler
            var url = _webHelper.GetThisPageUrl(true);
            if (!IsSizeCategoryPage(url)) { return null; }

            var pams = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productId);
            ProductAttributeMapping mattressSizePam = null;
            foreach (var pam in pams)
            {
                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
                if (productAttribute?.Name == AbcMattressesConsts.MattressSizeName)
                {
                    mattressSizePam = pam;
                    break;
                }
            }

            if (mattressSizePam == null) // if no mattress sizes, return price
            {
                return null;
            }

            var value = (await _productAttributeService.GetProductAttributeValuesAsync(
                mattressSizePam.Id
            )).Where(pav => pav.Name == GetMattressSizeFromUrl(url))
             .FirstOrDefault();
            if (value == null) // no matching sizes, check for default (queen)
            {
                value = (await _productAttributeService.GetProductAttributeValuesAsync(
                    mattressSizePam.Id
                )).Where(pav => pav.Name == AbcMattressesConsts.Queen)
                .FirstOrDefault();
            }

            var product = await _productService.GetProductByIdAsync(productId);

            return value == null ? null :
                                    (Math.Round(product.Price + value.PriceAdjustment, 2),
                                     value.Cost); // using ProductAttributeValue Cost for OldPrice
        }

        private bool IsSizeCategoryPage(string url)
        {
            return url.Contains("twin-mattress") ||
                   url.Contains("twinxl-mattress") ||
                   url.Contains("twin-extra-long-mattress") ||
                   url.Contains("full-mattress") ||
                   url.Contains("queen-mattress") ||
                   url.Contains("king-mattress") ||
                   url.Contains("california-king-mattress");
        }

        // default to queen if nothing matches
        private string GetMattressSizeFromUrl(string url)
        {
            var slug = url.Substring(url.LastIndexOf('/') + 1);
            if (slug.Contains("california-king-mattress"))
            {
                return AbcMattressesConsts.CaliforniaKing;
            }
            if (slug.Contains("king-mattress"))
            {
                return AbcMattressesConsts.King;
            }
            if (slug.Contains("full-mattress"))
            {
                return AbcMattressesConsts.Full;
            }
            if (slug.Contains("twin-extra-long-mattress") ||
                slug.Contains("twinxl-mattress"))
            {
                return AbcMattressesConsts.TwinXL;
            }
            if (slug.Contains("twin-mattress"))
            {
                return AbcMattressesConsts.Twin;
            }

            return AbcMattressesConsts.Queen;
        }
    }
}
