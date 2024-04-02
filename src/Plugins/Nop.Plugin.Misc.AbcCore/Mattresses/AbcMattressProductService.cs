using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Seo;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Plugin.Misc.AbcCore.Mattresses.Domain;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Core;

namespace Nop.Plugin.Misc.AbcCore.Mattresses
{
    public class AbcMattressProductService : IAbcMattressProductService
    {
        private readonly IAbcMattressModelService _abcMattressService;
        private readonly IAbcMattressBaseService _abcMattressBaseService;
        private readonly IAbcMattressEntryService _abcMattressEntryService;
        private readonly IAbcMattressFrameService _abcMattressFrameService;
        private readonly IAbcMattressGiftService _abcMattressGiftService;
        private readonly IAbcMattressPackageService _abcMattressPackageService;
        private readonly IAbcMattressProtectorService _abcMattressProtectorService;
        private readonly ICategoryService _categoryService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IProductAbcFinanceService _productAbcFinanceService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILogger _logger;
        private readonly INopDataProvider _nopDataProvider;
        private readonly ISpecificationAttributeService _specificationAttributeService;

        public AbcMattressProductService(
            IAbcMattressModelService abcMattressService,
            IAbcMattressBaseService abcMattressBaseService,
            IAbcMattressEntryService abcMattressEntryService,
            IAbcMattressFrameService abcMattressFrameService,
            IAbcMattressGiftService abcMattressGiftService,
            IAbcMattressPackageService abcMattressPackageService,
            IAbcMattressProtectorService abcMattressProtectorService,
            ICategoryService categoryService,
            IGenericAttributeService genericAttributeService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IProductAbcFinanceService productAbcFinanceService,
            IProductAttributeService productAttributeService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            ITaxCategoryService taxCategoryService,
            IUrlRecordService urlRecordService,
            ILogger logger,
            INopDataProvider nopDataProvider,
            ISpecificationAttributeService specificationAttributeService
        )
        {
            _abcMattressService = abcMattressService;
            _abcMattressBaseService = abcMattressBaseService;
            _abcMattressEntryService = abcMattressEntryService;
            _abcMattressFrameService = abcMattressFrameService;
            _abcMattressGiftService = abcMattressGiftService;
            _abcMattressPackageService = abcMattressPackageService;
            _abcMattressProtectorService = abcMattressProtectorService;
            _categoryService = categoryService;
            _genericAttributeService = genericAttributeService;
            _manufacturerService = manufacturerService;
            _productService = productService;
            _productAbcFinanceService = productAbcFinanceService;
            _productAttributeService = productAttributeService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _taxCategoryService = taxCategoryService;
            _urlRecordService = urlRecordService;
            _logger = logger;
            _nopDataProvider = nopDataProvider;
            _specificationAttributeService = specificationAttributeService;
        }

        public async Task SetSpecificationAttributesAsync(AbcMattressModel model, Product product)
        {
            var comfortSpecAttr = (await _specificationAttributeService.GetSpecificationAttributesAsync())
                                                            .Where(sa => sa.Name == "Comfort")
                                                            .FirstOrDefault();
            if (comfortSpecAttr == null)
            {
                throw new NopException("Unable to find 'Comfort' specification attribute.");
            }

            var options = await _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttributeAsync(comfortSpecAttr.Id);
            var option = options.Where(so => so.Name == model.Comfort)
                                .FirstOrDefault();
            if (option == null)
            {
                throw new NopException($"Unable to find '{model.Comfort}' " +
                "specification attribute option for the 'Comfort' " +
                "specification attribute.");
            }

            var productSpecificationAttributes = await _specificationAttributeService.GetProductSpecificationAttributesAsync(product.Id);
            var comfortProductSpecAttr = productSpecificationAttributes.Where(psa => psa.SpecificationAttributeOptionId == option.Id)
                                                                       .FirstOrDefault();
            // Found the existing product spec. attribute - just skip processing
            if (comfortProductSpecAttr != null) { return; }

            // delete any currently existing Comfort specs
            var optionIds = options.Select(o => o.Id);
            var existingComfortProductSpecAttributes = productSpecificationAttributes.Where(psa => optionIds.Contains(psa.SpecificationAttributeOptionId));
            foreach (var psa in existingComfortProductSpecAttributes)
            {
                await _specificationAttributeService.DeleteProductSpecificationAttributeAsync(psa);
            }

            // Add new product spec attribute
            var productSpecAttr = new ProductSpecificationAttribute()
            {
                ProductId = product.Id,
                AttributeType = SpecificationAttributeType.Option,
                SpecificationAttributeOptionId = option.Id,
                AllowFiltering = true
            };
            await _specificationAttributeService.InsertProductSpecificationAttributeAsync(
                productSpecAttr
            );
        }

