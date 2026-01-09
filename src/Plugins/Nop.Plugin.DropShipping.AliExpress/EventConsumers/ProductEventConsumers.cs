using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Plugin.DropShipping.AliExpress.Services;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Media;


namespace Nop.Plugin.DropShipping.AliExpress.EventConsumers;

/// <summary>
/// Event consumer for product updates
/// </summary>
public class ProductInsertedEventConsumer : IConsumer<EntityInsertedEvent<Product>>
{
    private readonly IAliExpressProductMappingService _mappingService;
    private readonly IAliExpressService _aliExpressService;
    private readonly IPictureService _pictureService;
    private readonly IProductService _productService;
    private readonly ILogger _logger;
    private readonly AliExpressSettings _settings;

    public ProductInsertedEventConsumer(
        IAliExpressProductMappingService mappingService,
        IAliExpressService aliExpressService,
        IPictureService pictureService,
        IProductService productService,
        ILogger logger,
        AliExpressSettings settings)
    {
        _mappingService = mappingService;
        _aliExpressService = aliExpressService;
        _pictureService = pictureService;
        _productService = productService;
        _logger = logger;
        _settings = settings;
    }

    public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
    {
        // This will be called when a product is created
        // We can check if AliExpress product data was provided and create mapping
        // The actual mapping will be handled in the controller when saving the product
        await Task.CompletedTask;
    }
}

/// <summary>
/// Event consumer for product updates
/// </summary>
public class ProductUpdatedEventConsumer : IConsumer<EntityUpdatedEvent<Product>>
{
    private readonly IAliExpressProductMappingService _mappingService;
    private readonly ILogger _logger;

    public ProductUpdatedEventConsumer(
        IAliExpressProductMappingService mappingService,
        ILogger logger)
    {
        _mappingService = mappingService;
        _logger = logger;
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
    {
        // Handle product updates if needed
        await Task.CompletedTask;
    }
}

/// <summary>
/// Event consumer for product deletion
/// </summary>
public class ProductDeletedEventConsumer : IConsumer<EntityDeletedEvent<Product>>
{
    private readonly IAliExpressProductMappingService _mappingService;
    private readonly ILogger _logger;

    public ProductDeletedEventConsumer(
        IAliExpressProductMappingService mappingService,
        ILogger logger)
    {
        _mappingService = mappingService;
        _logger = logger;
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
    {
        try
        {
            // Delete the AliExpress mapping when product is deleted
            var mapping = await _mappingService.GetMappingByProductIdAsync(eventMessage.Entity.Id);
            if (mapping != null)
            {
                await _mappingService.DeleteMappingAsync(mapping);
                await _logger.InformationAsync($"Deleted AliExpress mapping for product {eventMessage.Entity.Id}");
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error deleting AliExpress mapping: {ex.Message}", ex);
        }
    }
}
