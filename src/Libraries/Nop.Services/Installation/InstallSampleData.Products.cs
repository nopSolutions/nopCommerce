using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Installation.SampleData;
using Nop.Services.Media;

namespace Nop.Services.Installation;

public partial class InstallationService
{
    #region Fields

    protected Dictionary<string, ProductTag> _tags = new(comparer: StringComparer.InvariantCultureIgnoreCase);

    #endregion

    #region Utilities

    /// <summary>
    /// Inserts the product picture
    /// </summary>
    /// <param name="product">Product to insert picture</param>
    /// <param name="fileName">Picture file name</param>
    /// <param name="displayOrder">Display order</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the identifier of inserted picture
    /// </returns>
    protected virtual async Task<int> InsertProductPictureAsync(Product product, string fileName, int displayOrder = 1)
    {
        var pictureId = await InsertPictureAsync(fileName, product.Name);

        await _dataProvider.InsertEntityAsync(
            new ProductPicture
            {
                ProductId = product.Id,
                PictureId = pictureId,
                DisplayOrder = displayOrder
            });

        return pictureId;
    }

    /// <summary>
    /// Gets a specification attribute option identifier
    /// </summary>
    /// <param name="specAttributeName">The spec attribute name</param>
    /// <param name="specAttributeOptionName">The spec attribute option name</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<int> GetSpecificationAttributeOptionIdAsync(string specAttributeName, string specAttributeOptionName)
    {
        var specificationAttribute = await Table<SpecificationAttribute>()
            .SingleAsync(sa => sa.Name == specAttributeName);

        var specificationAttributeOption = await Table<SpecificationAttributeOption>()
            .SingleAsync(sao => sao.Name == specAttributeOptionName && sao.SpecificationAttributeId == specificationAttribute.Id);

        return specificationAttributeOption.Id;
    }