        public List<string> GetMattressItemNos()
        {
            var entryItemNos = _abcMattressEntryService.GetAllAbcMattressEntries().Select(e => e.ItemNo);
            var packageItemNos = _abcMattressPackageService.GetAllAbcMattressPackages().Select(p => p.ItemNo);

            return entryItemNos.Union(packageItemNos).ToList();
        }

        public async Task<Product> UpsertAbcMattressProductAsync(AbcMattressModel abcMattressModel)
        {
            var entries = _abcMattressEntryService.GetAbcMattressEntriesByModelId(
                abcMattressModel.Id
            );

            var hasExistingProduct = abcMattressModel.ProductId != null;
            Product product = hasExistingProduct ?
                await _productService.GetProductByIdAsync(abcMattressModel.ProductId.Value) :
                new Product();

            product.Name = await GetProductNameAsync(abcMattressModel);
            product.Sku = $"M{abcMattressModel.Name}";
            product.AllowCustomerReviews = false;
            product.Published = entries.Any();
            product.CreatedOnUtc = DateTime.UtcNow;
            product.UpdatedOnUtc = DateTime.UtcNow;
            product.VisibleIndividually = true;
            product.ProductType = ProductType.SimpleProduct;
            product.OrderMinimumQuantity = 1;
            product.OrderMaximumQuantity = 10000;
            product.IsShipEnabled = true;

            var prices = await CalculatePriceAsync(abcMattressModel, entries);
            product.OldPrice = prices.OldPrice;
            product.Price = prices.Price;
            product.TaxCategoryId = (await _taxCategoryService.GetAllTaxCategoriesAsync())
                                                       .Where(tc => tc.Name == "Everything")
                                                       .Select(tc => tc.Id)
                                                       .FirstOrDefault();

            await MapProductToStoreAsync(product);

            if (hasExistingProduct)
            {
                await _productService.UpdateProductAsync(product);
            }
            else
            {
                await _productService.InsertProductAsync(product);
            }


            await _urlRecordService.SaveSlugAsync(product, await _urlRecordService.ValidateSeNameAsync(
                product,
                string.Empty,
                product.Name,
                false),
                0
            );

            if (!hasExistingProduct)
            {
                abcMattressModel.ProductId = product.Id;
                await _abcMattressService.UpdateAbcMattressModelAsync(abcMattressModel);
            }
            if (!string.IsNullOrWhiteSpace(abcMattressModel.Sku))
            {
                await _genericAttributeService.SaveAttributeAsync<string>(
                    product,
                    "MattressSku",
                    abcMattressModel.Sku
                );
                await _genericAttributeService.SaveAttributeAsync<string>(
                    product,
                    "PowerReviewsSku",
                    abcMattressModel.Sku
                );
            }

            // add information relating to Synchrony payments
            await SyncSynchronyPaymentsDataAsync(product, abcMattressModel);


            var plpDescription = await _genericAttributeService.GetAttributeAsync<string>(
                product,
                "PLPDescription"
            );
            // Add description for PowerReviews
            await _genericAttributeService.SaveAttributeAsync<string>(
                product,
                "PowerReviewsDescription",
                plpDescription
            );

            return product;
        }

        private async Task SyncSynchronyPaymentsDataAsync(Product product, AbcMattressModel model)
        {
            var entries = _abcMattressEntryService.GetAbcMattressEntriesByModelId(model.Id);
            var packages = _abcMattressPackageService.GetAbcMattressPackagesByEntryIds(entries.Select(e => e.Id));

            var itemNos = entries.Select(e => e.ItemNo).Union(packages.Select(p => p.ItemNo));

            int? months = null;
            bool? isMinimumPayment = null;
            DateTime? startDate = null;
            DateTime? endDate = null;

            foreach (var itemNo in itemNos)
            {
                var productAbcFinance = await _productAbcFinanceService.GetProductAbcFinanceByAbcItemNumberAsync(itemNo);

                if (productAbcFinance == null) { continue; }

                months = productAbcFinance.Months;
                isMinimumPayment = productAbcFinance.IsDeferredPricing;
                startDate = productAbcFinance.StartDate.Value;
                endDate = productAbcFinance.EndDate.Value;
            }

            await _genericAttributeService.SaveAttributeAsync<int?>(
                product,
                "SynchronyPaymentMonths",
                months
            );
            await _genericAttributeService.SaveAttributeAsync<bool?>(
                product,
                "SynchronyPaymentIsMinimum",
                isMinimumPayment
            );
            await _genericAttributeService.SaveAttributeAsync<DateTime?>(
                product,
                "SynchronyPaymentOfferValidFrom",
                startDate
            );
            await _genericAttributeService.SaveAttributeAsync<DateTime?>(
                product,
                "SynchronyPaymentOfferValidTo",
                endDate
            );
        }

