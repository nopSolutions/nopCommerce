using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public class AbcProductAttributeService : ProductAttributeService, IAbcProductAttributeService
    {
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;

        // We need to exclude Warranty, Home Delivery, and Pickup until we've fully deployed
        // the Delivery Options functionality
        private string[] excludedProductAttributes = new string[]
        {
            "Home Delivery",
            "Pickup",
            "Warranty"
        };

        public AbcProductAttributeService(
            IRepository<PredefinedProductAttributeValue> predefinedProductAttributeValueRepository,
            IRepository<ProductAttribute> productAttributeRepository,
            IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
            IRepository<ProductAttributeMapping> productAttributeMappingRepository,
            IRepository<ProductAttributeValue> productAttributeValueRepository,
            IStaticCacheManager staticCacheManager)
        : base(predefinedProductAttributeValueRepository, productAttributeRepository, productAttributeCombinationRepository,
               productAttributeMappingRepository, productAttributeValueRepository, staticCacheManager)
        {
            _productAttributeRepository = productAttributeRepository;
            _productAttributeMappingRepository = productAttributeMappingRepository;
        }

        public async Task<ProductAttribute> GetProductAttributeByNameAsync(string name)
        {
            var query = from pa in _productAttributeRepository.Table
                where pa.Name == name
                select pa;

            return await query.FirstOrDefaultAsync();
        }

        public async Task SaveProductAttributeAsync(ProductAttribute pa)
        {
            var existingPa = (await GetAllProductAttributesAsync()).FirstOrDefault(epa => epa.Name == pa.Name);

            if (existingPa != null)
            {
                pa.Id = existingPa.Id;
                await UpdateProductAttributeAsync(pa);
            }
            else
            {
                await InsertProductAttributeAsync(pa);
            }
        }

        public async Task<IList<ProductAttributeMapping>> SaveProductAttributeMappingsAsync(
            int productId,
            IList<ProductAttributeMapping> pams,
            string[] excludedPas)
        {
            var existingPams = await GetProductAttributeMappingsByProductIdAsync(productId);

            excludedPas = excludedPas.Concat(excludedProductAttributes).ToArray();
            existingPams = await existingPams.WhereAwait(
                async pam => !excludedPas.Contains((await GetProductAttributeByIdAsync(pam.ProductAttributeId)).Name)
            ).ToListAsync();

            var toBeDeleted = existingPams.Where(e => !pams.Any(n => n.EqualTo(e)));
            var toBeInserted = pams.Where(n => !existingPams.Any(e => e.EqualTo(n)));

            // Handles duplicates
            var groupedPams = existingPams.GroupBy(pam => pam.ProductAttributeId).Select(g => g.First());
            var duplicatePams = existingPams.Except(groupedPams);

            toBeDeleted.Union(duplicatePams).ToList().ForEach(async pam => await DeleteProductAttributeMappingAsync(pam));
            toBeInserted.ToList().ForEach(async pam => await InsertProductAttributeMappingAsync(pam));

            return await groupedPams.Union(toBeInserted).ToListAsync();
        }

        public async Task<bool> ProductHasDeliveryOptionsAsync(int productId)
        {
            var deliveryOptionsPa = await GetProductAttributeByNameAsync("Delivery/Pickup Options");
            var query = from pam in _productAttributeMappingRepository.Table
                where pam.ProductAttributeId == deliveryOptionsPa.Id && pam.ProductId == productId
                select pam;

            return await query.AnyAsync();
        }
    }
}