using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Plugin.Misc.Zettle.Domain;
using Nop.Plugin.Misc.Zettle.Domain.Api;
using Nop.Plugin.Misc.Zettle.Domain.Api.Image;
using Nop.Plugin.Misc.Zettle.Domain.Api.Inventory;
using Nop.Plugin.Misc.Zettle.Domain.Api.OAuth;
using Nop.Plugin.Misc.Zettle.Domain.Api.Product;
using Nop.Plugin.Misc.Zettle.Domain.Api.Pusher;
using Nop.Plugin.Misc.Zettle.Domain.Api.Secure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Logging;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.Zettle.Services;

/// <summary>
/// Represents the plugin service
/// </summary>
public class ZettleService
{
    #region Fields

    protected readonly CurrencySettings _currencySettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDiscountService _discountService;
    protected readonly ILogger _logger;
    protected readonly IPictureService _pictureService;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly ISettingService _settingService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly ZettleHttpClient _zettleHttpClient;
    protected readonly ZettleRecordService _zettleRecordService;
    protected readonly ZettleSettings _zettleSettings;

    protected Dictionary<string, string> _locations = new();

    #endregion

    #region Ctor

    public ZettleService(CurrencySettings currencySettings,
        ICurrencyService currencyService,
        IDiscountService discountService,
        ILogger logger,
        IPictureService pictureService,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        ISettingService settingService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        ZettleHttpClient zettleHttpClient,
        ZettleRecordService zettleRecordService,
        ZettleSettings zettleSettings)
    {
        _currencySettings = currencySettings;
        _currencyService = currencyService;
        _discountService = discountService;
        _logger = logger;
        _pictureService = pictureService;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _settingService = settingService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _zettleHttpClient = zettleHttpClient;
        _zettleRecordService = zettleRecordService;
        _zettleSettings = zettleSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Handle function and get result
    /// </summary>
    /// <typeparam name="TResult">Result type</typeparam>
    /// <param name="function">Function</param>
    /// <param name="logErrors">Whether to log errors</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the result; error if exists
    /// </returns>
    protected async Task<(TResult Result, string Error)> HandleFunctionAsync<TResult>(Func<Task<TResult>> function, bool logErrors = true)
    {
        try
        {
            //ensure that plugin is configured
            if (!IsConfigured(_zettleSettings))
                throw new NopException("Plugin not configured");

            return (await function(), default);
        }
        catch (Exception exception)
        {
            var errorMessage = exception.Message;
            if (logErrors)
            {
                var logMessage = $"{ZettleDefaults.SystemName} error: {Environment.NewLine}{errorMessage}";
                await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());
            }

            return (default, errorMessage);
        }
    }

    #region Sync

    /// <summary>
    /// Import discounts to Zettle library
    /// </summary>
    /// <param name="log">Log message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ImportDiscountsAsync(StringBuilder log)
    {
        //if enabled
        if (!_zettleSettings.DiscountSyncEnabled)
            return;

        log.AppendLine("Add discounts...");

        var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

        //add only assigned to order subtotal discounts
        var existingDiscounts = await _zettleHttpClient.RequestAsync<GetDiscountsRequest, DiscountList>(new());
        var discounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToOrderSubTotal, showHidden: true);
        var discountsToAdd = discounts
            .Where(discount => !existingDiscounts.Any(existingDiscount => existingDiscount.ExternalReference == discount.Id.ToString()))
            .ToList();