    /// <summary>
    /// Insert product tags mappings
    /// </summary>
    /// <param name="product"></param>
    /// <param name="tags"></param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InsertProductTagMappingAsync(Product product, string[] tags)
    {
        if (!_tags.Any())
            _tags = Table<ProductTag>().AsEnumerable().GroupBy(p => p.Name, p => p)
                .ToDictionary(p => p.Key, p => p.FirstOrDefault());

        var newProductTags = await tags.Distinct().Where(tag => !_tags.ContainsKey(tag))
            .Select(item => new ProductTag { Name = item }).ToListAsync();

        if (newProductTags.Any())
        {
            await _dataProvider.BulkInsertEntitiesAsync(newProductTags);
            await InsertSearchEngineNamesAsync(newProductTags, productTag => productTag.Name);

            foreach (var productTag in newProductTags)
                _tags.Add(productTag.Name, productTag);
        }

        await _dataProvider.BulkInsertEntitiesAsync(tags.Select(tag =>
            new ProductProductTagMapping { ProductTagId = _tags[tag].Id, ProductId = product.Id }));
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task InstallProductsAsync(SampleProducts jsonData)
    {
        //products
        var allProducts = new List<Product>();

        var productTemplates = new Dictionary<string, int>();
        var taxCategories = new Dictionary<string, int>();
        var categories = new Dictionary<string, int>();
        var manufacturers = new Dictionary<string, int>();
        var productAttributes = new Dictionary<string, int>();
        var productAvailabilityRanges = new Dictionary<string, int>();
        var deliveryDates = new Dictionary<string, int>();
        var products = new Dictionary<string, int>();

        async Task<int> getAndSaveId(Dictionary<string, int> dict, string key, Func<string, Task<int>> foo)
        {
            if (string.IsNullOrEmpty(key))
                return 0;

            if (dict.TryGetValue(key, out var id))
                return id;

            id = await foo(key);
            dict[key] = id;

            return id;
        }

        async Task<int> getProductTemplate(string templateName)
        {
            return await getAndSaveId(productTemplates, templateName,
                async tName => await GetFirstEntityIdAsync<ProductTemplate>(pt => pt.Name == tName) ??
                    throw new Exception($"\"{tName}\" template could not be loaded"));
        }

        async Task<int> getTaxCategoryId(string taxCategoryName)
        {
            return await getAndSaveId(taxCategories, taxCategoryName, async tcName => await GetFirstEntityIdAsync<TaxCategory>(tc => tc.Name == tcName) ??
                throw new Exception($"\"{tcName}\" tax category could not be loaded"));
        }

        async Task<int> getCategoryId(string categoryName)
        {
            return await getAndSaveId(categories, categoryName, async cName => await GetFirstEntityIdAsync<Category>(c => c.Name == cName) ??
                throw new Exception($"\"{cName}\" category could not be loaded"));
        }

        async Task<int> getManufacturerId(string manufacturerName)
        {
            return await getAndSaveId(manufacturers, manufacturerName, async mName => await GetFirstEntityIdAsync<Manufacturer>(m => m.Name == mName) ??
                throw new Exception($"\"{mName}\" manufacturer could not be loaded"));
        }

        async Task<int> getProductAttributeId(string productAttributeName)
        {
            return await getAndSaveId(productAttributes, productAttributeName, async paName => await GetFirstEntityIdAsync<ProductAttribute>(pa => pa.Name == paName) ??
                throw new Exception($"\"{paName}\" product attribute could not be loaded"));
        }

        async Task<int> getProductAvailabilityRangeId(string productAvailabilityRangeName)
        {
            return await getAndSaveId(productAvailabilityRanges, productAvailabilityRangeName, async parName => await GetFirstEntityIdAsync<ProductAvailabilityRange>(par => par.Name == parName) ??
                throw new Exception($"\"{parName}\" product availability range could not be loaded"));
        }

        async Task<int> getDeliveryDateId(string deliveryDateName)
        {
            return await getAndSaveId(deliveryDates, deliveryDateName, async ddName => await GetFirstEntityIdAsync<DeliveryDate>(dd => dd.Name == ddName) ??
                throw new Exception($"\"{ddName}\" delivery date could not be loaded"));
        }

        async Task<int> getProductId(string productSku)
        {
            return await getAndSaveId(products, productSku, async sku => await GetFirstEntityIdAsync<Product>(p => p.Sku == sku) ??
                throw new Exception($"Product with SKU = \"{sku}\" could not be loaded"));
        }

        //TODO: avoid using service
        var downloadService = EngineContext.Current.Resolve<IDownloadService>();

        var sampleDownloadsPath = _fileProvider.GetAbsolutePath(NopInstallationDefaults.SampleImagesPath);

        async Task insertProduct(SampleProducts.SampleProduct sample, int parentGroupedProductId = 0)
        {
            var product = new Product
            {
                ProductType = sample.ProductType,
                VisibleIndividually = sample.VisibleIndividually,
                ParentGroupedProductId = parentGroupedProductId,
                Name = sample.Name,
                Sku = sample.Sku,
                ShortDescription = sample.ShortDescription,
                FullDescription = sample.FullDescription,
                ProductTemplateId = await getProductTemplate(sample.ProductTemplateName),
                AllowCustomerReviews = sample.AllowCustomerReviews,
                Price = sample.Price,
                OldPrice = sample.OldPrice,
                IsShipEnabled = sample.IsShipEnabled,
                IsFreeShipping = sample.IsFreeShipping,
                Weight = sample.Weight,
                Length = sample.Length,
                Width = sample.Width,
                Height = sample.Height,
                TaxCategoryId = await getTaxCategoryId(sample.TaxCategoryName),
                ManageInventoryMethod = sample.ManageInventoryMethod,
                StockQuantity = sample.StockQuantity,
                NotifyAdminForQuantityBelow = sample.NotifyAdminForQuantityBelow,
                AllowBackInStockSubscriptions = sample.AllowBackInStockSubscriptions,
                DisplayStockAvailability = sample.DisplayStockAvailability,
                LowStockActivity = sample.LowStockActivity,
                BackorderMode = sample.BackorderMode,
                OrderMinimumQuantity = sample.OrderMinimumQuantity,
                OrderMaximumQuantity = sample.OrderMaximumQuantity,
                Published = sample.Published,
                ShowOnHomepage = sample.ShowOnHomepage,
                MarkAsNew = sample.MarkAsNew,
                IsRecurring = sample.IsRecurring,
                RecurringCycleLength = sample.RecurringCycleLength,
                RecurringCyclePeriod = sample.RecurringCyclePeriod,
                RecurringTotalCycles = sample.RecurringTotalCycles,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                IsRental = sample.IsRental,
                RentalPriceLength = sample.RentalPriceLength,
                RentalPricePeriod = sample.RentalPricePeriod,
                IsGiftCard = sample.IsGiftCard,
                GiftCardType = sample.GiftCardType,
                IsDownload = sample.IsDownload,
                DownloadActivationType = sample.DownloadActivationType,
                UnlimitedDownloads = sample.UnlimitedDownloads,
                HasUserAgreement = sample.HasUserAgreement,
                CustomerEntersPrice = sample.CustomerEntersPrice,
                MinimumCustomerEnteredPrice = sample.MinimumCustomerEnteredPrice,
                MaximumCustomerEnteredPrice = sample.MaximumCustomerEnteredPrice,
            };

            if (!string.IsNullOrEmpty(sample.ProductAvailabilityRange))
                product.ProductAvailabilityRangeId = await getProductAvailabilityRangeId(sample.ProductAvailabilityRange);

            if (!string.IsNullOrEmpty(sample.DeliveryDate))
                product.DeliveryDateId = await getDeliveryDateId(sample.DeliveryDate);

            if (sample.Download != null)
            {
                var download = new Download
                {
                    DownloadGuid = Guid.NewGuid(),
                    ContentType = sample.Download.ContentType,
                    DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + sample.Download.DownloadFileName),
                    Extension = sample.Download.Extension,
                    Filename = sample.Download.Filename,
                    IsNew = sample.Download.IsNew
                };
                await downloadService.InsertDownloadAsync(download);

                product.DownloadId = download.Id;
            }

            if (sample.SampleDownload != null)
            {
                var download = new Download
                {
                    DownloadGuid = Guid.NewGuid(),
                    ContentType = sample.SampleDownload.ContentType,
                    DownloadBinary = await _fileProvider.ReadAllBytesAsync(sampleDownloadsPath + sample.SampleDownload.DownloadFileName),
                    Extension = sample.SampleDownload.Extension,
                    Filename = sample.SampleDownload.Filename,
                    IsNew = sample.SampleDownload.IsNew
                };
                await downloadService.InsertDownloadAsync(download);

                product.HasSampleDownload = true;
                product.DownloadId = download.Id;
            }

            allProducts.Add(product);

            await _dataProvider.InsertEntityAsync(product);

            if (!string.IsNullOrEmpty(sample.CategoryName))
                await _dataProvider.InsertEntityAsync(new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = await getCategoryId(sample.CategoryName),
                    DisplayOrder = 1
                });

            if (!string.IsNullOrEmpty(sample.ManufacturerName))
                await _dataProvider.InsertEntityAsync(new ProductManufacturer
                {
                    ProductId = product.Id,
                    ManufacturerId = await getManufacturerId(sample.ManufacturerName),
                    DisplayOrder = 1
                });

            if (sample.ProductPictures.Any())
            {
                var productPictures = new List<ProductPicture>();

                foreach (var pictureName in sample.ProductPictures)
                    productPictures.Add(new()
                    {
                        ProductId = product.Id,
                        PictureId = await InsertPictureAsync(pictureName, product.Name),
                        DisplayOrder = 1
                    });

                await _dataProvider.BulkInsertEntitiesAsync(productPictures);
            }

            foreach (var productAttributeMapping in sample.ProductAttributeMapping)
            {
                var attributeMapping = await _dataProvider.InsertEntityAsync(new ProductAttributeMapping
                {
                    ProductId = product.Id,
                    ProductAttributeId = await getProductAttributeId(productAttributeMapping.ProductAttributeName),
                    AttributeControlType = productAttributeMapping.AttributeControlType,
                    TextPrompt = productAttributeMapping.TextPrompt,
                    IsRequired = productAttributeMapping.IsRequired
                });

                var productAttributeValues = new List<ProductAttributeValue>();

                foreach (var sampleProductAttributeValue in productAttributeMapping.AttributeValues)
                {
                    var productAttributeValue = new ProductAttributeValue
                    {
                        ProductAttributeMappingId = attributeMapping.Id,
                        AttributeValueType = sampleProductAttributeValue.AttributeValueType,
                        Name = sampleProductAttributeValue.Name,
                        DisplayOrder = sampleProductAttributeValue.DisplayOrder,
                        PriceAdjustment = sampleProductAttributeValue.PriceAdjustment,
                        IsPreSelected = sampleProductAttributeValue.IsPreSelected,
                        ColorSquaresRgb = sampleProductAttributeValue.ColorSquaresRgb
                    };

                    if (!string.IsNullOrEmpty(sampleProductAttributeValue.ImageSquaresPictureName))
                        productAttributeValue.ImageSquaresPictureId = await InsertPictureAsync(sampleProductAttributeValue.ImageSquaresPictureName, productAttributeValue.Name);

                    if (sampleProductAttributeValue.AttributeValuePictures.Any())
                    {
                        await _dataProvider.InsertEntityAsync(productAttributeValue);

                        await _dataProvider.BulkInsertEntitiesAsync(await sampleProductAttributeValue.AttributeValuePictures
                            .SelectAwait(async p =>
                                new ProductAttributeValuePicture
                                {
                                    PictureId = await InsertProductPictureAsync(product, p),
                                    ProductAttributeValueId = productAttributeValue.Id
                                }).ToListAsync());
                    }
                    else
                        productAttributeValues.Add(productAttributeValue);
                }

                await _dataProvider.BulkInsertEntitiesAsync(productAttributeValues);

                if (productAttributeValues.Any(p => p.ImageSquaresPictureId != 0))
                    await _dataProvider.BulkInsertEntitiesAsync(productAttributeValues.Where(p => p.ImageSquaresPictureId != 0).Select(v =>
                        new ProductAttributeValuePicture
                        {
                            PictureId = v.ImageSquaresPictureId,
                            ProductAttributeValueId = v.Id
                        }));
            }

            if (sample.ProductTags.Any())
                await InsertProductTagMappingAsync(product, sample.ProductTags.ToArray());

            if (sample.ProductSpecificationAttribute.Any())
            {
                var productSpecificationAttributes = new List<ProductSpecificationAttribute>();

                foreach (var specificationAttribute in sample.ProductSpecificationAttribute)
                    productSpecificationAttributes.Add(new ProductSpecificationAttribute
                    {
                        ProductId = product.Id,
                        AllowFiltering = specificationAttribute.AllowFiltering,
                        ShowOnProductPage = specificationAttribute.ShowOnProductPage,
                        DisplayOrder = specificationAttribute.DisplayOrder,
                        SpecificationAttributeOptionId = await GetSpecificationAttributeOptionIdAsync(specificationAttribute.SpecAttributeName, specificationAttribute.SpecAttributeOptionName)
                    });

                await _dataProvider.BulkInsertEntitiesAsync(productSpecificationAttributes);
            }

            foreach (var sampleGroupedProduct in sample.GroupedProducts)
                await insertProduct(sampleGroupedProduct, product.Id);

            if (sample.TierPrices.Any())
                await _dataProvider.BulkInsertEntitiesAsync(sample.TierPrices.Select(tp => new TierPrice
                {
                    Quantity = tp.Quantity,
                    Price = tp.Price,
                    ProductId = product.Id
                }));
        }

