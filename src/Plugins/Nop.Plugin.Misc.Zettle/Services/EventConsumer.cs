using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Plugin.Misc.Zettle.Domain;
using Nop.Services.Catalog;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.Zettle.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumer :
        IConsumer<EntityInsertedEvent<Product>>,
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>,
        IConsumer<EntityInsertedEvent<ProductCategory>>,
        IConsumer<EntityDeletedEvent<ProductCategory>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        IConsumer<EntityInsertedEvent<ProductPicture>>,
        IConsumer<EntityUpdatedEvent<ProductPicture>>,
        IConsumer<EntityDeletedEvent<ProductPicture>>,
        IConsumer<EntityUpdatedEvent<ProductAttribute>>,
        IConsumer<EntityDeletedEvent<ProductAttribute>>,
        IConsumer<EntityUpdatedEvent<ProductAttributeValue>>,
        IConsumer<EntityDeletedEvent<ProductAttributeValue>>,
        IConsumer<EntityInsertedEvent<ProductAttributeCombination>>,
        IConsumer<EntityUpdatedEvent<ProductAttributeCombination>>,
        IConsumer<EntityDeletedEvent<ProductAttributeCombination>>,
        IConsumer<EntityInsertedEvent<StockQuantityHistory>>

    {
        #region Fields

        protected readonly ICategoryService _categoryService;
        protected readonly IProductAttributeParser _productAttributeParser;
        protected readonly IProductAttributeService _productAttributeService;
        protected readonly IProductService _productService;
        protected readonly ZettleRecordService _zettleRecordService;
        protected readonly ZettleService _zettleService;
        protected readonly ZettleSettings _zettleSettings;

        #endregion

        #region Ctor

        public EventConsumer(ICategoryService categoryService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            ZettleRecordService zettleRecordService,
            ZettleService zettleService,
            ZettleSettings zettleSettings)
        {
            _categoryService = categoryService;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _zettleRecordService = zettleRecordService;
            _zettleService = zettleService;
            _zettleSettings = zettleSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Create, eventMessage.Entity.Id);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (!eventMessage.Entity.Deleted)
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.Id);
            else
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, eventMessage.Entity.Id);
                var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(eventMessage.Entity.Id);
                foreach (var combination in combinations)
                {
                    await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, combination.ProductId, combination.Id);
                }
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, eventMessage.Entity.Id);
            var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(eventMessage.Entity.Id);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, combination.ProductId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (!_zettleSettings.CategorySyncEnabled)
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.ProductId);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (!_zettleSettings.CategorySyncEnabled)
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.ProductId);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (!_zettleSettings.CategorySyncEnabled)
                return;

            var mappings = await _categoryService.GetProductCategoriesByCategoryIdAsync(eventMessage.Entity.Id, showHidden: true);
            foreach (var mapping in mappings)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, mapping.ProductId);
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (!_zettleSettings.CategorySyncEnabled)
                return;

            var mappings = await _categoryService.GetProductCategoriesByCategoryIdAsync(eventMessage.Entity.Id, showHidden: true);
            foreach (var mapping in mappings)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, mapping.ProductId);
            }
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductPicture> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var pictures = await _productService.GetProductPicturesByProductIdAsync(eventMessage.Entity.ProductId);
            if (eventMessage.Entity.DisplayOrder <= pictures.Min(picture => picture.DisplayOrder))
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.ImageChanged, eventMessage.Entity.ProductId);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductPicture> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var pictures = await _productService.GetProductPicturesByProductIdAsync(eventMessage.Entity.ProductId);
            if (eventMessage.Entity.DisplayOrder <= pictures.Min(picture => picture.DisplayOrder))
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.ImageChanged, eventMessage.Entity.ProductId);
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductPicture> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.ImageChanged, eventMessage.Entity.ProductId);
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductAttribute> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var products = await _productService.GetProductsByProductAttributeIdAsync(eventMessage.Entity.Id);
            foreach (var product in products)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, product.Id);
                var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
                foreach (var combination in combinations)
                {
                    await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.ProductId, combination.Id);
                }
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductAttribute> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var products = await _productService.GetProductsByProductAttributeIdAsync(eventMessage.Entity.Id);
            foreach (var product in products)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, product.Id);
                var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
                foreach (var combination in combinations)
                {
                    var mappings = await _productAttributeParser.ParseProductAttributeMappingsAsync(combination.AttributesXml);
                    if (mappings.Any(mapping => mapping.ProductAttributeId == eventMessage.Entity.Id))
                        await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, combination.ProductId, combination.Id);
                    else
                        await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.ProductId, combination.Id);
                }
            }
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductAttributeValue> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var mapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(eventMessage.Entity.ProductAttributeMappingId);
            if (mapping is null)
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, mapping.ProductId);
            var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(mapping.ProductId);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.ProductId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductAttributeValue> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            var mapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(eventMessage.Entity.ProductAttributeMappingId);
            if (mapping is null)
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, mapping.ProductId);
            var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(mapping.ProductId);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.ProductId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<ProductAttributeCombination> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.ProductId);
            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Create, eventMessage.Entity.ProductId, eventMessage.Entity.Id);
            var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(eventMessage.Entity.ProductId);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.ProductId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ProductAttributeCombination> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.ProductId);
            var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(eventMessage.Entity.ProductId);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.ProductId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<ProductAttributeCombination> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, eventMessage.Entity.ProductId);
            await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Delete, eventMessage.Entity.ProductId, eventMessage.Entity.Id);
            var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(eventMessage.Entity.ProductId);
            foreach (var combination in combinations)
            {
                await _zettleRecordService.CreateOrUpdateRecordAsync(OperationType.Update, combination.ProductId, combination.Id);
            }
        }

        /// <summary>
        /// Handle entity created event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<StockQuantityHistory> eventMessage)
        {
            if (eventMessage.Entity is null)
                return;

            if (!ZettleService.IsConfigured(_zettleSettings))
                return;

            if (eventMessage.Entity.QuantityAdjustment == 0)
                return;

            if (eventMessage.Entity.Message.StartsWith(ZettleDefaults.SystemName))
                return;

            await _zettleService.ChangeInventoryBalanceAsync(eventMessage.Entity.ProductId,
                eventMessage.Entity.CombinationId ?? 0, eventMessage.Entity.QuantityAdjustment);
        }

        #endregion
    }
}