        private async Task MapProductToStoreAsync(Product product)
        {
            var store = (await _storeService.GetAllStoresAsync())
                                                   .FirstOrDefault();
            if (store == null)
            {
                throw new Exception("Unable to find store.");
            }

            await _productService.UpdateProductStoreMappingsAsync(
                product,
                new int[] { store.Id }
            );
        }

        private async Task<(decimal OldPrice, decimal Price)> CalculatePriceAsync(AbcMattressModel model, IList<AbcMattressEntry> entries)
        {
            if (!entries.Any()) return (0M, 0M);

            var entry = entries.Where(e => e.Size.ToLower() == "queen")
                                       .FirstOrDefault();
            if (entry == null)
            {
                await _logger.WarningAsync(
                    $"Mattress model {model.Name} has no queen, using mid-priced item");

                entry = entries.OrderBy(e => e.Price)
                               .Skip(entries.Count / 2)
                               .First();
            }

            return (entry.OldPrice, entry.Price);
        }

        public async Task SetProductAttributesAsync(AbcMattressModel model, Product product)
        {
            var nonBaseProductAttributes = (await _productAttributeService.GetAllProductAttributesAsync())
                                                            .Where(pa => pa.Name == "Home Delivery" ||
                                                                         pa.Name == AbcMattressesConsts.MattressSizeName ||
                                                                         pa.Name == AbcMattressesConsts.FreeGiftName);

            foreach (var pa in nonBaseProductAttributes)
            {
                switch (pa.Name)
                {
                    case "Home Delivery":
                        await MergeHomeDeliveryAsync(pa, product);
                        break;
                    case AbcMattressesConsts.MattressSizeName:
                        await MergeSizesAsync(model, pa, product);
                        break;
                    case AbcMattressesConsts.FreeGiftName:
                        await MergeGiftsAsync(model, pa, product);
                        break;
                }
            }

            await SetBasesAsync(model, product);
            await SetMattressProtectorsAsync(model, product);
            await SetFramesAsync(model, product);
        }

        private async Task SetFramesAsync(AbcMattressModel model, Product product)
        {
            var frameAttributes = (await _productAttributeService.GetAllProductAttributesAsync())
                                                                        .Where(pa => AbcMattressesConsts.IsFrame(pa.Name));
            foreach (var pa in frameAttributes)
            {
                await MergeFramesAsync(model, pa, product);
            }
        }

        private async Task SetMattressProtectorsAsync(AbcMattressModel model, Product product)
        {
            var mattressProtectorAttributes = (await _productAttributeService.GetAllProductAttributesAsync())
                                                                        .Where(pa => AbcMattressesConsts.IsMattressProtector(pa.Name));
            foreach (var pa in mattressProtectorAttributes)
            {
                await MergeMattressProtectorsAsync(model, pa, product);
            }
        }

        private async Task SetBasesAsync(AbcMattressModel model, Product product)
        {
            var baseProductAttributes = (await _productAttributeService.GetAllProductAttributesAsync())
                                                                        .Where(pa => AbcMattressesConsts.IsBase(pa.Name));
            foreach (var pa in baseProductAttributes)
            {
                await MergeBasesAsync(model, pa, product);
            }
        }