        foreach (var sample in jsonData.Products)
            await insertProduct(sample);

        //search engine names
        await InsertSearchEngineNamesAsync(allProducts, product => product.Name);

        //related products
        if (jsonData.RelatedProducts.Any())
            await _dataProvider.BulkInsertEntitiesAsync(await jsonData.RelatedProducts.SelectAwait(async rp =>
                new RelatedProduct
                {
                    ProductId1 = await getProductId(rp.FirstProductSku),
                    ProductId2 = await getProductId(rp.SecondProductSku)
                }).ToListAsync());

        //reviews
        using (var random = new SecureRandomNumberGenerator())
            foreach (var product in allProducts)
            {
                if (product.ProductType != ProductType.SimpleProduct)
                    continue;

                //only 3 of 4 products will have reviews
                if (random.Next(4) == 3)
                    continue;

                //rating from 4 to 5
                var rating = random.Next(4, 6);

                await _dataProvider.InsertEntityAsync(new ProductReview
                {
                    CustomerId = await GetDefaultCustomerIdAsync(),
                    ProductId = product.Id,
                    StoreId = await GetDefaultStoreIdAsync(),
                    IsApproved = true,
                    Title = "Some sample review",
                    ReviewText = $"This sample review is for the {product.Name}. I've been waiting for this product to be available. It is priced just right.",
                    //random (4 or 5)
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    CreatedOnUtc = DateTime.UtcNow
                });

                product.ApprovedRatingSum = rating;
                product.ApprovedTotalReviews = 1;
            }

        await _dataProvider.UpdateEntitiesAsync(allProducts);

        //stock quantity history
        foreach (var product in allProducts.Where(product => product.StockQuantity > 0))
            await _dataProvider.InsertEntityAsync(new StockQuantityHistory
            {
                ProductId = product.Id,
                WarehouseId = product.WarehouseId > 0 ? product.WarehouseId : null,
                QuantityAdjustment = product.StockQuantity,
                StockQuantity = product.StockQuantity,
                Message = "The stock quantity has been edited",
                CreatedOnUtc = DateTime.UtcNow
            });
    }

    #endregion
}