        foreach (var discount in discountsToAdd)
        {
            var request = new CreateDiscountRequest
            {
                Uuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                Name = discount.Name,
                Description = discount.Name,
                ExternalReference = discount.Id.ToString()
            };
            if (!discount.UsePercentage)
            {
                request.Amount = new Domain.Api.Product.Discount.DiscountAmount
                {
                    CurrencyId = storeCurrency.CurrencyCode.ToUpper(),
                    Amount = storeCurrency.CurrencyCode.ToUpper() switch
                    {
                        "JPY" or "ISK" => Convert.ToInt32(Math.Round(discount.DiscountAmount, 0)),
                        _ => Convert.ToInt32(Math.Round(discount.DiscountAmount * 100, 0))
                    }
                };
            }
            else
                request.Percentage = discount.DiscountPercentage;

            log.AppendLine($"\tAdd discount '{discount.Name}'");

            await _zettleHttpClient.RequestAsync<CreateDiscountRequest, ApiResponse>(request);
        }
    }

    /// <summary>
    /// Delete products from Zettle library
    /// </summary>
    /// <param name="log">Log message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ImportDeletedAsync(StringBuilder log)
    {
        log.AppendLine("Delete products...");

        //get records to delete
        var records = await _zettleRecordService
            .GetAllRecordsAsync(active: true, operationTypes: [OperationType.Delete]);
        var idsToDelete = records
            .Where(record => !string.IsNullOrEmpty(record.Uuid) && record.ProductId > 0 && record.CombinationId == 0)
            .Select(record => record.Uuid)
            .Distinct()
            .ToList();

        if (idsToDelete.Any())
            log.AppendLine($"\tDelete {idsToDelete.Count} products (#{string.Join(", #", idsToDelete)})");

        //if needed, also delete all existing products
        if (_zettleSettings.DeleteBeforeImport)
        {
            var idsToKeep = (await _zettleRecordService.GetAllRecordsAsync(productOnly: true, active: true))
                .Where(record => !string.IsNullOrEmpty(record.Uuid))
                .Select(record => record.Uuid)
                .Distinct()
                .Except(idsToDelete)
                .ToList();

            var products = await _zettleHttpClient.RequestAsync<GetProductsRequest, ProductList>(new());
            var existingIds = products.Select(product => product.Uuid).ToList();

            idsToDelete.AddRange(existingIds.Except(idsToKeep).ToList());

            log.AppendLine($"\tAlso delete all existing library items before importing products");
        }

        idsToDelete = idsToDelete.Distinct().ToList();
        if (idsToDelete.Any())
            await _zettleHttpClient.RequestAsync<DeleteProductsRequest, ApiResponse>(new DeleteProductsRequest { ProductUuids = idsToDelete });

        await _zettleRecordService.DeleteRecordsAsync(records.Select(record => record.Id).ToList());
    }

    /// <summary>
    /// Change product images in Zettle library
    /// </summary>
    /// <param name="log">Log message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ImportImageChangedAsync(StringBuilder log)
    {
        log.AppendLine("Change images...");

        //upload new images
        var records = (await _zettleRecordService
                .GetAllRecordsAsync(active: true, operationTypes: [OperationType.ImageChanged]))
            .Where(record => record.ImageSyncEnabled && !string.IsNullOrEmpty(record.Uuid))
            .ToList();
        await UploadImagesAsync(records, true, log);

        //then update appropriate products
        var products = records
            .GroupBy(record => record.ProductId)
            .Select(group => new
            {
                ProductRecord = group.FirstOrDefault(record => record.CombinationId == 0),
                CombinationRecords = group.Where(record => record.CombinationId > 0 && !string.IsNullOrEmpty(record.VariantUuid)).ToList()
            })
            .ToList();
        foreach (var product in products)
        {
            var existingProduct = await _zettleHttpClient
                .RequestAsync<GetProductRequest, Product>(new GetProductRequest { Uuid = product.ProductRecord.Uuid });
            var request = new UpdateProductRequest
            {
                Uuid = existingProduct.Uuid,
                Name = existingProduct.Name,
                ETag = $"\"{existingProduct.ETag}\""
            };
            if (!product.CombinationRecords.Any())
            {
                request.Presentation = new Product.ProductPresentation { ImageUrl = product.ProductRecord?.ImageUrl };
                request.Variants =
                [
                    new() { Uuid = product.ProductRecord.VariantUuid }
                ];
            }
            else
            {
                request.Variants = product.CombinationRecords.Select(record => new Product.ProductVariant
                {
                    Uuid = record.VariantUuid,
                    Presentation = new Product.ProductPresentation { ImageUrl = record.ImageUrl }
                }).ToList();
            }

            log.AppendLine($"\tAdd image to product #{product.ProductRecord.ProductId}");

            await _zettleHttpClient.RequestAsync<UpdateProductRequest, ApiResponse>(request);
        }
    }

    /// <summary>
    /// Update inventory tracking balances in Zettle library
    /// </summary>
    /// <param name="log">Log message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task ImportInventoryTrackingAsync(StringBuilder log)
    {
        log.AppendLine("Update inventory tracking...");

        var records = (await _zettleRecordService
                .GetAllRecordsAsync(active: true, operationTypes: [OperationType.Update]))
            .Where(record => record.InventoryTrackingEnabled && !string.IsNullOrEmpty(record.Uuid))
            .ToList();
        if (!records.Any())
            return;

        var storeBalance = await _zettleHttpClient.RequestAsync<GetLocationInventoryBalanceRequest, LocationInventoryBalance>(new());

        var products = records
            .GroupBy(record => record.ProductId)
            .Select(group => new
            {
                ProductRecord = group.FirstOrDefault(record => record.CombinationId == 0),
                CombinationRecords = group.Where(record => record.CombinationId > 0 && !string.IsNullOrEmpty(record.VariantUuid)).ToList()
            })
            .Where(product => !storeBalance.TrackedProducts?.Contains(product.ProductRecord.Uuid, StringComparer.InvariantCultureIgnoreCase) ?? true)
            .ToList();
        if (!products.Any())
            return;

        var productChanges = new List<CreateTrackingRequest.ProductBalanceChange>();
        var recordsToUpdate = new List<ZettleRecord>();

        foreach (var product in products)
        {
            log.AppendLine($"\tStart inventory tracking for product #{product.ProductRecord.ProductId}");

            //get current quantity if exists
            var productQuantity = storeBalance.Variants
                ?.FirstOrDefault(balance => balance.ProductUuid == product.ProductRecord.Uuid && balance.VariantUuid == product.ProductRecord.VariantUuid)
                ?.Balance ?? 0;
            (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) productRecordToStart = (product.ProductRecord, productQuantity, null);
            var combinationRecordsToStart = new List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)>();
            foreach (var combinationRecord in product.CombinationRecords)
            {
                //get current quantity if exists
                var combinationQuantity = storeBalance.Variants
                    ?.FirstOrDefault(balance => balance.ProductUuid == combinationRecord.Uuid && balance.VariantUuid == combinationRecord.VariantUuid)
                    ?.Balance ?? 0;
                combinationRecordsToStart.Add((combinationRecord, combinationQuantity, null));
            }
            var productChange = await PrepareInventoryBalanceChangeAsync(InventoryBalanceChangeType.StartTracking,
                productRecordToStart, combinationRecordsToStart);
            if (productChange is null)
                continue;

            productChanges.Add(productChange);
            recordsToUpdate.AddRange(product.CombinationRecords.Union([product.ProductRecord]));
        }

        await UpdateInventoryBalanceAsync(productChanges, recordsToUpdate);
    }

    /// <summary>
    /// Create or update products in Zettle library
    /// </summary>
    /// <param name="log">Log message</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the import details
    /// </returns>
    protected async Task<Import> ImportCreatedOrUpdatedAsync(StringBuilder log)
    {
        log.AppendLine("Create and update products...");

        //check currency match
        var storeCurrency = await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);
        var (accountInfo, _) = await GetAccountInfoAsync();
        var priceSyncAvailable = string.Equals(storeCurrency.CurrencyCode, accountInfo.Currency, StringComparison.InvariantCultureIgnoreCase);

        //prepare price function
        int preparePrice(decimal price) => storeCurrency.CurrencyCode.ToUpper() switch
        {
            "JPY" or "ISK" => Convert.ToInt32(Math.Round(price, 0)),
            _ => Convert.ToInt32(Math.Round(price * 100, 0))
        };

        Import import = null;
        var pageIndex = 0;
        while (true)
        {
            //we can add up to 2000 products per request, but when uploading images, this may be too much
            var records = await _zettleRecordService.GetAllRecordsAsync(active: true,
                    operationTypes: [OperationType.Create, OperationType.Update],
                pageIndex: pageIndex++,
            pageSize: _zettleSettings.ImportProductsNumber);
            if (!records.Any())
                return import;

            log.AppendLine($"\tPrepare {records.Count} records to import");

            //upload images if needed
            await UploadImagesAsync(records.ToList(), false, log);

            //prepare products to import
            var products = await _zettleRecordService.PrepareToSyncRecords(records.ToList()).SelectAwait(async product =>
            {
                var request = new Product
                {
                    Uuid = product.Uuid,
                    ExternalReference = product.Sku,
                    Name = product.Name,
                    Id = product.Id,
                    Description = product.Description,
                    CreateWithDefaultTax = _zettleSettings.DefaultTaxEnabled,
                    Category = new Product.ProductCategory
                    {
                        Name = product.CategoryName,
                        Uuid = GuidGenerator.GenerateTimeBasedGuid().ToString()
                    },
                    Metadata = new Product.ProductMetadata
                    {
                        InPos = true,
                        Source = new Product.ProductMetadata.ProductSource
                        {
                            External = true,
                            Name = ZettleDefaults.PartnerIdentifier
                        }
                    }
                };

                //set image
                if (product.ImageSyncEnabled && !string.IsNullOrEmpty(product.ImageUrl))
                    request.Presentation = new Product.ProductPresentation { ImageUrl = product.ImageUrl };

                var combinationRecords = records
                    .Where(record => record.ProductId == product.Id && record.CombinationId != 0)
                    .ToList();
                if (!combinationRecords.Any())
                {
                    //a single variant
                    var variant = new Product.ProductVariant
                    {
                        Uuid = product.VariantUuid,
                        Name = product.Name,
                        Sku = product.Sku,
                        Description = product.Description
                    };

                    //set the price if available
                    if (product.PriceSyncEnabled && priceSyncAvailable)
                    {
                        variant.Price = new Product.ProductVariant.ProductPrice
                        {
                            Amount = preparePrice(product.Price),
                            CurrencyId = accountInfo.Currency
                        };
                        variant.CostPrice = new Product.ProductVariant.ProductPrice
                        {
                            Amount = preparePrice(product.ProductCost),
                            CurrencyId = accountInfo.Currency
                        };
                    }
                    request.Variants = [variant];
                }
                else
                {
                    //or multi variants
                    var productCombinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
                    var productAttributMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                    var productAttributes = await productAttributMappings.SelectAwait(async mapping =>
                    {
                        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(mapping.ProductAttributeId);
                        var productAttributeValues = await _productAttributeService.GetProductAttributeValuesAsync(mapping.Id);
                        return new { Name = productAttribute.Name, Values = productAttributeValues.Select(value => value.Name).ToList() };
                    }).ToListAsync();

                    request.VariantOptionDefinitions = new Product.ProductVariantDefinitions
                    {
                        Definitions = productAttributes.Select(attribute => new Product.ProductVariantDefinitions.ProductVariantOptionDefinition
                        {
                            Name = attribute.Name,
                            Properties = attribute.Values.Select(value => new Product.ProductVariantDefinitions.ProductVariantOptionDefinition.ProductVariantOptionProperty
                            {
                                Value = value
                            }).ToList()
                        }).ToList()
                    };

                    var combinations = combinationRecords
                        .Join(productCombinations,
                            record => record.CombinationId,
                            combination => combination.Id,
                            (record, combination) => new { Record = record, Combination = combination })
                        .ToList();
                    request.Variants = await combinations.SelectAwait(async combination =>
                    {
                        var variant = new Product.ProductVariant
                        {
                            Uuid = combination.Record.VariantUuid,
                            Name = product.Name,
                            Sku = combination.Combination.Sku,
                            Description = product.Description
                        };

                        //set image
                        if (combination.Record.ImageSyncEnabled && !string.IsNullOrEmpty(combination.Record.ImageUrl))
                            variant.Presentation = new Product.ProductPresentation { ImageUrl = combination.Record.ImageUrl };

                        //set the price if available
                        if (combination.Record.PriceSyncEnabled && priceSyncAvailable)
                        {
                            variant.Price = new Product.ProductVariant.ProductPrice
                            {
                                Amount = preparePrice(combination.Combination.OverriddenPrice ?? product.Price),
                                CurrencyId = accountInfo.Currency
                            };

                            var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(combination.Combination.AttributesXml);
                            var attributesCost = attributeValues
                                .Where(value => value.AttributeValueType == Core.Domain.Catalog.AttributeValueType.Simple)
                                .Sum(value => value.Cost);
                            variant.CostPrice = new Product.ProductVariant.ProductPrice
                            {
                                Amount = preparePrice(product.ProductCost + attributesCost),
                                CurrencyId = accountInfo.Currency
                            };
                        }

                        variant.Options = await (await _productAttributeParser.ParseProductAttributeMappingsAsync(combination.Combination.AttributesXml))
                            .SelectAwait(async mapping =>
                            {
                                var attribute = await _productAttributeService.GetProductAttributeByIdAsync(mapping.ProductAttributeId);
                                var values = await _productAttributeParser.ParseProductAttributeValuesAsync(combination.Combination.AttributesXml, mapping.Id);
                                return new Product.ProductVariant.ProductVariantOption { Name = attribute.Name, Value = values.FirstOrDefault()?.Name };
                            })
                            .ToListAsync();

                        return variant;
                    }).ToListAsync();
                }
                return request;
            }).ToListAsync();

            log.AppendLine($"\tImport {products.Count} products (#{string.Join(", #", products.Select(product => product.Id).ToList())})");

            import = await _zettleHttpClient.RequestAsync<CreateImportRequest, Import>(new CreateImportRequest { Products = products });

            log.AppendLine($"\t\tImport ({import.Uuid}) created at {import.Created?.ToLongTimeString()}");
        }
    }

    /// <summary>
    /// Upload images for the passed records
    /// </summary>
    /// <param name="records">Records</param>
    /// <param name="update">Whether to update existing images</param>
    /// <param name="log">Log message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task UploadImagesAsync(IList<ZettleRecord> records, bool update, StringBuilder log)
    {
        //ensure MediaSettings.UseAbsoluteImagePath is enabled (used for images uploading)
        if (!_mediaSettings.UseAbsoluteImagePath)
            throw new NopException("For the correct image uploading need to use absolute pictures path (MediaSettings.UseAbsoluteImagePath setting)");

        //prepare images to upload
        var recordsWithImages = await records
            .Where(record => record.ImageSyncEnabled && (update || string.IsNullOrEmpty(record.ImageUrl)))
            .SelectAwait(async record =>
            {
                var product = await _productService.GetProductByIdAsync(record.ProductId);
                var combination = await _productAttributeService.GetProductAttributeCombinationByIdAsync(record.CombinationId);
                var picture = await _pictureService.GetProductPictureAsync(product, combination?.AttributesXml);
                var ext = await _pictureService.GetFileExtensionFromMimeTypeAsync(picture.MimeType);
                var (url, _) = await _pictureService.GetPictureUrlAsync(picture);
                return new { Record = record, Url = url, Format = ext };
            })
            .ToListAsync();
        var imagesToUpload = recordsWithImages.Select(record => new CreateImageRequest
        {
            ImageFormat = record.Format?.ToUpper().Replace("JPG", "JPEG"),
            ImageUrl = record.Url
        }).ToList();

        if (!imagesToUpload.Any())
            return;

        log.AppendLine($"\tUpload {recordsWithImages.Count} new images");

        //upload images
        var images = await _zettleHttpClient
            .RequestAsync<CreateImagesRequest, ImageList>(new CreateImagesRequest { ImageUploads = imagesToUpload });

        log.AppendLine($"\t{images.Uploaded?.Count ?? 0} images uploaded successfully and {images.Invalid?.Count ?? 0} failed to upload");

        //set uploaded images URLs to records
        var recordsToUpdate = images.Uploaded?
            .SelectMany(image =>
            {
                var recordsWithUploadedImage = recordsWithImages
                    .Where(record => string.Equals(record.Url, image.Source, StringComparison.InvariantCultureIgnoreCase))
                    .Select(record => record.Record)
                    .ToList();
                foreach (var record in recordsWithUploadedImage)
                {
                    record.ImageUrl = image.ImageUrls?.FirstOrDefault();
                }
                return recordsWithUploadedImage;
            })
            .Distinct()
            .ToList();
        await _zettleRecordService.UpdateRecordsAsync(recordsToUpdate);
    }

    #region Inventory

    /// <summary>
    /// Prepare product inventory balance changes
    /// </summary>
    /// <param name="changeType">Inventory balance change type</param>
    /// <param name="productRecord">Product record with initial stock quantity and qunatity adjustment</param>
    /// <param name="combinationRecords">Combination records with initial stock quantity and qunatity adjustment</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains list of balance changes
    /// </returns>
    protected async Task<CreateTrackingRequest.ProductBalanceChange> PrepareInventoryBalanceChangeAsync(InventoryBalanceChangeType changeType,
        (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) productRecord,
        List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)> combinationRecords)
    {
        //ensure that inventory is tracked for the product
        var product = await _productService.GetProductByIdAsync(productRecord.Record?.ProductId ?? 0);
        if (product is null || product.ManageInventoryMethod == Core.Domain.Catalog.ManageInventoryMethod.DontManageStock)
            return null;

        var productChange = new CreateTrackingRequest.ProductBalanceChange
        {
            ProductUuid = productRecord.Record.Uuid,
            TrackingStatusChange = changeType == InventoryBalanceChangeType.StartTracking ? "START_TRACKING" : "NO_CHANGE"
        };

        //Zettle Inventory service keeps track of inventory balances by moving product items between so-called locations
        var fromLocation = await (changeType switch
        {
            InventoryBalanceChangeType.StartTracking or InventoryBalanceChangeType.Restock => GetLocationAsync("SUPPLIER"),
            InventoryBalanceChangeType.Purchase or InventoryBalanceChangeType.Void => GetLocationAsync("STORE"),
            _ => GetLocationAsync("SUPPLIER")
        });
        var toLocation = await (changeType switch
        {
            InventoryBalanceChangeType.StartTracking or InventoryBalanceChangeType.Restock => GetLocationAsync("STORE"),
            InventoryBalanceChangeType.Purchase => GetLocationAsync("SOLD"),
            InventoryBalanceChangeType.Void => GetLocationAsync("BIN"),
            _ => GetLocationAsync("BIN")
        });

        if (!combinationRecords.Any())
        {
            //get initial quantity
            var quantity = changeType == InventoryBalanceChangeType.StartTracking
                ? product.StockQuantity - productRecord.StockQuantity
                : productRecord.QuantityAdjustment ?? 0;
            if (quantity != 0)
            {
                productChange.VariantChanges =
                [
                    new()
                    {
                        FromLocationUuid = quantity > 0 ? fromLocation : toLocation,
                        ToLocationUuid = quantity > 0 ? toLocation : fromLocation,
                        VariantUuid = productRecord.Record.VariantUuid,
                        Change = Math.Abs(quantity)
                    }
                ];
            }
        }
        else
        {
            var combinations = await _productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
            productChange.VariantChanges = await combinationRecords.SelectAwait(async combinationRecord =>
            {
                var combination = await _productAttributeService.GetProductAttributeCombinationByIdAsync(combinationRecord.Record.CombinationId);

                //get initial quantity
                var quantity = changeType == InventoryBalanceChangeType.StartTracking
                    ? (product.ManageInventoryMethod == Core.Domain.Catalog.ManageInventoryMethod.ManageStockByAttributes
                        ? (combination?.StockQuantity ?? 0) - combinationRecord.StockQuantity
                        : (product.StockQuantity / combinations.Count) - combinationRecord.StockQuantity)
                    : combinationRecord.QuantityAdjustment ?? 0;
                if (quantity == 0)
                    return null;

                return new CreateTrackingRequest.VariantBalanceChange
                {
                    FromLocationUuid = quantity > 0 ? fromLocation : toLocation,
                    ToLocationUuid = quantity > 0 ? toLocation : fromLocation,
                    VariantUuid = combinationRecord.Record.VariantUuid,
                    Change = Math.Abs(quantity)
                };
            }).Where(variantChange => variantChange is not null).ToListAsync();
        }

        return productChange;
    }

    /// <summary>
    /// Update inventory balance
    /// </summary>
    /// <param name="productChanges">List of product changes</param>
    /// <param name="records">Records</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected async Task UpdateInventoryBalanceAsync(List<CreateTrackingRequest.ProductBalanceChange> productChanges, List<ZettleRecord> records)
    {
        if (!productChanges.Any())
            return;

        var inventoryRequest = new CreateTrackingRequest
        {
            ReturnLocationUuid = await GetLocationAsync("STORE"),
            ProductChanges = productChanges,
            ExternalUuid = GuidGenerator.GenerateTimeBasedGuid().ToString()
        };

        //save external UUID to avoid a double change, we will check it when receive a webhook event
        foreach (var record in records)
        {
            record.ExternalUuid = inventoryRequest.ExternalUuid;
        }
        await _zettleRecordService.UpdateRecordsAsync(records);

        //update balances
        await _zettleHttpClient.RequestAsync<CreateTrackingRequest, LocationInventoryBalance>(inventoryRequest);
    }

    /// <summary>
    /// Get location UUID by the passed type
    /// </summary>
    /// <param name="type">Location type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains location UUID
    /// </returns>
    protected async Task<string> GetLocationAsync(string type)
    {
        if (!_locations.TryGetValue(type, out var _))
        {
            var locationList = await _zettleHttpClient.RequestAsync<GetLocationsRequest, LocationList>(new());
            _locations = locationList.ToDictionary(location => location.Type?.ToUpper(), location => location.Uuid);
        }

        return _locations[type];
    }

    #endregion

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Check whether the plugin is configured
    /// </summary>
    /// <param name="settings">Plugin settings</param>
    /// <returns>Result</returns>
    public static bool IsConfigured(ZettleSettings settings)
    {
        //Client ID and API Key are required to request services
        return !string.IsNullOrEmpty(settings?.ClientId) && !string.IsNullOrEmpty(settings?.ApiKey);
    }

    #region Account

    /// <summary>
    /// Get the authenticated user info
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains user details; error message if exists
    /// </returns>
    public async Task<(UserInfo Result, string Error)> GetUserInfoAsync()
    {
        return await HandleFunctionAsync(async () => await _zettleHttpClient.RequestAsync<GetUserInfoRequest, UserInfo>(new()), false);
    }

    /// <summary>
    /// Get the merchant account info
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains account details; error message if exists
    /// </returns>
    public async Task<(AccountInfo Result, string Error)> GetAccountInfoAsync()
    {
        return await HandleFunctionAsync(async () => await _zettleHttpClient.RequestAsync<GetAccountInfoRequest, AccountInfo>(new()), false);
    }

    /// <summary>
    /// Get the default tax rate
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the default tax rate; error message if exists
    /// </returns>
    public async Task<(decimal? Result, string Error)> GetDefaultTaxRateAsync()
    {
        return await HandleFunctionAsync(async () =>
        {
            var taxRates = await _zettleHttpClient.RequestAsync<GetTaxRatesRequest, TaxRateList>(new());
            return taxRates.TaxRates?.FirstOrDefault(rate => rate.IsDefault == true)?.Percentage;
        });
    }

    /// <summary>
    /// Disconnect the app from an associated Zettle organisation
    /// </summary>
    /// <returns>A task that represents the asynchronous operation
    /// The task result contains disconnect result; error message if exists
    /// </returns>
    public async Task<(bool Result, string Error)> DisconnectAsync()
    {
        return await HandleFunctionAsync(async () =>
        {
            await _zettleHttpClient.RequestAsync<DeleteAppRequest, ApiResponse>(new());
            return true;
        }, false);
    }

    #endregion

    #region Webhooks

    /// <summary>
    /// Create webhook that receive events for the subscribed event types
    /// </summary>
    /// <param name="webhookUrl">Webhook URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the webhook; error message if exists
    /// </returns>
    public async Task<(Subscription Result, string Error)> CreateWebhookAsync(string webhookUrl)
    {
        return await HandleFunctionAsync(async () =>
        {
            //check whether the webhook already exists
            var webhooks = await _zettleHttpClient.RequestAsync<GetSubscriptionsRequest, SubscriptionList>(new());
            var existingWebhook = webhooks
                ?.FirstOrDefault(webhook => webhook.Destination?.Equals(webhookUrl, StringComparison.InvariantCultureIgnoreCase) ?? false);
            if (existingWebhook is not null)
                return existingWebhook;

            //or try to create the new one if doesn't
            var (accountInfo, _) = await GetAccountInfoAsync();
            var request = new CreateSubscriptionRequest
            {
                Uuid = GuidGenerator.GenerateTimeBasedGuid().ToString(),
                TransportName = "WEBHOOK",
                EventNames = ZettleDefaults.WebhookEventNames,
                Destination = webhookUrl,
                ContactEmail = accountInfo?.ContactEmail
            };

            return await _zettleHttpClient.RequestAsync<CreateSubscriptionRequest, Subscription>(request);
        });
    }

    /// <summary>
    /// Delete webhook
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task DeleteWebhookAsync()
    {
        await HandleFunctionAsync(async () =>
        {
            var webhooks = await _zettleHttpClient.RequestAsync<GetSubscriptionsRequest, SubscriptionList>(new());
            var existingWebhook = webhooks
                ?.FirstOrDefault(webhook => webhook.Destination?.Equals(_zettleSettings.WebhookUrl, StringComparison.InvariantCultureIgnoreCase) ?? false);
            if (existingWebhook is null)
                return false;

            var request = new DeleteSubscriptionsRequest { Uuid = existingWebhook.Uuid };
            await _zettleHttpClient.RequestAsync<DeleteSubscriptionsRequest, ApiResponse>(request);

            return true;
        }, false);
    }

    /// <summary>
    /// Handle webhook request
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleWebhookAsync(Microsoft.AspNetCore.Http.HttpRequest request)
    {
        await HandleFunctionAsync(async () =>
        {
            using var streamReader = new StreamReader(request.Body);
            var requestContent = await streamReader.ReadToEndAsync();
            if (string.IsNullOrEmpty(requestContent))
                throw new NopException("Webhook request content is empty");

            //get webhook message
            var message = JsonConvert.DeserializeObject<Message>(requestContent);

            //test message is sent during webhook initialization
            if (message.EventName == "TestMessage")
                return true;

            if (string.IsNullOrEmpty(_zettleSettings.WebhookKey))
                throw new NopException("Webhook is not set");

            //ensure that request is signed
            if (!request.Headers.TryGetValue(ZettleDefaults.SignatureHeader, out var signatures))
                throw new NopException("Webhook request not signed by a signature header");

            var messageBytes = Encoding.UTF8.GetBytes($"{message.Timestamp}.{message.Payload}");
            var keyBytes = Encoding.UTF8.GetBytes(_zettleSettings.WebhookKey);
            using var cryptographer = new HMACSHA256(keyBytes);
            var hashBytes = cryptographer.ComputeHash(messageBytes);
            var encryptedString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            if (!signatures.Any(signature => signature.Equals(encryptedString, StringComparison.InvariantCultureIgnoreCase)))
                throw new NopException("Webhook request isn't valid");

            switch (message.EventName)
            {
                case "InventoryBalanceChanged":
                {
                    var balanceInfo = JsonConvert.DeserializeObject<InventoryBalanceUpdate>(message.Payload);

                    for (var i = 0; i < (balanceInfo.BalanceBefore ?? new()).Count; i++)
                    {
                        var balanceBefore = balanceInfo.BalanceBefore?.ElementAtOrDefault(i);
                        var balanceAfter = balanceInfo.BalanceAfter?.ElementAtOrDefault(i);

                        if (string.IsNullOrEmpty(balanceBefore?.ProductUuid) || string.IsNullOrEmpty(balanceAfter?.ProductUuid))
                            continue;

                        if (balanceBefore.ProductUuid != balanceAfter.ProductUuid || balanceBefore.VariantUuid != balanceAfter.VariantUuid)
                            continue;

                        if (!balanceBefore.Balance.HasValue || !balanceAfter.Balance.HasValue)
                            continue;

                        var records = await _zettleRecordService.GetAllRecordsAsync(productUuid: balanceAfter.ProductUuid);
                        var productRecord = records.FirstOrDefault(record => string.Equals(record.VariantUuid, balanceAfter.VariantUuid, StringComparison.InvariantCultureIgnoreCase));
                        if (productRecord is null || !productRecord.Active || !productRecord.InventoryTrackingEnabled)
                            continue;

                        //whether the сhange is initiated by the plugin (inventory balance has already been changed)
                        if (productRecord.ExternalUuid == balanceInfo.ExternalUuid)
                        {
                            //keep external UUID for a day in case of errors when processing webhook requests
                            var balanceChangeDate = balanceInfo.UpdateDetails.Timestamp ?? DateTime.UtcNow;
                            if (balanceChangeDate < DateTime.UtcNow.AddDays(-1))
                            {
                                productRecord.ExternalUuid = null;
                                await _zettleRecordService.UpdateRecordAsync(productRecord);
                            }
                            continue;
                        }

                        //adjust inventory
                        var product = await _productService.GetProductByIdAsync(productRecord.ProductId);
                        var combination = await _productAttributeService.GetProductAttributeCombinationByIdAsync(productRecord.CombinationId);
                        var quantityToChange = balanceAfter.Balance.Value - balanceBefore.Balance.Value;
                        var logMessage = $"{ZettleDefaults.SystemName} update. Inventory balance changed at {balanceAfter.Created?.ToLongTimeString()}";
                        await _productService.AdjustInventoryAsync(product, quantityToChange, combination?.AttributesXml, logMessage);
                    }

                    break;
                }
                case "InventoryTrackingStopped":
                {
                    var inventoryTrackingInfo = JsonConvert.DeserializeAnonymousType(message.Payload, new { ProductUuid = string.Empty });
                    if (string.IsNullOrEmpty(inventoryTrackingInfo.ProductUuid))
                        break;

                    //stop tracking
                    var records = (await _zettleRecordService.GetAllRecordsAsync(productUuid: inventoryTrackingInfo.ProductUuid)).ToList();
                    foreach (var record in records)
                    {
                        record.InventoryTrackingEnabled = false;
                        record.UpdatedOnUtc = DateTime.UtcNow;
                    }
                    await _zettleRecordService.UpdateRecordsAsync(records);

                    break;
                }

                case "ProductCreated":
                {
                    //use this event only to start inventory tracking for product
                    var productInfo = JsonConvert.DeserializeObject<Product>(message.Payload);
                    var records = await _zettleRecordService.GetAllRecordsAsync(productUuid: productInfo.Uuid);
                    var productRecord = records.FirstOrDefault(record => record.CombinationId == 0);
                    if (productRecord is null || !productRecord.Active || !productRecord.InventoryTrackingEnabled)
                        break;

                    var storeBalance = await _zettleHttpClient
                        .RequestAsync<GetLocationInventoryBalanceRequest, LocationInventoryBalance>(new());
                    var trackingStarted = storeBalance.TrackedProducts
                        ?.Contains(productRecord.Uuid, StringComparer.InvariantCultureIgnoreCase);
                    if (trackingStarted ?? true)
                        break;

                    var combinationRecords = records.Where(record => record.CombinationId != 0).ToList();
                    var combinationRecordsToStart = new List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)>();
                    foreach (var combinationRecord in combinationRecords)
                    {
                        combinationRecordsToStart.Add((combinationRecord, 0, null));
                    }
                    (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) productRecordToStart = (productRecord, 0, null);
                    var productChange = await PrepareInventoryBalanceChangeAsync(InventoryBalanceChangeType.StartTracking,
                        productRecordToStart, combinationRecordsToStart);
                    if (productChange is null)
                        break;

                    await UpdateInventoryBalanceAsync([productChange], combinationRecords.Union([productRecord]).ToList());

                    break;
                }

                case "ApplicationConnectionRemoved":
                {
                    var applicationInfo = JsonConvert.DeserializeAnonymousType(message.Payload, new { Type = string.Empty });
                    if (string.IsNullOrEmpty(applicationInfo.Type))
                        break;

                    var warning = applicationInfo.Type;
                    if (applicationInfo.Type.Equals("ApplicationConnectionRemoved", StringComparison.InvariantCultureIgnoreCase) ||
                        applicationInfo.Type.Equals("PersonalAssertionDeleted", StringComparison.InvariantCultureIgnoreCase))
                    {
                        warning = "The application was disconnected from PayPal Zettle organization. You need to reconfigure the plugin.";

                        _zettleSettings.ClientId = string.Empty;
                        _zettleSettings.ApiKey = string.Empty;
                        _zettleSettings.WebhookUrl = string.Empty;
                        _zettleSettings.WebhookKey = string.Empty;
                        _zettleSettings.ImportId = string.Empty;
                        await _settingService.SaveSettingAsync(_zettleSettings);
                    }
                    await _logger.WarningAsync($"{ZettleDefaults.SystemName}. {warning}");

                    break;
                }

                default:
                    throw new NopException($"Unknown webhook resource type '{message.EventName}'");
            }

            return true;
        });
    }

    #endregion

    #region Sync

    /// <summary>
    /// Get last import details
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the import details; error message if exists
    /// </returns>
    public async Task<(Import Result, string Error)> GetImportAsync()
    {
        return await HandleFunctionAsync(async () =>
        {
            return await _zettleHttpClient.RequestAsync<GetImportRequest, Import>(new() { ImportUuid = _zettleSettings.ImportId });
        }, false);
    }

    /// <summary>
    /// Start products import
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the import details; error message if exists
    /// </returns>
    public async Task<(Import Result, string Error)> ImportAsync()
    {
        return await HandleFunctionAsync(async () =>
        {
            var log = new StringBuilder($"{ZettleDefaults.SystemName} information.{Environment.NewLine}");
            log.AppendLine($"Synchronization started at {DateTime.UtcNow.ToLongTimeString()} UTC");

            await ImportDiscountsAsync(log);

            await ImportDeletedAsync(log);

            await ImportImageChangedAsync(log);

            await ImportInventoryTrackingAsync(log);

            var import = await ImportCreatedOrUpdatedAsync(log);

            if (!string.IsNullOrEmpty(import?.Uuid))
            {
                //save import id for future use
                await _settingService.SetSettingAsync($"{nameof(ZettleSettings)}.{nameof(ZettleSettings.ImportId)}", import?.Uuid);

                //refresh records
                var records = await _zettleRecordService.GetAllRecordsAsync(active: true,
                    operationTypes: [OperationType.Create, OperationType.Update, OperationType.ImageChanged]);
                foreach (var record in records)
                {
                    record.OperationType = OperationType.None;
                    record.UpdatedOnUtc = DateTime.UtcNow;
                }
                await _zettleRecordService.UpdateRecordsAsync(records.ToList());
            }

            log.AppendLine($"Synchronization finished at {DateTime.UtcNow.ToLongTimeString()} UTC");

            if (_zettleSettings.LogSyncMessages)
                await _logger.InformationAsync(log.ToString());

            return import;
        });
    }

    #endregion

    #region Inventory

    /// <summary>
    /// Change inventory balance
    /// </summary>
    /// <param name="productId">Product identifier</param>
    /// <param name="combinationId">Combination identifier</param>
    /// <param name="quantityAdjustment">Stock quantity adjustment</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task ChangeInventoryBalanceAsync(int productId, int combinationId, int quantityAdjustment)
    {
        var records = (await _zettleRecordService.GetAllRecordsAsync(active: true))
            .Where(record => record.ProductId == productId && record.InventoryTrackingEnabled && !string.IsNullOrEmpty(record.Uuid))
            .ToList();
        if (!records.Any())
            return;

        var productRecord = records.FirstOrDefault(record => record.CombinationId == 0);
        var combinationRecords = combinationId > 0
            ? records.Where(record => record.CombinationId == combinationId && record.InventoryTrackingEnabled && !string.IsNullOrEmpty(record.VariantUuid)).ToList()
            : new List<ZettleRecord>();

        //we cannot know the exact reason of the change, so we will use Purchase for negative adjustments and Re-stock for positive ones
        var changeType = quantityAdjustment < 0 ? InventoryBalanceChangeType.Purchase : InventoryBalanceChangeType.Restock;
        var combinationRecordsToUpdate = new List<(ZettleRecord Record, int StockQuantity, int? QuantityAdjustment)>();
        foreach (var combinationRecord in combinationRecords)
        {
            combinationRecordsToUpdate.Add((combinationRecord, 0, Math.Abs(quantityAdjustment)));
        }
        (ZettleRecord Record, int StockQuantity, int? QuantityAdjustment) productRecordToUpdate = (productRecord, 0, Math.Abs(quantityAdjustment));
        var productChange = await PrepareInventoryBalanceChangeAsync(changeType, productRecordToUpdate, combinationRecordsToUpdate);
        if (productChange is null)
            return;

        await UpdateInventoryBalanceAsync([productChange], combinationRecords.Union([productRecord]).ToList());
    }

    #endregion

    #endregion
}