        private async Task MergeFramesAsync(AbcMattressModel model, ProductAttribute pa, Product product)
        {
            var pam = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                                                          .Where(pam => pam.ProductAttributeId == pa.Id)
                                                          .FirstOrDefault();
            var abcMattressEntry = _abcMattressEntryService.GetAbcMattressEntriesByModelId(model.Id)
                                                           .Where(ame => pa.Name == $"Frame ({ame.Size})")
                                                           .FirstOrDefault();
            if (abcMattressEntry == null) { return; }

            var frames = _abcMattressFrameService.GetAbcMattressFramesBySize(abcMattressEntry.Size);

            if (pam == null && frames.Any())
            {
                var sizeAttrs = await GetSizeAttributesAsync(product, abcMattressEntry);

                pam = new ProductAttributeMapping()
                {
                    ProductId = product.Id,
                    ProductAttributeId = pa.Id,
                    IsRequired = false,
                    AttributeControlType = AttributeControlType.DropdownList,
                    DisplayOrder = 30,
                    TextPrompt = "Frame",
                    ConditionAttributeXml = $"<Attributes><ProductAttribute ID=\"{sizeAttrs.pam.Id}\"><ProductAttributeValue><Value>{sizeAttrs.pav.Id}</Value></ProductAttributeValue></ProductAttribute></Attributes>"
                };
                await _productAttributeService.InsertProductAttributeMappingAsync(pam);
            }
            else if (pam != null && !frames.Any())
            {
                await _productAttributeService.DeleteProductAttributeMappingAsync(pam);
            }
            else if (pam != null)
            {
                await UpdatePamAsync(product, pam, abcMattressEntry);
            }

            if (!frames.Any()) { return; }

            var existingFrames = (await _productAttributeService.GetProductAttributeValuesAsync(pam.Id))
                                                        .Where(pav =>
                                                            pav.ProductAttributeMappingId == pam.Id
                                                        );
            var newFrames = frames.Select(np => np.ToProductAttributeValue(
                pam.Id
            )).OrderBy(f => f.PriceAdjustment).ToList();

            ApplyDisplayOrder(newFrames);

            var toBeDeleted = existingFrames
                .Where(e => !newFrames.Any(n => n.Name == e.Name && n.DisplayOrder == e.DisplayOrder &&
                                                       n.PriceAdjustment == e.PriceAdjustment));
            var toBeInserted = newFrames
                .Where(n => !existingFrames.Any(e => n.Name == e.Name && n.DisplayOrder == e.DisplayOrder &&
                                                       n.PriceAdjustment == e.PriceAdjustment));

            toBeInserted.ToList().ForEach(async n => await _productAttributeService.InsertProductAttributeValueAsync(n));
            toBeDeleted.ToList().ForEach(async e => await _productAttributeService.DeleteProductAttributeValueAsync(e));
        }

        private async Task UpdatePamAsync(
            Product product,
            ProductAttributeMapping pam,
            AbcMattressEntry abcMattressEntry)
        {
            var sizeAttrs = await GetSizeAttributesAsync(product, abcMattressEntry);

            pam.ConditionAttributeXml = $"<Attributes><ProductAttribute ID=\"{sizeAttrs.pam.Id}\"><ProductAttributeValue><Value>{sizeAttrs.pav.Id}</Value></ProductAttributeValue></ProductAttribute></Attributes>";

            await _productAttributeService.UpdateProductAttributeMappingAsync(pam);
        }

        private async Task<(ProductAttributeMapping pam, ProductAttributeValue pav)> GetSizeAttributesAsync(
            Product product,
            AbcMattressEntry abcMattressEntry
        )
        {
            var sizePa = (await _productAttributeService.GetAllProductAttributesAsync())
                                                 .Where(pa => pa.Name == AbcMattressesConsts.MattressSizeName)
                                                 .FirstOrDefault();
            var sizePam = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                                                      .Where(pam => pam.ProductAttributeId == sizePa.Id)
                                                      .FirstOrDefault();
            var sizePav = (await _productAttributeService.GetProductAttributeValuesAsync(sizePam.Id))
                                                .Where(pav =>
                                                pav.ProductAttributeMappingId == sizePam.Id &&
                                                pav.Name == abcMattressEntry.Size
                                                )
                                                .FirstOrDefault();

            return (sizePam, sizePav);
        }

