using AO.ChatGPT;
using AO.Services.Domain;
using AO.Services.Products.Models;
using HtmlAgilityPack;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Seo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static AO.Services.Utilities;

namespace AO.Services.Services.SyncjobServices
{
    public class AONopProductService : IAONopProductService
    {
        #region Private variables
        private readonly ILogger _logger;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IRepository<AOProductExtensionData> _productExtensionDataRepository;
        private readonly IProductStatusService _productStatusService;
        private readonly IImageEditingService _imageEditingService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;       
        private readonly IUrlRecordService _urlRecordService;
        private IList<ProductAttributeCombination> _productAttributeCombinations;
        private IList<AOProductExtensionData> _productExtensionData;
        private IList<Manufacturer> _allManufacturers;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private string _updaterName = "";
        #endregion

        public AONopProductService(IRepository<ProductAttributeCombination> productAttributeCombinationRepository, IRepository<AOProductExtensionData> productExtensionDataRepository, IProductStatusService productStatusService, IProductAttributeService productAttributeService, IProductAttributeParser productAttributeParser, IProductService productService, IUrlRecordService urlRecordService, ILogger logger, ISpecificationAttributeService specificationAttributeService, ILocalizedEntityService localizedEntityService, IPictureService pictureService, IManufacturerService manufacturerService, IImageEditingService imageEditingService)
        {
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productExtensionDataRepository = productExtensionDataRepository;
            _productStatusService = productStatusService;
            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _urlRecordService = urlRecordService;

            _productAttributeCombinations = GetAllProductAttributeCombinations();
            _productExtensionData = _productExtensionDataRepository.GetAll();
            _logger = logger;
            _specificationAttributeService = specificationAttributeService;
            _localizedEntityService = localizedEntityService;
            _pictureService = pictureService;
            _manufacturerService = manufacturerService;
            _imageEditingService = imageEditingService;
        }

        public async Task<SingleUpdateResult> UpdateDatabaseAsync(VariantData variantData, SyncUpdaterInfo syncUpdaterInfo)
        {
            if (_allManufacturers == null ||_allManufacturers.Count == 0)
            {
                _allManufacturers = await _manufacturerService.GetAllManufacturersAsync();
            }

            _updaterName = syncUpdaterInfo.UpdaterName;
            var combination = _productAttributeCombinations.Where(c => c.Gtin == variantData.EAN).FirstOrDefault();
            var singleUpdateResult = new SingleUpdateResult();
            if (combination != null)
            {
                // We have this product variant in Friliv
                var productExtension = _productExtensionData.Where(p => p.ProductId == combination.ProductId).FirstOrDefault();

                if (productExtension == null)
                {
                    // Probably a new product, lets create new extension for it
                    productExtension = await CreateNewProductExtension(combination);
                    _productExtensionData.Add(productExtension);
                }

                if (syncUpdaterInfo.CrawlerEndpoints != null && syncUpdaterInfo.CrawlerEndpoints.Count > 0)
                {
                    // Here we enrich product with text and image
                    var result = await SetCrawledInfoAsync(variantData, syncUpdaterInfo, productExtension);
                    singleUpdateResult.LogCrawlErrors = result.CrawlErrors;
                }

                if (IsStatusValidForSyncJob(productExtension) == false)
                {
                    // This is found, it has a ProductStatus, but the status is not OnHold or ControlledBySync
                    singleUpdateResult.UpdateResult = Result.Skipped;
                    return singleUpdateResult;
                }

                // Now everything should be right to make changes to the variant
                singleUpdateResult = await UpdateVariantAsync(variantData, combination, productExtension, syncUpdaterInfo.MinStockCount);

                return singleUpdateResult;
            }
            else if (variantData.StockCount >= syncUpdaterInfo.MinStockCount)
            {
                return await CreateVariantAsync(variantData);
            }
            else
            {
                singleUpdateResult.UpdateResult = Result.Skipped;
                return singleUpdateResult;
            }
        }

