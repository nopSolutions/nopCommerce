using Nop.Data;
using Nop.Plugin.DropShipping.AliExpress.Domain;


namespace Nop.Plugin.DropShipping.AliExpress.Services;

/// <summary>
/// Interface for AliExpress product mapping service
/// </summary>
public interface IAliExpressProductMappingService
{
    /// <summary>
    /// Gets a product mapping by NopCommerce product ID
    /// </summary>
    Task<AliExpressProductMapping?> GetMappingByProductIdAsync(int productId);

    /// <summary>
    /// Gets a product mapping by AliExpress product ID
    /// </summary>
    Task<AliExpressProductMapping?> GetMappingByAliExpressProductIdAsync(long aliExpressProductId);

    /// <summary>
    /// Inserts a product mapping
    /// </summary>
    Task InsertMappingAsync(AliExpressProductMapping mapping);

    /// <summary>
    /// Updates a product mapping
    /// </summary>
    Task UpdateMappingAsync(AliExpressProductMapping mapping);

    /// <summary>
    /// Deletes a product mapping
    /// </summary>
    Task DeleteMappingAsync(AliExpressProductMapping mapping);

    /// <summary>
    /// Checks if a product has an AliExpress mapping
    /// </summary>
    Task<bool> HasMappingAsync(int productId);

    /// <summary>
    /// Gets all mappings for synchronization
    /// </summary>
    Task<IList<AliExpressProductMapping>> GetAllMappingsAsync();

    /// <summary>
    /// Calculates the final price for a product
    /// </summary>
    decimal CalculateFinalPrice(decimal productPrice, decimal shippingCost, decimal vatPercentage, decimal customsDutyPercentage, decimal marginPercentage);
}

/// <summary>
/// Service for managing AliExpress product mappings
/// </summary>
public class AliExpressProductMappingService : IAliExpressProductMappingService
{
    private readonly IRepository<AliExpressProductMapping> _mappingRepository;

    public AliExpressProductMappingService(IRepository<AliExpressProductMapping> mappingRepository)
    {
        _mappingRepository = mappingRepository;
    }

    public async Task<AliExpressProductMapping?> GetMappingByProductIdAsync(int productId)
    {
        return await _mappingRepository.Table
            .FirstOrDefaultAsync(m => m.ProductId == productId);
    }

    public async Task<AliExpressProductMapping?> GetMappingByAliExpressProductIdAsync(long aliExpressProductId)
    {
        return await _mappingRepository.Table
            .FirstOrDefaultAsync(m => m.AliExpressProductId == aliExpressProductId);
    }

    public async Task InsertMappingAsync(AliExpressProductMapping mapping)
    {
        await _mappingRepository.InsertAsync(mapping);
    }

    public async Task UpdateMappingAsync(AliExpressProductMapping mapping)
    {
        await _mappingRepository.UpdateAsync(mapping);
    }

    public async Task DeleteMappingAsync(AliExpressProductMapping mapping)
    {
        await _mappingRepository.DeleteAsync(mapping);
    }

    public async Task<bool> HasMappingAsync(int productId)
    {
        return await _mappingRepository.Table
            .AnyAsync(m => m.ProductId == productId);
    }

    public async Task<IList<AliExpressProductMapping>> GetAllMappingsAsync()
    {
        return await _mappingRepository.GetAllAsync(query => query);
    }

    public decimal CalculateFinalPrice(
        decimal productPrice,
        decimal internationalShipping,
        decimal vatPercentage,
        decimal customsDutyPercentage,
        decimal marginPercentage)
    {
        // 1. Customs value (CIF without insurance for simplicity)
        var customsValue = productPrice + internationalShipping;

        // 2. Customs duty
        var customsDuty = customsValue * (customsDutyPercentage / 100m);

        // 3. VAT on (customs value + duty)
        var vatAmount = (customsValue + customsDuty) * (vatPercentage / 100m);

        // 4. Landed cost
        var landedCost = customsValue + customsDuty + vatAmount;

        // 5. Apply margin (markup)
        var finalPriceExVat = landedCost * (1 + marginPercentage / 100m);

        return Math.Round(finalPriceExVat, 2);
    }

}