        private async Task MergeMattressProtectorsAsync(AbcMattressModel model, ProductAttribute pa, Product product)
        {
            var displayOrder = 40;
            var pam = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                                                          .Where(pam => pam.ProductAttributeId == pa.Id)
                                                          .FirstOrDefault();
            var abcMattressEntry = _abcMattressEntryService.GetAbcMattressEntriesByModelId(model.Id)
                                                           .Where(ame => pa.Name == $"Mattress Protector ({ame.Size})")
                                                           .FirstOrDefault();
            if (abcMattressEntry == null) { return; }

            var protectors = _abcMattressProtectorService.GetAbcMattressProtectorsBySize(abcMattressEntry.Size);

            if (pam == null && protectors.Any())
            {
                var sizeAttrs = await GetSizeAttributesAsync(product, abcMattressEntry);
                pam = new ProductAttributeMapping()
                {
                    ProductId = product.Id,
                    ProductAttributeId = pa.Id,
                    IsRequired = false,
                    AttributeControlType = AttributeControlType.DropdownList,
                    DisplayOrder = displayOrder,
                    TextPrompt = "Mattress Protector",
                    ConditionAttributeXml = $"<Attributes><ProductAttribute ID=\"{sizeAttrs.pam.Id}\"><ProductAttributeValue><Value>{sizeAttrs.pav.Id}</Value></ProductAttributeValue></ProductAttribute></Attributes>"
                };
                await _productAttributeService.InsertProductAttributeMappingAsync(pam);
            }
            else if (pam != null && !protectors.Any())
            {
                await _productAttributeService.DeleteProductAttributeMappingAsync(pam);
            }
            else if (pam != null)
            {
                await UpdatePamAsync(product, pam, abcMattressEntry);
            }

            if (!protectors.Any()) { return; }

            var existingMattressProtectors = (await _productAttributeService.GetProductAttributeValuesAsync(pam.Id))
                                                        .Where(pav =>
                                                            pav.ProductAttributeMappingId == pam.Id
                                                        );
            var newMattressProtectors = protectors.Select(np => np.ToProductAttributeValue(
                pam.Id
            )).OrderBy(mp => mp.PriceAdjustment).ToList();

            ApplyDisplayOrder(newMattressProtectors);

            var toBeDeleted = existingMattressProtectors
                .Where(e => !newMattressProtectors.Any(n => n.Name == e.Name &&
                                                       n.DisplayOrder == e.DisplayOrder &&
                                                       n.PriceAdjustment == e.PriceAdjustment));
            var toBeInserted = newMattressProtectors
                .Where(n => !existingMattressProtectors.Any(e => n.Name == e.Name &&
                                                            n.DisplayOrder == e.DisplayOrder &&
                                                            n.PriceAdjustment == e.PriceAdjustment));

            toBeInserted.ToList().ForEach(async n => await _productAttributeService.InsertProductAttributeValueAsync(n));
            toBeDeleted.ToList().ForEach(async e => await _productAttributeService.DeleteProductAttributeValueAsync(e));
        }

        private async Task MergeHomeDeliveryAsync(ProductAttribute pa, Product product)
        {
            var pam = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                                                          .Where(pam => pam.ProductAttributeId == pa.Id)
                                                          .FirstOrDefault();
            if (pam == null)
            {
                pam = new ProductAttributeMapping()
                {
                    ProductId = product.Id,
                    ProductAttributeId = pa.Id,
                    IsRequired = false,
                    AttributeControlType = AttributeControlType.MultilineTextbox,
                    DisplayOrder = 0
                };
                await _productAttributeService.InsertProductAttributeMappingAsync(pam);
            }
        }

        public async Task SetManufacturerAsync(AbcMattressModel abcMattressModel, Product product)
        {
            var existingProductManufacturer = (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id, true)).FirstOrDefault();
            var newProductManufacturer = new ProductManufacturer()
            {
                ProductId = product.Id,
                ManufacturerId = abcMattressModel.ManufacturerId.Value
            };

            // No current manufacturer, insert
            if (existingProductManufacturer == null)
            {
                await _manufacturerService.InsertProductManufacturerAsync(newProductManufacturer);
            }
            // manufacturers don't match, update
            else if (existingProductManufacturer.ManufacturerId != newProductManufacturer.ManufacturerId)
            {
                existingProductManufacturer.ManufacturerId = newProductManufacturer.ManufacturerId;
                await _manufacturerService.UpdateProductManufacturerAsync(existingProductManufacturer);
            }
        }

