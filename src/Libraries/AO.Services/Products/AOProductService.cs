using AO.Services.Domain;
using AO.Services.Extensions;
using AO.Services.Logging;
using AO.Services.Products.Models;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AO.Services.Utilities;

namespace AO.Services.Products
{
    public class AOProductService : IAOProductService
    {
        #region Private variables
        private readonly ILogger _logger;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IProductAttributeService _productAttributeService;               
        private readonly IRepository<AOProductExtensionData> _productExtensionRepository;
        private readonly IAOCreateProductService _aoCreateProductService;
        
        private string _updaterName;
        private IList<ProductAttributeCombination> _productAttributeCombinations;
        #endregion

        public AOProductService(ILogger logger,
                                IRepository<ProductAttributeCombination> productAttributeCombinationRepository,
                                IProductAttributeService productAttributeService,
                                IRepository<Product> productRepository,
                                IRepository<AOProductExtensionData> productExtensionRepository,
                                IConfiguration configuration,
                                IAOCreateProductService aoCreateProductService)
        {
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productRepository = productRepository;
            _productExtensionRepository = productExtensionRepository;
            _productAttributeService = productAttributeService;
            _aoCreateProductService = aoCreateProductService;
            _logger = logger;

            var apiSettings = configuration["ApiSettings:ApiUrl"];
            if (string.IsNullOrEmpty(apiSettings))
            {
                throw new Exception("ApiSettings:ApiUrl missing from configuration. Needed to create new products.");
            }                        
        }

