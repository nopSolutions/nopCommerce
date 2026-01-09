using Nop.Data;
using Nop.Plugin.DropShipping.AliExpress.Domain;
using Nop.Plugin.DropShipping.AliExpress.Services;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;


namespace Nop.Plugin.DropShipping.AliExpress.ScheduledTasks;

/// <summary>
/// Scheduled task to sync AliExpress product prices and availability
/// </summary>
public class ProductSyncTask : IScheduleTask
{
    private readonly IAliExpressService _aliExpressService;
    private readonly IRepository<AliExpressProductMapping> _mappingRepository;
    private readonly IProductService _productService;
    private readonly ILogger _logger;
    private readonly AliExpressSettings _settings;

    public ProductSyncTask(
        IAliExpressService aliExpressService,
        IRepository<AliExpressProductMapping> mappingRepository,
        IProductService productService,
        ILogger logger,
        AliExpressSettings settings)
    {
        _aliExpressService = aliExpressService;
        _mappingRepository = mappingRepository;
        _productService = productService;
        _logger = logger;
        _settings = settings;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            if (!_settings.EnableDailySync)
            {
                await _logger.InformationAsync("AliExpress product sync is disabled");
                return;
            }

            // Check if it's the right hour to run
            if (DateTime.UtcNow.Hour != _settings.DailySyncHour)
                return;

            await _logger.InformationAsync("Starting AliExpress product sync...");

            var mappings = await _mappingRepository.GetAllAsync(query => query);
            var syncedCount = 0;
            var errorCount = 0;

            foreach (var mapping in mappings)
            {
                try
                {
                    // Get updated product details from AliExpress
                    var productDetails = await _aliExpressService.GetProductDetailsAsync(
                        mapping.AliExpressProductId,
                        _settings.DefaultShippingCountry);

                    if (productDetails != null)
                    {
                        // Update mapping with new information
                        mapping.IsAvailable = productDetails.IsAvailable;
                        mapping.AliExpressPrice = productDetails.BasePrice;
                        mapping.LastSyncOnUtc = DateTime.UtcNow;
                        mapping.LastSyncMessage = "Synced successfully";

                        // Get freight info
                        var freightInfo = await _aliExpressService.GetFreightInfoAsync(
                            mapping.AliExpressProductId,
                            1,
                            _settings.DefaultShippingCountry ?? "ZA");

                        if (freightInfo.Any())
                        {
                            var cheapestShipping = freightInfo.OrderBy(f => f.ShippingCost).First();
                            mapping.ShippingCost = cheapestShipping.ShippingCost;
                            mapping.ShippingServiceName = cheapestShipping.ServiceName;
                            mapping.EstimatedDeliveryDays = cheapestShipping.EstimatedDeliveryDays;
                        }

                        // Recalculate price
                        var baseAmount = mapping.AliExpressPrice + mapping.ShippingCost;
                        mapping.CustomsDuty = baseAmount * (_settings.CustomsDutyPercentage / 100);
                        var subtotal = baseAmount + mapping.CustomsDuty;
                        mapping.VatAmount = subtotal * (_settings.VatPercentage / 100);
                        var totalBeforeMargin = subtotal + mapping.VatAmount;
                        mapping.CalculatedPrice = totalBeforeMargin * (1 + mapping.MarginPercentage / 100);

                        await _mappingRepository.UpdateAsync(mapping);

                        // Update the NopCommerce product price
                        var product = await _productService.GetProductByIdAsync(mapping.ProductId);
                        if (product != null)
                        {
                            product.Price = mapping.CalculatedPrice;
                            await _productService.UpdateProductAsync(product);
                        }

                        syncedCount++;
                    }
                    else
                    {
                        mapping.LastSyncOnUtc = DateTime.UtcNow;
                        mapping.LastSyncMessage = "Product not found on AliExpress";
                        mapping.IsAvailable = false;
                        await _mappingRepository.UpdateAsync(mapping);
                        errorCount++;
                    }
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync($"Error syncing product {mapping.ProductId}: {ex.Message}", ex);
                    
                    mapping.LastSyncOnUtc = DateTime.UtcNow;
                    mapping.LastSyncMessage = $"Error: {ex.Message}";
                    await _mappingRepository.UpdateAsync(mapping);
                    
                    errorCount++;
                }
            }

            await _logger.InformationAsync(
                $"AliExpress product sync completed. Synced: {syncedCount}, Errors: {errorCount}");
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Error in AliExpress product sync task", ex);
        }
    }
}