        public async Task SetCategoriesAsync(AbcMattressModel model, Product product)
        {
            var existingProductCategories = await _categoryService.GetProductCategoriesByProductIdAsync(product.Id);
            var entries = _abcMattressEntryService.GetAbcMattressEntriesByModelId(model.Id);
            var newProductCategories = new List<ProductCategory>();
            foreach (var entry in entries)
            {
                var pc = await AbcMattressEntryToProductCategoryAsync(entry);
                newProductCategories.Add(pc);
            }

            // comfort
            var comfortCategory = (await _categoryService.GetAllCategoriesAsync())
                                             .Where(c => c.Name.ToLower().Equals(ConvertComfortToCategoryName(model.Comfort)))
                                             .FirstOrDefault();
            if (comfortCategory != null)
            {
                newProductCategories.Add(new ProductCategory()
                {
                    ProductId = product.Id,
                    CategoryId = comfortCategory.Id
                });
            }

            // brand
            if (model.BrandCategoryId.HasValue)
            {
                var brandCategory = await _categoryService.GetCategoryByIdAsync(model.BrandCategoryId.Value);
                if (brandCategory != null)
                {
                    newProductCategories.Add(new ProductCategory()
                    {
                        ProductId = product.Id,
                        CategoryId = brandCategory.Id
                    });
                }
            }

            var toBeDeleted = existingProductCategories
                .Where(e => !newProductCategories.Any(n => n.ProductId == e.ProductId && n.CategoryId == e.CategoryId));
            var toBeInserted = newProductCategories
                .Where(n => !existingProductCategories.Any(e => n.ProductId == e.ProductId && n.CategoryId == e.CategoryId));

            toBeInserted.ToList().ForEach(async n => await _categoryService.InsertProductCategoryAsync(n));
            toBeDeleted.ToList().ForEach(async e => await _categoryService.DeleteProductCategoryAsync(e));
        }

        private async Task<string> GetProductNameAsync(AbcMattressModel model)
        {
            var modelName = model.Description ?? model.Name;

            var brand = await _manufacturerService.GetManufacturerByIdAsync(model.ManufacturerId.Value);

            return $"{brand.Name} {modelName}";
        }

        private async Task MergeGiftsAsync(AbcMattressModel model, ProductAttribute pa, Product product)
        {
            var displayOrder = 50;
            var pam = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                                                          .Where(pam => pam.ProductAttributeId == pa.Id && pam.DisplayOrder == displayOrder)
                                                          .FirstOrDefault();
            var modelHasGifts = _abcMattressGiftService.GetAbcMattressGiftsByModelId(model.Id).Any();

            if (pam == null && modelHasGifts)
            {
                pam = new ProductAttributeMapping()
                {
                    ProductId = product.Id,
                    ProductAttributeId = pa.Id,
                    IsRequired = false,
                    AttributeControlType = AttributeControlType.DropdownList,
                    DisplayOrder = displayOrder
                };
                await _productAttributeService.InsertProductAttributeMappingAsync(pam);
            }
            else if (pam != null && !modelHasGifts)
            {
                await _productAttributeService.DeleteProductAttributeMappingAsync(pam);
            }

            if (!modelHasGifts) { return; }

            var existingGifts = (await _productAttributeService.GetProductAttributeValuesAsync(pam.Id))
                                                        .Where(pav =>
                                                            pav.ProductAttributeMappingId == pam.Id
                                                        );
            var gifts = _abcMattressGiftService.GetAbcMattressGiftsByModelId(model.Id);
            var newGifts = gifts.Select(g => g.ToProductAttributeValue(pam.Id)).ToList();

            var toBeDeleted = existingGifts
                .Where(e => !newGifts.Any(n => n.Name == e.Name && n.DisplayOrder == e.DisplayOrder));
            var toBeInserted = newGifts
                .Where(n => !existingGifts.Any(e => n.Name == e.Name && n.DisplayOrder == e.DisplayOrder));

            toBeInserted.ToList().ForEach(async n => await _productAttributeService.InsertProductAttributeValueAsync(n));
            toBeDeleted.ToList().ForEach(async e => await _productAttributeService.DeleteProductAttributeValueAsync(e));
        }