        [Obsolete("This method makes use of web api, use AO.Services.Services.SyncjobServices.IAONopProductService.UpdateDatabaseAsync instead")]
        public async Task<string> SaveVariantDataAsync(List<VariantData> variantDataList, string updaterName, int minimumStockCount)
        {
            var fullFileLogger = NLogBuilder.ConfigureNLog($"{LoggingUtil.ExecutingFolder}\\Logging\\nlog.config").GetCurrentClassLogger();
            StringBuilder sb = new();
            sb.Append($"{Environment.NewLine} **** Starting running through products from {updaterName} **** {Environment.NewLine}");

            if (variantDataList == null)
            {
                fullFileLogger.Warn($"variantDataList is empty, doing nothing");
                return "variantDataList is null, nothing to save";
            }

            _productAttributeCombinations = GetAllProductAttributeCombinations();
            _updaterName = updaterName;

            int variantsUpdated = 0, productsCreated = 0, sameVariants = 0, variantsCreated = 0;
            variantDataList = variantDataList.Cleanup();

            int testCount = 0;
            foreach (var data in variantDataList)
            {
                testCount++;
                Console.WriteLine(data.EAN);
                try
                {
                    var orgItemNumber = GetOrgItemNumber(data.OrgItemNumber, updaterName);
                    var product = await GetProduct(orgItemNumber);  
                    
                    var combinations = _productAttributeCombinations.Where(p => p.Gtin == data.EAN).ToList();
                    ProductAttributeCombination combination = null;
                    if (combinations != null && combinations.Count > 0)
                    {
                        combination = combinations.FirstOrDefault();

                        if (combinations.Count > 1)
                        {
                            // Remove all other with the same EAN number
                            await CleanupCombinationsAsync(combinations.Where(c => c.Id != combination.Id).ToList());
                        }
                    }                    
                    
                    // Validate whether this product has a ProductStatus that will allow sync job to control it
                    if (combination != null && product != null)
                    {
                        var productExtension = await GetProductExtensionData(product.Id);
                        if (productExtension != null)
                        {
                            if ((ProductStatus)productExtension.StatusId != ProductStatus.ControlledBySync
                                && (ProductStatus)productExtension.StatusId != ProductStatus.OnHold)
                            {
                                sb.AppendLine($"Combination found in friliv DB with ean: '{data.EAN}' and product found with OrgItemNumber: '{data.OrgItemNumber}', but Product Status is {((ProductStatus)productExtension.StatusId).ToString()}, so skipping this variant");
                                continue;
                            }
                        }
                    }                    

                    (bool combinationFound, bool combinationUpdated, bool allowOutOfStockOrders) = await UpdateStockAsync(data, combination, minimumStockCount);
                    if (combinationFound && combinationUpdated)
                    {
                        // Combination found, and AllowOutOfStockOrders updated
                        variantsUpdated++;
                        sb.AppendLine($"Combination found in friliv DB with ean: '{data.EAN}', AllowOutOfStockOrders changed to {allowOutOfStockOrders}. Stock count from {updaterName} was {data.StockCount}");
                    }
                    else if (combinationFound && combinationUpdated == false)
                    {
                        // Nothing changed
                        sameVariants++;
                        sb.AppendLine($"Combination NOT found in friliv DB with ean: '{data.EAN}', no changes made. Stock count from {updaterName} was {data.StockCount}");
                    }
                    else if (data.StockCount >= minimumStockCount)
                    {
                        // No variant found, create one as stock count is high enough                                                
                        if (product == null)
                        {
                            // No product found, create one
                            product = await _aoCreateProductService.CreateProductAsync(data, orgItemNumber, updaterName);
                            productsCreated++;
                            sb.AppendLine($"Product created with ean: '{data.EAN}'. Stock count from {updaterName} was {data.StockCount}");
                        }

                        if (product != null)
                        {
                            await _aoCreateProductService.CreateVariantAsync(data);
                            _productAttributeCombinations.Add(new ProductAttributeCombination()
                            {
                                Gtin = data.EAN,
                                Sku = data.SupplierProductId,
                                ProductId = product.Id
                            });

                            variantsCreated++;
                            sb.AppendLine($"Variant created with ean: '{data.EAN}'. Stock count from {updaterName} was {data.StockCount}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    var inner = ex;
                    while (inner.InnerException != null)
                        inner = inner.InnerException;
                    await _logger.ErrorAsync("UpdateStock() in SaveVariantData in AOProductService (" + _updaterName + "): " + inner.Message, ex);
                    sb.AppendLine($"Error '{ex}'");
                }
            }

            if (variantsCreated > 0 || variantsUpdated > 0 || productsCreated > 0)
            {
                // Only create log file if anything has changed
                sb.AppendLine($"{Environment.NewLine}Done doing stock count updates{Environment.NewLine}");
                fullFileLogger.Info(sb.ToString());
                NLog.LogManager.Shutdown();
            }

            string message = $"Variants updated: {variantsUpdated:N0}{Environment.NewLine}" +
                             $"Variants not changed: {sameVariants:N0}{Environment.NewLine}" +
                             $"Variants created: {variantsCreated:N0}{Environment.NewLine}" +
                             $"Products created: {productsCreated:N0}{Environment.NewLine}";

            return message;
        }

        /// <summary>
        /// Delete all combination with same ean number as the one we are working on
        /// </summary>
        private async Task CleanupCombinationsAsync(List<ProductAttributeCombination> productAttributeCombinations)
        {
            await _productAttributeCombinationRepository.DeleteAsync(productAttributeCombinations);            
        }

        private async Task<AOProductExtensionData> GetProductExtensionData(int productId)
        {
            var rep = await _productExtensionRepository.GetAllAsync(query =>
            {
                return from extensionData in query
                       where extensionData.ProductId == productId
                       select extensionData;
            });

            return rep.FirstOrDefault();
        }

        private static string GetOrgItemNumber(string orgItemNumber, string frilivSupplierName)
        {
            if (frilivSupplierName.ToLower().Contains("stm"))
            {
                if (orgItemNumber.Contains('-'))
                {
                    // STM uses first part of itemnumber as actual itemnumber, the rest is color and size
                    orgItemNumber = orgItemNumber.Substring(0, orgItemNumber.IndexOf('-'));
                }

                orgItemNumber = "STM-" + orgItemNumber;
            }
            else if (frilivSupplierName.ToLower().Contains("intersurf"))
            {
                orgItemNumber = "Intersurf-" + orgItemNumber;
            }

            return orgItemNumber;
        }

        private async Task<ProductDto> GetProduct(string orgItemNumber)
        {
            var product = await _aoCreateProductService.GetProductAsync(orgItemNumber);
            return product;
        }

        #region Private methods
        /// <summary>
        /// Gets a product attribute combination by Gtin
        /// </summary>
        /// <param name="Gtin">Gtin</param>
        /// <returns>Product attribute combination</returns>
        private ProductAttributeCombination GetProductAttributeCombinationByGtin(string gtin)
        {
            if (string.IsNullOrEmpty(gtin))
                return null;

            gtin = gtin.Trim();

            var query = from pac in _productAttributeCombinationRepository.Table
                        orderby pac.Gtin
                        where pac.Gtin == gtin
                        select pac;
            var combination = query.FirstOrDefault();
            return combination;
        }

        private List<Product> GetAllProductsSKUAndName()
        {
            var query = from m in _productRepository.Table
                        select new Product() { Sku = m.Sku, Name = m.Name };
            var products = query.ToList();
            return products;
        }

        private List<ProductAttributeCombination> GetAllProductAttributeCombinations()
        {
            var query = from pac in _productAttributeCombinationRepository.Table
                        select pac;
            var combinations = query.ToList();
            return combinations;
        }

        private async Task<(bool, bool, bool)> UpdateStockAsync(VariantData data, ProductAttributeCombination combination, int minimumStock)
        {
            if (data.EAN.Length == 14 && data.EAN.StartsWith("0"))
            {                
                data.EAN = data.EAN.Substring(1);
            }
            
            bool combinationFound = false;
            bool combinationUpdated = false;
            bool allowOutOfStockOrders = false;

            if (combination != null)
            {
                combinationFound = true;
                if (data.StockCount < minimumStock)
                {
                    if (combination.AllowOutOfStockOrders)
                    {
                        // Now we dont try to sell this variant as vendor does not have enough in stock
                        combination.AllowOutOfStockOrders = false;
                        await _productAttributeService.UpdateProductAttributeCombinationAsync(combination);
                        combinationUpdated = true;
                    }
                }
                else if (data.StockCount >= minimumStock)
                {
                    if (combination.AllowOutOfStockOrders == false)
                    {
                        // Now we try to sell this variant as vendor has enough in stock
                        combination.AllowOutOfStockOrders = true;
                        await _productAttributeService.UpdateProductAttributeCombinationAsync(combination);
                        combinationUpdated = true;
                    }
                }

                allowOutOfStockOrders = combination.AllowOutOfStockOrders;
            }

            return (combinationFound, combinationUpdated, allowOutOfStockOrders);
        }
        #endregion
    }
}