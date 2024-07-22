using System.Collections.Concurrent;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Catalog;

namespace Nop.Plugin.Payments.AmazonPay.Services;

public class DisallowedProducts
{
    private bool _initialized;

    private readonly ICategoryService _categoryService;
    private readonly IRepository<GenericAttribute> _genericAttributeRepository;

    private readonly ConcurrentDictionary<int, List<int>> _products;
    private readonly ConcurrentDictionary<int, bool> _categories;

    public DisallowedProducts(ICategoryService categoryService,
        IRepository<GenericAttribute> genericAttributeRepository)
    {
        _products = new ConcurrentDictionary<int, List<int>>();
        _categories = new ConcurrentDictionary<int, bool>();

        _categoryService = categoryService;
        _genericAttributeRepository = genericAttributeRepository;
    }

    public async Task InitAsync()
    {
        if (_initialized)
            return;

        var ga = await _genericAttributeRepository.Table.Where(ga =>
            ga.Key == AmazonPayDefaults.DoNotUseWithAmazonPayAttributeName && ga.Value == "true").ToListAsync();

        foreach (var genericAttribute in ga)
        {
            if (genericAttribute.KeyGroup.Equals(nameof(Product), StringComparison.InvariantCultureIgnoreCase))
                _products.TryAdd(genericAttribute.EntityId, null);

            if (genericAttribute.KeyGroup.Equals(nameof(Category), StringComparison.InvariantCultureIgnoreCase))
                _categories.TryAdd(genericAttribute.EntityId, true);
        }

        _initialized = true;
    }

    public async Task<bool> IsProductDisallowAsync(int productId)
    {
        await InitAsync();

        if (_products.ContainsKey(productId))
            return true;

        var categories = await _categoryService.GetProductCategoriesByProductIdAsync(productId);

        var disallowCategories = categories.Select(pc => pc.CategoryId).Where(c => _categories.ContainsKey(c)).ToList();

        if (disallowCategories.Any())
        {
            if (!_products.ContainsKey(productId))
                _products.TryAdd(productId, disallowCategories);
            else
                _products.TryUpdate(productId, disallowCategories, _products[productId]);

            return true;
        }

        return false;
    }

    public async Task UpdateDataAsync(BaseEntity entity, bool disallowed)
    {
        if (entity == null)
            return;

        await InitAsync();

        if (entity is Product)
        {
            if (disallowed && !_products.ContainsKey(entity.Id))
                _products.TryAdd(entity.Id, null);

            if (!disallowed && _products.ContainsKey(entity.Id))
                _products.TryRemove(entity.Id, out _);
        }

        if (entity is Category)
        {
            if (disallowed && !_categories.ContainsKey(entity.Id))
                _categories.TryAdd(entity.Id, true);

            if (!disallowed && _categories.ContainsKey(entity.Id))
            {
                _categories.TryRemove(entity.Id, out _);

                var itemsToUpdate = _products.Where(key => key.Value?.Contains(entity.Id) ?? false);

                foreach (var item in itemsToUpdate)
                {
                    var newValues = item.Value.Where(p => p != entity.Id).ToList();

                    if (newValues.Any())
                        _products.TryUpdate(item.Key, newValues, item.Value);
                    else
                        _products.TryRemove(item.Key, out _);
                }
            }
        }
    }
}