        private async Task MergeBasesAsync(AbcMattressModel model, ProductAttribute pa, Product product)
        {
            var attributeName = "Box Spring or Adjustable Base";
            var pam = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                                                          .Where(pam => pam.ProductAttributeId == pa.Id && pam.TextPrompt == attributeName)
                                                          .FirstOrDefault();
            var abcMattressEntry = _abcMattressEntryService.GetAbcMattressEntriesByModelId(model.Id)
                                                           .Where(ame => pa.Name == $"Base ({ame.Size})")
                                                           .FirstOrDefault();
            if (abcMattressEntry == null) { return; }

            var bases = _abcMattressBaseService.GetAbcMattressBasesByEntryId(abcMattressEntry.Id);

            if (pam == null && bases.Any())
            {
                var sizePa = (await _productAttributeService.GetAllProductAttributesAsync())
                                                            .Where(pa => pa.Name == AbcMattressesConsts.MattressSizeName)
                                                            .FirstOrDefault();
                var sizePam = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                                                          .Where(pam => pam.ProductAttributeId == sizePa.Id)
                                                          .FirstOrDefault();
                var sizePav = (await _productAttributeService.GetProductAttributeValuesAsync(sizePam.Id))
                                                        .Where(pav =>
                                                            pav.ProductAttributeMappingId == sizePam.Id &&
                                                            pav.Name == abcMattressEntry.Size
                                                        )
                                                        .FirstOrDefault();
                pam = new ProductAttributeMapping()
                {
                    ProductId = product.Id,
                    ProductAttributeId = pa.Id,
                    IsRequired = false,
                    AttributeControlType = AttributeControlType.DropdownList,
                    DisplayOrder = 10,
                    TextPrompt = attributeName,
                    ConditionAttributeXml = $"<Attributes><ProductAttribute ID=\"{sizePam.Id}\"><ProductAttributeValue><Value>{sizePav.Id}</Value></ProductAttributeValue></ProductAttribute></Attributes>"
                };
                await _productAttributeService.InsertProductAttributeMappingAsync(pam);
            }
            else if (pam != null && !bases.Any())
            {
                await _productAttributeService.DeleteProductAttributeMappingAsync(pam);
            }
            else if (pam != null)
            {
                var pas = await _productAttributeService.GetAllProductAttributesAsync();
                var sizePa = pas.Where(pa => pa.Name == AbcMattressesConsts.MattressSizeName)
                                .FirstOrDefault();

                var pams = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                var sizePam = pams.Where(pam => pam.ProductAttributeId == sizePa.Id)
                                  .FirstOrDefault();

                var sizePavs = await _productAttributeService.GetProductAttributeValuesAsync(sizePam.Id);
                var sizePav = sizePavs.Where(pav => 
                                        pav.ProductAttributeMappingId == sizePam.Id &&
                                        pav.Name == abcMattressEntry.Size
                                   ).FirstOrDefault();

                pam.ConditionAttributeXml = $"<Attributes><ProductAttribute ID=\"{sizePam.Id}\"><ProductAttributeValue><Value>{sizePav.Id}</Value></ProductAttributeValue></ProductAttribute></Attributes>";

                await _productAttributeService.UpdateProductAttributeMappingAsync(pam);
            }

            if (!bases.Any()) { return; }

            var pavs = await _productAttributeService.GetProductAttributeValuesAsync(pam.Id);
            var existingBases = pavs.Where(pav =>
                                        pav.ProductAttributeMappingId == pam.Id
                                    );
            var newBases = bases.Select(nb => nb.ToProductAttributeValue(
                pam.Id,
                _abcMattressPackageService.GetAbcMattressPackageByEntryIdAndBaseId(abcMattressEntry.Id, nb.Id).Price,
                abcMattressEntry.Price
            )).OrderBy(nb => nb.PriceAdjustment).ToList();

            ApplyDisplayOrder(newBases);

            var toBeDeleted = existingBases
                .Where(e => !newBases.Any(n => n.Name == e.Name && n.DisplayOrder == e.DisplayOrder && n.PriceAdjustment == e.PriceAdjustment));
            var toBeInserted = newBases
                .Where(n => !existingBases.Any(e => n.Name == e.Name && n.DisplayOrder == e.DisplayOrder && n.PriceAdjustment == e.PriceAdjustment));

            toBeInserted.ToList().ForEach(async n => await _productAttributeService.InsertProductAttributeValueAsync(n));
            toBeDeleted.ToList().ForEach(async e => await _productAttributeService.DeleteProductAttributeValueAsync(e));
        }

