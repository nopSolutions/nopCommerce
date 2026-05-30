using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.Zettle.Domain;

namespace Nop.Plugin.Misc.Zettle.Services;

/// <summary>
/// Represents the service to manage synchronization records
/// </summary>
public class ZettleRecordService
{
    #region Fields

    protected readonly IRepository<Category> _categoryRepository;
    protected readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
    protected readonly IRepository<ProductCategory> _productCategoryRepository;
    protected readonly IRepository<Product> _productRepository;
    protected readonly IRepository<ZettleRecord> _repository;
    protected readonly ZettleSettings _zettleSettings;

    #endregion

    #region Ctor

    public ZettleRecordService(IRepository<Category> categoryRepository,
        IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IRepository<Product> productRepository,
        IRepository<ZettleRecord> repository,
        ZettleSettings zettleSettings)
    {
        _categoryRepository = categoryRepository;
        _productAttributeCombinationRepository = productAttributeCombinationRepository;
        _productCategoryRepository = productCategoryRepository;
        _productRepository = productRepository;
        _repository = repository;
        _zettleSettings = zettleSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare records to add
    /// </summary>
    /// <param name="productIds">Product identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the prepared records; the number of products that were not added
    /// </returns>
    protected async Task<(List<ZettleRecord> Records, int InvalidProducts)> PrepareRecordsToAddAsync(List<int> productIds)
    {
        var products = await _productRepository.GetByIdsAsync(productIds, null, false);
        var productsWithSku = products.Where(product => !string.IsNullOrEmpty(product.Sku)).ToList();
        var invalidProducts = products.Where(product => string.IsNullOrEmpty(product.Sku)).Count();
        var records = await productsWithSku.SelectManyAwait(async product =>
        {
            var uuid = GuidGenerator.GenerateTimeBasedGuid().ToString();
            var productRecord = new ZettleRecord
            {
                Active = _zettleSettings.SyncEnabled,
                ProductId = product.Id,
                Uuid = uuid,
                VariantUuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                PriceSyncEnabled = _zettleSettings.PriceSyncEnabled,
                ImageSyncEnabled = _zettleSettings.ImageSyncEnabled,
                InventoryTrackingEnabled = _zettleSettings.InventoryTrackingEnabled,
                OperationType = OperationType.Create
            };

            var combinations = await _productAttributeCombinationRepository
                .GetAllAsync(query => query.Where(combination => combination.ProductId == product.Id && !string.IsNullOrEmpty(combination.Sku)), null);
            var combinationsRecords = combinations.Select(combination => new ZettleRecord
            {
                Active = _zettleSettings.SyncEnabled,
                ProductId = product.Id,
                CombinationId = combination.Id,
                Uuid = uuid,
                VariantUuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                PriceSyncEnabled = _zettleSettings.PriceSyncEnabled,
                ImageSyncEnabled = _zettleSettings.ImageSyncEnabled,
                InventoryTrackingEnabled = _zettleSettings.InventoryTrackingEnabled,
                OperationType = OperationType.Create
            }).ToList();

            return new List<ZettleRecord> { productRecord }.Union(combinationsRecords);
        }).ToListAsync();

        return (records, invalidProducts);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get a record by the identifier
    /// </summary>
    /// <param name="id">Record identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the record for synchronization
    /// </returns>
    public async Task<ZettleRecord> GetRecordByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id, null);
    }

    /// <summary>
    /// Insert the record
    /// </summary>
    /// <param name="record">Record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertRecordAsync(ZettleRecord record)
    {
        await _repository.InsertAsync(record, false);
    }

    /// <summary>
    /// Insert records
    /// </summary>
    /// <param name="records">Records</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task InsertRecordsAsync(List<ZettleRecord> records)
    {
        await _repository.InsertAsync(records, false);
    }

    /// <summary>
    /// Update the record
    /// </summary>
    /// <param name="record">Record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateRecordAsync(ZettleRecord record)
    {
        await _repository.UpdateAsync(record, false);
    }