        public async Task CleanUpDatabaseAsync(IList<VariantData> variantsData, IList<ProductAttributeCombination> combinations, int securityPercantage, string updaterName)
        {
            if (variantsData == null || variantsData.Count <= 0 || combinations == null || combinations.Count <= 0 || securityPercantage <= 0)
            {
                return;
            }

            var combinationsToNotSell = new List<ProductAttributeCombination>();

            // Running through all active combinations from supplier
            foreach (var combination in combinations)
            {
                if (variantsData.Any(v => v.EAN == combination.Gtin) == false)
                {
                    // This combination is no longer in the API
                    combinationsToNotSell.Add(combination);
                }
            }

            if (combinationsToNotSell.Count <= 0)
            {
                return;
            }

            double percentageOfCombinationsToNotSell = (double)combinationsToNotSell.Count / combinations.Count * 100;

            if (percentageOfCombinationsToNotSell > securityPercantage)
            {
                await _logger.ErrorAsync($"{updaterName}: Too many combinations would have been removed.{Environment.NewLine}Would have set {combinationsToNotSell.Count:N0} not for sale. Security percantage set to {securityPercantage} in configuration.{Environment.NewLine}Now doing nothing");
                return;
            }

            foreach (var combination in combinationsToNotSell)
            {
                // Now we dont try to sell this variant as vendor does not have this anymore
                combination.AllowOutOfStockOrders = false;
                await _productAttributeService.UpdateProductAttributeCombinationAsync(combination);
            }

            await _logger.ErrorAsync($"{combinationsToNotSell.Count:N0} combinations have been set to not allow out of stock orders");
        }

        #region Private Methods
        private async Task<CrawlResult> SetCrawledInfoAsync(VariantData variantData, SyncUpdaterInfo syncUpdaterInfo, AOProductExtensionData productExtension)
        {
            var crawlResult = new CrawlResult();
            if (variantData.StockCount >= syncUpdaterInfo.MinStockCount)
            {
                if (productExtension == null || (ProductStatus)productExtension.StatusId != ProductStatus.Crawled)
                {
                    // Only do this if product has just been created, or never been prepared or active
                    return crawlResult;
                }

                var product = await _productService.GetProductByIdAsync(productExtension.ProductId);
                if (product == null)
                {
                    return crawlResult;
                }

                // Insert full description from website if missing                
               await InsertFullDescriptionAsync(product, syncUpdaterInfo);

                // Find and add images to product if crawl urls are working
                crawlResult = await InsertImagesFromWebsiteAsync(product, syncUpdaterInfo);
            }

            return crawlResult;
        }