        private async Task MergeSizesAsync(AbcMattressModel model, ProductAttribute pa, Product product)
        {
            var pams = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            var pam = pams.Where(pam => pam.ProductAttributeId == pa.Id)
                          .FirstOrDefault();

            if (pam == null)
            {
                pam = new ProductAttributeMapping()
                {
                    ProductId = product.Id,
                    ProductAttributeId = pa.Id,
                    IsRequired = true,
                    AttributeControlType = AttributeControlType.DropdownList,
                    DisplayOrder = 0
                };
                await _productAttributeService.InsertProductAttributeMappingAsync(pam);
            }

            var pavs = await _productAttributeService.GetProductAttributeValuesAsync(pam.Id);
            var existingSizes = pavs.Where(pav => pav.ProductAttributeMappingId == pam.Id);

            var entries = _abcMattressEntryService.GetAbcMattressEntriesByModelId(model.Id);
            var newSizes = entries.Select(e => e.ToProductAttributeValue(pam.Id, product.Price, e.OldPrice)).ToList();

            var toBeDeleted = existingSizes
                .Where(e => !newSizes.Any(n => n.Name == e.Name &&
                                               n.PriceAdjustment == e.PriceAdjustment &&
                                               n.DisplayOrder == e.DisplayOrder &&
                                               n.Cost == e.Cost));
            var toBeInserted = newSizes
                .Where(n => !existingSizes.Any(e => n.Name == e.Name &&
                                                    n.PriceAdjustment == e.PriceAdjustment &&
                                                    n.DisplayOrder == e.DisplayOrder &&
                                                    n.Cost == e.Cost));

            toBeInserted.ToList().ForEach(async n => await _productAttributeService.InsertProductAttributeValueAsync(n));
            toBeDeleted.ToList().ForEach(async e => await _productAttributeService.DeleteProductAttributeValueAsync(e));
        }

        private string ConvertComfortToCategoryName(string comfort)
        {
            return comfort.Replace("-", "").ToLower();
        }

        public async Task<ProductCategory> AbcMattressEntryToProductCategoryAsync(AbcMattressEntry entry)
        {
            var model = _abcMattressService.GetAllAbcMattressModels()
                                           .Where(amm => amm.Id == entry.AbcMattressModelId)
                                           .FirstOrDefault();
            var convertedCategoryName = ConvertSizeToCategoryName(entry.Size);
            var categories = await _categoryService.GetAllCategoriesAsync();
            var category = categories.Where(c => c.Name.ToLower().Equals(convertedCategoryName))
                                     .FirstOrDefault();

            if (category == null)
            {
                throw new Exception($"Unable to find category {convertedCategoryName}");
            }

            return new ProductCategory()
            {
                ProductId = model.ProductId.Value,
                CategoryId = category.Id
            };
        }

        private string ConvertSizeToCategoryName(string size)
        {
            var loweredSize = size.ToLower();
            switch (loweredSize)
            {
                case "twinxl":
                    return "twin extra long";
                case "queen-flexhead":
                    return "queen";
                case "king-flexhead":
                    return "king";
                case "california king-flexhead":
                case "cal/king-flexhead":
                    return "california king";
                default:
                    return loweredSize;
            }
        }

        private static void ApplyDisplayOrder(List<ProductAttributeValue> values)
        {
            var displayOrderCounter = 0;
            foreach (var value in values)
            {
                value.DisplayOrder = displayOrderCounter;
                displayOrderCounter++;
            }
        }

        public bool IsMattressProduct(int productId)
        {
            return _abcMattressService.GetAbcMattressModelByProductId(productId) != null;
        }

        public async Task SetComfortRibbonAsync(AbcMattressModel model, Product product)
        {
            var productRibbonName = GetRibbonByComfort(model.Comfort);

            var conditionIdCommand = $@"
	        SELECT TOP 1 ec.ConditionId FROM SS_PR_ProductRibbon pr
	        JOIN SS_C_EntityCondition ec ON pr.Id = ec.EntityId
	        WHERE pr.Name = '{productRibbonName}'
	        AND ec.EntityType = 30
            ";

            var conditionId = (await _nopDataProvider.QueryAsync<int?>(conditionIdCommand)).FirstOrDefault();
            if (conditionId == null)
            {
                throw new NopException(
                    $"Did not find condition ID needed for mattress ribbon sync, make sure '{productRibbonName}' product ribbon exists."
                );
            }

            var syncCommand = $@"
            DELETE FROM SS_C_ProductOverride
	        WHERE ProductId = {product.Id}
	        INSERT INTO SS_C_ProductOverride VALUES
            ({conditionId}, {product.Id}, 0)
            ";
            await _nopDataProvider.ExecuteNonQueryAsync(syncCommand);

            return;
        }

        private string GetRibbonByComfort(string comfort)
        {
            switch (comfort)
            {
                case "Firm":
                    return "mattress-comfort-firm";
                case "Cushion-Firm":
                    return "mattress-comfort-cushion-firm";
                case "Plush":
                    return "mattress-comfort-plush";
                case "Ultra Luxury Plush":
                    return "mattress-comfort-ultra-luxury-plush";
                default:
                    throw new ArgumentException($"Unsupported mattress comfort provided: {comfort}");
            }
        }
    }
}