    /// <summary>
    /// Update records
    /// </summary>
    /// <param name="records">Records</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task UpdateRecordsAsync(List<ZettleRecord> records)
    {
        await _repository.UpdateAsync(records, false);
    }

    /// <summary>
    /// Delete the record
    /// </summary>
    /// <param name="record">Record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteRecordAsync(ZettleRecord record)
    {
        await _repository.DeleteAsync(record, false);
    }

    /// <summary>
    /// Delete records
    /// </summary>
    /// <param name="ids">Records identifiers</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteRecordsAsync(List<int> ids)
    {
        await _repository.DeleteAsync(record => ids.Contains(record.Id));
    }

    /// <summary>
    /// Clear all records
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ClearRecordsAsync()
    {
        if (_zettleSettings.ClearRecordsOnChangeCredentials)
            await _repository.TruncateAsync();
        else
        {
            var records = (await GetAllRecordsAsync()).ToList();
            foreach (var record in records)
            {
                record.ImageUrl = string.Empty;
                record.UpdatedOnUtc = null;
                record.OperationType = OperationType.Create;
            }
            await UpdateRecordsAsync(records);
        }
    }

    /// <summary>
    /// Get all records for synchronization
    /// </summary>
    /// <param name="productOnly">Whether to load only product records</param>
    /// <param name="active">Whether to load only active records; true - active only, false - inactive only, null - all records</param>
    /// <param name="operationTypes">Operation types; pass null to load all records</param>
    /// <param name="productUuid">Product unique identifier; pass null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the records for synchronization
    /// </returns>
    public async Task<IPagedList<ZettleRecord>> GetAllRecordsAsync(bool productOnly = false,
        bool? active = null, List<OperationType> operationTypes = null, string productUuid = null,
        int pageIndex = 0, int pageSize = int.MaxValue)
    {
        return await _repository.GetAllPagedAsync(query =>
        {
            if (productOnly)
                query = query.Where(record => record.ProductId > 0 && record.CombinationId == 0);

            if (active.HasValue)
                query = query.Where(record => record.Active == active.Value);

            if (operationTypes?.Any() ?? false)
                query = query.Where(record => operationTypes.Contains((OperationType)record.OperationTypeId));

            if (!string.IsNullOrEmpty(productUuid))
                query = query.Where(record => record.Uuid == productUuid);

            query = query.OrderBy(record => record.Id);

            return query;
        }, pageIndex, pageSize);
    }

    /// <summary>
    /// Create or update a record for synchronization
    /// </summary>
    /// <param name="operationType">Operation type</param>
    /// <param name="productId">Product identifier</param>
    /// <param name="attributeCombinationId">Product attribute combination identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task CreateOrUpdateRecordAsync(OperationType operationType, int productId, int attributeCombinationId = 0)
    {
        if (productId == 0 && attributeCombinationId == 0)
            return;

        var existingRecord = await _repository.Table.
            FirstOrDefaultAsync(record => record.ProductId == productId && record.CombinationId == attributeCombinationId);

        if (existingRecord is null)
        {
            if (operationType != OperationType.Create)
                return;

            if (!_zettleSettings.AutoAddRecordsEnabled)
                return;

            if (attributeCombinationId == 0 || (await _repository.Table.FirstOrDefaultAsync(record => record.ProductId == productId)) is not ZettleRecord productRecord)
            {
                var (records, _) = await PrepareRecordsToAddAsync([productId]);
                await InsertRecordsAsync(records);
            }
            else
            {
                await InsertRecordAsync(new()
                {
                    Active = _zettleSettings.SyncEnabled,
                    ProductId = productId,
                    CombinationId = attributeCombinationId,
                    Uuid = productRecord.Uuid,
                    VariantUuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                    PriceSyncEnabled = _zettleSettings.PriceSyncEnabled,
                    ImageSyncEnabled = _zettleSettings.ImageSyncEnabled,
                    InventoryTrackingEnabled = _zettleSettings.InventoryTrackingEnabled,
                    OperationType = operationType
                });
            }

            return;
        }

        switch (existingRecord.OperationType)
        {
            case OperationType.Create:
                if (operationType == OperationType.Delete)
                    await DeleteRecordAsync(existingRecord);
                return;

            case OperationType.Update:
                if (operationType == OperationType.Delete)
                {
                    existingRecord.OperationType = OperationType.Delete;
                    await UpdateRecordAsync(existingRecord);
                }
                return;

            case OperationType.Delete:
                if (operationType == OperationType.Create)
                {
                    existingRecord.OperationType = OperationType.Update;
                    await UpdateRecordAsync(existingRecord);
                }
                return;

            case OperationType.ImageChanged:
            case OperationType.None:
                existingRecord.OperationType = operationType;
                await UpdateRecordAsync(existingRecord);
                return;
        }
    }