        private async Task<CrawlResult> InsertImagesFromWebsiteAsync(Product product, SyncUpdaterInfo syncUpdaterInfo)
        {
            var crawlResult = new CrawlResult();
            try
            {               
                var productImages = await _productService.GetProductPicturesByProductIdAsync(product.Id);
                if (productImages != null && productImages.Count > 0)
                {
                    // We already have images
                    return crawlResult;
                }

                if (syncUpdaterInfo.CrawlerEndpoints != null && syncUpdaterInfo.CrawlerEndpoints.Count > 0)
                {
                    crawlResult = await GetValidEndpointsAsync(syncUpdaterInfo.CrawlerEndpoints);

                    if (crawlResult.ValidEndpoints.Count > 0)
                    {
                        var imageUrlsFromWeb = await GetImageUrlsAsync(crawlResult.ValidEndpoints[0], syncUpdaterInfo.NodeDefinitionForImages);
                        if (imageUrlsFromWeb != null && imageUrlsFromWeb.Count > 0)
                        {
                            var client = new HttpClient();

                            foreach (var imageUrl in imageUrlsFromWeb)
                            {
                                try
                                {
                                    var response = await client.GetAsync(imageUrl);

                                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
                                    string name = imageUrl.Substring(imageUrl.LastIndexOf("/") + 1);

                                    // Save the downloaded image to a temporary file
                                    string tempFilePath = Path.Combine(Path.GetTempPath(), name);
                                    await File.WriteAllBytesAsync(tempFilePath, imageBytes);

                                    // Crop the image
                                    string croppedFilePath = Path.Combine(Path.GetTempPath(), "cropped_" + name);
                                    _imageEditingService.CropImageToSquare(tempFilePath, Path.GetTempPath(), "cropped_" + name, targetSize: 1500);

                                    // Read the bytes of the cropped image
                                    byte[] croppedImageBytes = await File.ReadAllBytesAsync(croppedFilePath);

                                    var picture = await _pictureService.InsertPictureAsync(croppedImageBytes, "jpeg", name);
                                    await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(product.Name));

                                    await _productService.InsertProductPictureAsync(new ProductPicture
                                    {
                                        PictureId = picture.Id,
                                        ProductId = product.Id,
                                        DisplayOrder = 0                                        
                                    });

                                    // Clean up temporary files
                                    File.Delete(tempFilePath);
                                    File.Delete(croppedFilePath);
                                }
                                catch (Exception ex)
                                {
                                    var message = $"Error in SyncJob '{_updaterName}' when trying to add image to product. Imageurl: '{imageUrl}'. Error: {ex.Message}{Environment.NewLine}{Environment.NewLine}Product: {product.Name} ({product.Id})";
                                    await _logger.ErrorAsync(message, ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var message = $"Error in SyncJob '{_updaterName}' when trying to add images to product. Error: {ex.Message}";
                await _logger.ErrorAsync(message, ex);
            }

            return crawlResult;
        }

        private async Task<List<string>> GetImageUrlsAsync(string endpoint, string nodeDefinitionForImages)
        {
            var imageUrls = new List<string>();
            using HttpClient client = new HttpClient();
            try
            {
                var response = await client.GetStringAsync(endpoint);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response);

                var selectedNodes = doc.DocumentNode.SelectNodes(nodeDefinitionForImages);
                if (selectedNodes != null)
                {
                    var imgNodes = selectedNodes[0].SelectNodes("//img[@src]");

                    if (imgNodes != null)
                    {
                        foreach (var imgNode in imgNodes)
                        {
                            var imageUrl = imgNode.GetAttributeValue("src", string.Empty).Replace("/100/100/", "/1024/1024/");

                            if (ImageFromOrigin(endpoint, imageUrl) == false)
                            {
                                // Ex.: if image is not from https://www.stm-sport.dk
                                continue;
                            }

                            if (await IsEndpointLiveAsync(imageUrl))
                            {
                                imageUrls.Add(imageUrl);
                            }
                        }
                    }                   
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            return imageUrls;
        }

        private bool ImageFromOrigin(string endpoint, string imageUrl)
        {
            if (Uri.TryCreate(endpoint, UriKind.Absolute, out Uri websiteUri) &&
                Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri imageUri))
            {
                return websiteUri.Host == imageUri.Host;
            }

            // At least one of the URIs is invalid
            return false;
        }


        private async Task InsertFullDescriptionAsync(Product product, SyncUpdaterInfo syncUpdaterInfo)
        {
            try
            {                
                if (string.IsNullOrEmpty(product.FullDescription) == false)
                {
                    // We already have full description
                    return;
                }

                if (syncUpdaterInfo.CrawlerEndpoints != null && syncUpdaterInfo.CrawlerEndpoints.Count > 0)
                {
                    var crawlResult = await GetValidEndpointsAsync(syncUpdaterInfo.CrawlerEndpoints);

                    if (crawlResult.ValidEndpoints.Count > 0)
                    {
                        var specificationAttributesFromWeb = await GetAttributesAsync(crawlResult.ValidEndpoints[0], syncUpdaterInfo.NodeDefinitionForFullDescription);
                        if (string.IsNullOrEmpty(specificationAttributesFromWeb.PlainText) == false)
                        {
                            product.FullDescription = specificationAttributesFromWeb.PlainText;
                            await _productService.UpdateProductAsync(product);

                            await TranslateTextAsync(product);
                        }
                    }
                }               
            }
            catch (Exception ex)
            {
                var message = $"Error in SyncJob '{_updaterName}' when trying to add full desccription to product. Error: {ex.Message}";
                await _logger.ErrorAsync(message, ex);
            }

            if (string.IsNullOrEmpty(product.FullDescription))
            {
                // Full description is still empty, so set dummy text to prevent us from trying again next day
                product.FullDescription = $"No text found in '{syncUpdaterInfo?.CrawlerEndpoints?[0]}'";
                await _productService.UpdateProductAsync(product);
            }
        }

        private const int RetryCount = 3;
        private const int DelayDurationMilliseconds = 2000; // 2 seconds

        private async Task TranslateTextAsync(Product product)
        {
            const int _englishLanguageId = 1;
            const int _danishLanguageId = 2;
            const int _swedishLanguageId = 3;

            string _danishLanguage = "da"; // Danish
            string _englishLanguage = "en"; // English
            string _swedishLanguage = "sv"; // Swedish 

            var translations = new Translations("Bearer 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", "gpt-3.5-turbo");

            await TryTranslateAndAddDescription(translations, product, _englishLanguage, _danishLanguage, _danishLanguageId);
            await TryTranslateAndAddDescription(translations, product, _englishLanguage, _swedishLanguage, _swedishLanguageId);
        }

        private async Task TryTranslateAndAddDescription(Translations translations, Product product, string fromLanguage, string toLanguage, int languageId)
        {
            for (int i = 0; i < RetryCount; i++)
            {
                try
                {
                    string translatedDescription = await translations.TranslateText(fromLanguage, toLanguage, product.FullDescription);
                    translatedDescription = Utilities.CleanupText(translatedDescription, languageId);
                    await AddFullDescriptionAsync(product, languageId, translatedDescription);
                    return; // If successful, exit the method
                }
                catch
                {
                    if (i == RetryCount - 1) // If it's the last retry
                    {
                        await _logger.ErrorAsync($"{_updaterName}: Giving up after {RetryCount} attempt to translate from {fromLanguage} to {toLanguage}, on product {product.Name}");
                        return;
                    }
                    await Task.Delay(DelayDurationMilliseconds);
                }
            }
        }        

        private async Task AddFullDescriptionAsync(Product product, int languageId, string localeValue)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(product, x => x.FullDescription, localeValue, languageId);
        }       

        private async Task<CrawlResult> GetValidEndpointsAsync(IList<string> endpoints)
        {
            var validEndpoints = new List<string>();
            var crawlErrors = new StringBuilder(); // Use StringBuilder to efficiently build the error string.

            foreach (var endpoint in endpoints)
            {
                if (await IsEndpointLiveAsync(endpoint))
                {
                    validEndpoints.Add(endpoint);
                }
                else
                {
                    if (endpoint.Contains("-"))
                    {
                        var endpointWithoutItemNumber = endpoint.Substring(0, endpoint.LastIndexOf("-")).TrimEnd('-');
                        if (await IsEndpointLiveAsync(endpointWithoutItemNumber))
                        {
                            validEndpoints.Add(endpointWithoutItemNumber);
                        }
                        else
                        {
                            // Append the error message to the crawlErrors StringBuilder.
                            crawlErrors.AppendLine($"Syncjob ({_updaterName}) failed to load page: '{endpointWithoutItemNumber}'");
                        }
                    }
                }
            }

            return new CrawlResult { ValidEndpoints = validEndpoints, CrawlErrors = crawlErrors.ToString() };
        }


        private async Task<bool> IsEndpointLiveAsync(string endpoint)
        {
            try
            {
                using HttpClient client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Head, endpoint);
                using var response = await client.SendAsync(request);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<SpecificationResult> GetAttributesAsync(string endpoint, string nodeDefinition)
        {
            var result = new SpecificationResult();
            using HttpClient client = new HttpClient();
            try
            {
                var response = await client.GetStringAsync(endpoint);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response);

                var selectedNodes = doc.DocumentNode.SelectNodes(nodeDefinition);

                if (selectedNodes != null)
                {
                    foreach (var node in selectedNodes)
                    {
                        var stringBuilder = new StringBuilder();

                        foreach (var textNode in node.DescendantsAndSelf().Where(n => n.NodeType == HtmlAgilityPack.HtmlNodeType.Text && !string.IsNullOrWhiteSpace(n.InnerHtml)))
                        {
                            stringBuilder.Append(HtmlEntity.DeEntitize(textNode.InnerHtml.Trim()));
                            stringBuilder.Append("<br />");
                        }

                        result.PlainText = stringBuilder.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            return result;
        }

        private async Task<SingleUpdateResult> CreateVariantAsync(VariantData variantData)
        {
            var singleUpdateResult = new SingleUpdateResult();

            var orgItemNumber = GetOrgItemNumber(variantData.OrgItemNumber);
            var product = await _productService.GetProductBySkuAsync(orgItemNumber);

            if (product == null)
            {
                product = await CreateProduct(variantData, orgItemNumber);
                if (product == null)
                {
                    singleUpdateResult.UpdateResult = Result.Skipped;
                    singleUpdateResult.ErrorMessage = $"{_updaterName} Job: Product is null, see other error. OrgItemNumber: '{variantData.OrgItemNumber}'";
                    return singleUpdateResult;
                }
                else
                {
                    singleUpdateResult.ProductCreated = true;
                }
            }

            var productExtension = _productExtensionData.Where(p => p.ProductId == product.Id).FirstOrDefault();
            if (IsStatusValidForSyncJob(productExtension) == false)
            {
                // We should be creating a new variant here, but the product has status to not do that
                singleUpdateResult.UpdateResult = Result.Skipped;
                return singleUpdateResult;
            }

            if (string.IsNullOrEmpty(variantData.SizeStr))
            {
                singleUpdateResult.UpdateResult = Result.Skipped;
                singleUpdateResult.WarningMessage = $"SizeStr is null, not creating this variant: '{variantData.EAN}' for product: {product.Name} ({product.Id})";
                return singleUpdateResult;
            }

            if (string.IsNullOrEmpty(variantData.ColorStr))
            {
                singleUpdateResult.UpdateResult = Result.Skipped;
                singleUpdateResult.WarningMessage = $"ColorStr is null, not creating this variant: '{variantData.EAN}' for product: {product.Name} ({product.Id})";
                return singleUpdateResult;
            }

            var productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);

            ProductAttributeMapping productAttributeMappingSize = await GetOrCreateProductSizeMapping(product, productAttributeMappings);
            ProductAttributeMapping productAttributeMappingColor = await GetOrCreateProductColorMapping(product, productAttributeMappings);

            ProductAttributeValue sizeValue = await GetOrCreateSizeValue(variantData, product, productAttributeMappingSize);
            ProductAttributeValue colorValue = await GetOrCreateColorValue(variantData, product, productAttributeMappingColor);

            var allAttributesXml = await _productAttributeParser.GenerateAllCombinationsAsync(product, true, new List<int> { sizeValue.Id, colorValue.Id });

            // Do we now have any?
            productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);

            foreach (var attributesXml in allAttributesXml)
            {
                var doc = new XmlDocument();
                doc.LoadXml(attributesXml);
                var productAttributeNode = doc.GetElementsByTagName("ProductAttribute");
                if (productAttributeNode.Count != 2)
                {
                    continue;
                }

                // Save combination (Never add stock here, as its new and the StockCount is Vendors stock)
                var combination = new ProductAttributeCombination
                {
                    ProductId = product.Id,
                    AttributesXml = attributesXml,
                    AllowOutOfStockOrders = false, // Always be false here, as this variant is not ready yet
                    Sku = product.Sku,
                    ManufacturerPartNumber = product.ManufacturerPartNumber,
                    Gtin = variantData.EAN,
                    OverriddenPrice = null,
                    NotifyAdminForQuantityBelow = 1
                };
                await _productAttributeService.InsertProductAttributeCombinationAsync(combination);

                singleUpdateResult.LogMessage = $"Combination created in friliv DB with ean: '{variantData.EAN}', AllowOutOfStockOrders set to false as it is new and not ready";
                singleUpdateResult.UpdateResult = Result.Created;
                // Now this one combination has been created
                // IMPORTANT to step out here
                break;
            }
            
            return singleUpdateResult;
        }

        private async Task<ProductAttributeMapping> GetOrCreateProductColorMapping(Product product, IList<ProductAttributeMapping> productAttributeMappings)
        {
            var productAttributeMappingColor = productAttributeMappings.Where(p => p.ProductAttributeId == 2).FirstOrDefault();
            if (productAttributeMappingColor == null)
            {
                productAttributeMappingColor = await CreateProductAttributeMappingColor(product.Id);
            }

            return productAttributeMappingColor;
        }

        private async Task<ProductAttributeMapping> GetOrCreateProductSizeMapping(Product product, IList<ProductAttributeMapping> productAttributeMappings)
        {
            var productAttributeMappingSize = productAttributeMappings.Where(p => p.ProductAttributeId == 1).FirstOrDefault();
            if (productAttributeMappingSize == null)
            {
                productAttributeMappingSize = await CreateProductAttributeMappingSize(product.Id);
            }

            return productAttributeMappingSize;
        }

        private async Task<ProductAttributeValue> GetOrCreateColorValue(VariantData variantData, Product product, ProductAttributeMapping productAttributeMappingColor)
        {
            var colorValues = await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMappingColor.Id);
            ProductAttributeValue colorValue = colorValues.Where(c => c.Name.ToLower().Trim() == variantData.ColorStr.ToLower().Trim()).FirstOrDefault();
            if (colorValue == null && string.IsNullOrEmpty(variantData.ColorStr) == false)
            {
                // No color found in Nop and we have string, lets create it
                colorValue = await AddColorValueAsync(product.Id, productAttributeMappingColor.Id, variantData);
            }

            return colorValue;
        }

        private async Task<ProductAttributeValue> GetOrCreateSizeValue(VariantData variantData, Product product, ProductAttributeMapping productAttributeMappingSize)
        {
            var sizeValues = await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMappingSize.Id);
            ProductAttributeValue sizeValue = sizeValues.Where(s => s.Name.ToLower().Trim() == variantData.SizeStr.ToLower().Trim()).FirstOrDefault();
            if (sizeValue == null && string.IsNullOrEmpty(variantData.SizeStr) == false)
            {
                // No size found in Nop and we have string, lets create it
                sizeValue = await AddSizeValueAsync(product.Id, productAttributeMappingSize.Id, variantData);
            }

            return sizeValue;
        }

        private async Task<ProductAttributeMapping> CreateProductAttributeMappingColor(int productId)
        {
            ProductAttributeMapping productAttributeMappingColor = new()
            {
                AttributeControlTypeId = (int)AttributeControlType.ColorSquares,
                IsRequired = true,
                ProductAttributeId = 2, // Color
                ProductId = productId,
                DisplayOrder = 0
            };

            await _productAttributeService.InsertProductAttributeMappingAsync(productAttributeMappingColor);
            return productAttributeMappingColor;
        }

        private async Task<ProductAttributeMapping> CreateProductAttributeMappingSize(int productId)
        {
            ProductAttributeMapping productAttributeMappingColor = new()
            {
                AttributeControlTypeId = (int)AttributeControlType.Checkboxes,
                IsRequired = true,
                ProductAttributeId = 1, // Size                
                ProductId = productId,
                DisplayOrder = 1
            };

            await _productAttributeService.InsertProductAttributeMappingAsync(productAttributeMappingColor);
            return productAttributeMappingColor;
        }

        private async Task<ProductAttributeValue> AddColorValueAsync(int productId, int productAttributeMappingSizeId, VariantData variantData)
        {
            var productAttributeValue = new ProductAttributeValue()
            {
                AssociatedProductId = productId,
                Name = Utilities.FirstUpperCase(variantData.ColorStr),
                Quantity = variantData.StockCount,
                AttributeValueTypeId = 2, // 2==Color                        
                ProductAttributeMappingId = productAttributeMappingSizeId
            };

            await _productAttributeService.InsertProductAttributeValueAsync(productAttributeValue);
            return productAttributeValue;
        }

        private async Task<ProductAttributeValue> AddSizeValueAsync(int productId, int productAttributeMappingSizeId, VariantData variantData)
        {
            var productAttributeValue = new ProductAttributeValue()
            {
                AssociatedProductId = productId,
                Name = Utilities.FirstUpperCase(variantData.SizeStr),
                Quantity = variantData.StockCount,
                AttributeValueTypeId = 1, // 1==Size                        
                ProductAttributeMappingId = productAttributeMappingSizeId
            };

            await _productAttributeService.InsertProductAttributeValueAsync(productAttributeValue);
            return productAttributeValue;
        }

        private async Task<Product> CreateProduct(VariantData variantData, string orgItemNumber)
        {
            try
            {
                var product = new Product()
                {
                    Name = variantData.Title,
                    Price = variantData.RetailPrice,
                    AdminComment = $"Product created through sync ({_updaterName}) with use of variant data from '{variantData.Brand}'.{Environment.NewLine}{DateTime.Now:dd-MM-yyyy tt:mm:ss}",
                    ShortDescription = variantData.OriginalTitle,
                    Sku = orgItemNumber,
                    ManufacturerPartNumber = variantData.OrgItemNumber,
                    Published = false,
                    MarkAsNew = true,
                    VisibleIndividually = true,
                    ProductTypeId = 5, // 5==Simple product type
                    ProductTemplateId = 1,
                    BasepriceUnitId = 3,
                    BasepriceBaseUnitId = 3,
                    CreatedOnUtc = DateTime.UtcNow,
                    ManageInventoryMethodId = 2, // 2==Track inventory by product attributes
                    OrderMaximumQuantity = 1000,
                    AllowCustomerReviews = true,
                    DisplayStockAvailability = true,
                    DisplayStockQuantity = true,
                    IsShipEnabled = true,
                    TaxCategoryId = 6, // 6==Dansk moms'
                    DeliveryDateId = 2, // 2 = 2-4 days
                    VendorId = GetVendorId() // 2 = STM, Intersurf = 3                      
                };
                await _productService.InsertProductAsync(product);

                await _urlRecordService.SaveSlugAsync(product, await _urlRecordService.ValidateSeNameAsync(product, string.Empty, variantData.Title, false), 0);
                await _urlRecordService.SaveSlugAsync(product, await _urlRecordService.ValidateSeNameAsync(product, string.Empty, variantData.Title, false), 2);

                await AddManufacturerAsync(variantData, product);

                product.UpdatedOnUtc = DateTime.UtcNow;
                await _productService.UpdateProductAsync(product);

                await _productExtensionDataRepository.InsertAsync(new AOProductExtensionData()
                {
                    ForceOutOfGoogleShopping = false,
                    ForceOutOfPriceRunner = false,
                    ProductId = product.Id,
                    StatusId = (int)ProductStatus.Crawled,
                    InventoryCountLastDone = Convert.ToDateTime("01-01-1970")
                });

                return product;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            return null;
        }

        private async Task AddManufacturerAsync(VariantData variantData, Product product)
        {
            if (_allManufacturers != null)
            {
                var manufacturer = _allManufacturers.Where(m => m.Name.ToLower().Trim() == variantData?.Brand?.ToLower().Trim()).FirstOrDefault();
                if (manufacturer != null)
                {
                    var productManufacturer = new ProductManufacturer()
                    {
                        ManufacturerId = manufacturer.Id,
                        ProductId = product.Id
                    };

                    await _manufacturerService.InsertProductManufacturerAsync(productManufacturer);
                }
            }
        }

        private int GetVendorId()
        {
            switch(_updaterName.ToLower())
            {
                case "stm": return 2;
                case "intersurf": return 3;
                default: throw new Exception($"SyncJob do not suppport this updater: {_updaterName}");
            }
        }

        private string GetOrgItemNumber(string orgItemNumber)
        {
            if (orgItemNumber.Contains('-') && _updaterName.ToLower() == "stm")
            {
                // STM uses first part of itemnumber as actual itemnumber, the rest is color and size
                orgItemNumber = orgItemNumber.Substring(0, orgItemNumber.IndexOf('-'));
            }

            orgItemNumber = $"{_updaterName}-" + orgItemNumber;
            return orgItemNumber;
        }

        /// <summary>
        /// Combination already found in Nop, lets update it
        /// </summary>
        private async Task<SingleUpdateResult> UpdateVariantAsync(VariantData variantData, ProductAttributeCombination combination, AOProductExtensionData productExtension, int minimumStockCount)
        {
            var singleUpdateResult = new SingleUpdateResult();

            if (variantData.StockCount < minimumStockCount)
            {
                if (combination.AllowOutOfStockOrders)
                {
                    // Now we dont try to sell this variant as vendor does not have enough in stock
                    combination.AllowOutOfStockOrders = false;
                    await _productAttributeService.UpdateProductAttributeCombinationAsync(combination);

                    // Log that we have set Not allow out of stock orders 
                    await _productStatusService.AddToProductStatusHistory(productExtension, $"Combination with ean {combination.Gtin} as been set to Not allow out of stock orders as {_updaterName} stockcount is: {variantData.StockCount}");
                    singleUpdateResult.UpdateResult = Result.Updated;
                    singleUpdateResult.LogMessage = $"Combination found in friliv DB with ean: '{variantData.EAN}', AllowOutOfStockOrders changed to false. Stock count from {_updaterName} was {variantData.StockCount}";
                }
                else
                {
                    singleUpdateResult.UpdateResult = Result.Skipped;
                }
            }
            else if (variantData.StockCount >= minimumStockCount)
            {
                if (combination.AllowOutOfStockOrders == false)
                {
                    // If no PictureId, maybe this variant is not ready, maybe missing colorcode too
                    if (combination.PictureId > 0)
                    {
                        // Now we try to sell this variant as vendor has enough in stock
                        combination.AllowOutOfStockOrders = true;
                        await _productAttributeService.UpdateProductAttributeCombinationAsync(combination);

                        // Log that we have set Allow out of stock orders 
                        await _productStatusService.AddToProductStatusHistory(productExtension, $"Combination with ean {combination.Gtin} as been set to Allow out of stock orders as {_updaterName} stockcount is: {variantData.StockCount}");
                        singleUpdateResult.UpdateResult = Result.Updated;
                        singleUpdateResult.LogMessage = $"Combination found in friliv DB with ean: '{variantData.EAN}', AllowOutOfStockOrders changed to true. Stock count from {_updaterName} was {variantData.StockCount}";
                    }
                    else
                    {
                        singleUpdateResult.UpdateResult = Result.Skipped;
                    }
                }
                else
                {
                    singleUpdateResult.UpdateResult = Result.Skipped;
                }
            }
            else
            {
                singleUpdateResult.UpdateResult = Result.Skipped;
            }

            return singleUpdateResult;
        }

        private async Task<AOProductExtensionData> CreateNewProductExtension(ProductAttributeCombination combination)
        {
            // No one for this product, create a new
            var productExtension = new AOProductExtensionData()
            {
                ProductId = combination.ProductId,
                StatusId = (int)ProductStatus.Crawled,
                InventoryCountLastDone = Convert.ToDateTime("01-01-1970")
            };
            await _productStatusService.InsertNewProductStatus(productExtension);

            // Log that we have created a new ProductExtension (ProductStatus)
            await _productStatusService.AddToProductStatusHistory(productExtension, $"New product status created for {combination.Gtin}, did not exist before");

            return productExtension;
        }

        /// <summary>
        /// Here we make sure we dont try to change status of products/variants without proper ProductStatus (OnHold or ControlledBySync)
        /// </summary>
        /// <param name="productId"></param>       
        private bool IsStatusValidForSyncJob(AOProductExtensionData productExtension)
        {
            if (productExtension != null)
            {
                if ((ProductStatus)productExtension.StatusId == ProductStatus.Deactive)
                {
                    return false;
                }
            }

            return true;
        }

        private List<ProductAttributeCombination> GetAllProductAttributeCombinations()
        {
            var query = from pac in _productAttributeCombinationRepository.Table
                        select pac;
            var combinations = query.ToList();
            return combinations;
        } 
        #endregion
    }   
}