    /// <summary>
    /// Add records for synchronization
    /// </summary>
    /// <param name="productIds">Product identifiers</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of products that were not added
    /// </returns>
    public async Task<int> AddRecordsAsync(List<int> productIds)
    {
        if (!productIds?.Any() ?? true)
            return 0;

        var newProductIds = productIds.Except(await _repository.Table.Select(record => record.ProductId).ToListAsync()).ToList();
        
        if (!newProductIds.Any())
            return 0;

        var (records, invalidProducts) = await PrepareRecordsToAddAsync(newProductIds);
        await InsertRecordsAsync(records);

        return invalidProducts;
    }

    /// <summary>
    /// Prepare records for synchronization
    /// </summary>
    /// <param name="records">Records</param>
    /// <returns>Records ready for synchronization</returns>
    public List<ProductToSync> PrepareToSyncRecords(List<ZettleRecord> records)
    {
        var recordIds = records.Select(record => record.Id).ToList();
        var productToSync = _repository.Table
            .Where(record => recordIds.Contains(record.Id))
            .Join(_productCategoryRepository.Table,
                record => record.ProductId,
                pc => pc.ProductId,
                (record, pc) => new { Record = record, ProductCategory = pc })
            .Join(_productRepository.Table,
                item => item.ProductCategory.ProductId,
                product => product.Id,
                (item, product) => new { Product = product, Record = item.Record, ProductCategory = item.ProductCategory })
            .Join(_categoryRepository.Table,
                item => item.ProductCategory.CategoryId,
                category => category.Id,
                (item, category) => new { Category = category, Product = item.Product, Record = item.Record, ProductCategory = item.ProductCategory })
            .Select(item => new
            {
                Id = item.Product.Id,
                Uuid = item.Record.Uuid,
                VariantUuid = item.Record.VariantUuid,
                Name = item.Product.Name,
                Sku = item.Product.Sku,
                Description = item.Product.ShortDescription,
                Price = item.Product.Price,
                ProductCost = item.Product.ProductCost,
                CategoryName = item.Category.Name,
                ImageUrl = item.Record.ImageUrl,
                ImageSyncEnabled = item.Record.ImageSyncEnabled,
                PriceSyncEnabled = item.Record.PriceSyncEnabled,
                ProductCategoryId = item.ProductCategory.Id,
                ProductCategoryDisplayOrder = item.ProductCategory.DisplayOrder
            })
            .GroupBy(item => item.Id)
            .Select(group => new ProductToSync
            {
                Id = group.Key,
                Uuid = group.FirstOrDefault().Uuid,
                VariantUuid = group.FirstOrDefault().VariantUuid,
                Name = group.FirstOrDefault().Name,
                Sku = group.FirstOrDefault().Sku,
                Description = group.FirstOrDefault().Description,
                Price = group.FirstOrDefault().Price,
                ProductCost = group.FirstOrDefault().ProductCost,
                CategoryName = group
                    .OrderBy(item => item.ProductCategoryDisplayOrder)
                    .ThenBy(item => item.ProductCategoryId)
                    .Select(item => item.CategoryName)
                    .FirstOrDefault(),
                ImageUrl = group.FirstOrDefault().ImageUrl,
                ImageSyncEnabled = group.FirstOrDefault().ImageSyncEnabled,
                PriceSyncEnabled = group.FirstOrDefault().PriceSyncEnabled
            })
            .ToList();

        return productToSync;
    }

    #endregion
}