using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.POS.Kaching.Models.SalesModels;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.POS.Kaching.Services
{
    public class SalesService : ISalesService
    {
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public SalesService(ILogger logger, IRepository<ProductAttributeCombination> productAttributeCombinationRepository, IProductService productService, ILocalizationService localizationService, IWorkContext workContext)
        {
            _logger = logger;
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productService = productService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public async Task<int> UpdateStockAsync(Dictionary<string, Models.SalesModels.Root> sales)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Update stock count from Kaching started");

            int count = 0;

            foreach (var kvp in sales.Values)
            {
                foreach (var item in kvp.summary.line_items)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(item.Barcode))
                        {
                            // Here we cant update any stock, lets log it
                            await HandleMissingBarcode(kvp, item);

                            continue;
                        }

                        // Get Combination
                        var combination = await GetCombinationAsync(item.Barcode);
                        if (combination == null)
                        {
                            throw new ArgumentException($"No combiation found with gtin: '{item.Barcode}'");
                        }
                        int originalStockQuantity = combination.StockQuantity;

                        // Do update of stock
                        await UpdateSingleStockAsync(combination, item.Quantity);
                        sb.AppendLine($"Quantity for combination with EAN: '{item.Barcode}' ({item.Name}) has been decreased by {item.Quantity}");

                        await LogStockChangeAsync(kvp, item, combination, originalStockQuantity);

                        count++;
                    }
                    catch (Exception ex)
                    {
                       await _logger.ErrorAsync(ex.Message, ex);
                    }
                }
            }

            sb.AppendLine("Update done, if any sold items did not have an EAN number, error log would have been created");
            await _logger.InformationAsync(sb.ToString());

            return count;
        }

        private async Task LogStockChangeAsync(Root kvp, LineItem item, ProductAttributeCombination combination, int originalStockQuantity)
        {
            var product = await _productService.GetProductByIdAsync(combination.ProductId);
            if (product == null)
            {
                await _logger.ErrorAsync($"No product found to update stock history, tried with ProductId: {combination.ProductId} for product: {item.Name}");
            }
            else

            {
                var localeValue = await _localizationService.GetLocaleStringResourceByNameAsync("Nop.Plugin.POS.Kaching.UpdateStockMessage", (await GetCustomerCurrentLanguageIdAsync()));
                string message;
                if (localeValue == null)
                {
                    message = string.Format("Stock updated through Kaching {0}", $"({kvp.source.cashier_full_name}, {kvp.source.shop_name}).");
                }
                else
                {
                    message = string.Format(localeValue.ResourceValue, $"({kvp.source.cashier_full_name}, {kvp.source.shop_name}).");
                }

                await _productService.AddStockQuantityHistoryEntryAsync(product, -item.Quantity, originalStockQuantity, 0, message, combination.Id);
            }
        }

        private async Task<int> GetCustomerCurrentLanguageIdAsync()
        {
            int languageId = 2;
            var language = await _workContext.GetWorkingLanguageAsync();
            if (language != null)
            {
                languageId = language.Id;
            }

            return languageId;
        }

        private async Task UpdateSingleStockAsync(ProductAttributeCombination combination, int quantity)
        {                        
            combination.StockQuantity -= quantity;
            await _productAttributeCombinationRepository.UpdateAsync(combination);    
        }

        private async Task<ProductAttributeCombination> GetCombinationAsync(string ean)
        {
            var combinations = await _productAttributeCombinationRepository.GetAllAsync(query =>
            {
                return from c in query                       
                       where c.Gtin == ean
                       select c;
            });
           
            return combinations.FirstOrDefault();
        }

        private async Task HandleMissingBarcode(Root kvp, Models.SalesModels.LineItem item)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Kaching - Missing EAN number\r\n\r\n");
            sb.Append("kvp.Key: " + kvp + "\r\n");
            sb.Append("kvp.Value.Identifier: '" + kvp.identifier + "'\r\n");
            sb.Append("kvp.Value.Source.CashierName: '" + kvp.source?.cashier_name + "'\r\n");
            sb.Append("\r\n");

            sb.Append("item.Name.String: '" + item.Name + "'\r\n");
            if (item.Name == null)
            {
                sb.Append("item.Name.NameClass == null\r\n");
            }
            else
            {
                sb.Append("item.Name.NameClass.Da: '" + item.Name + "'\r\n");
                sb.Append("item.Name.NameClass.En: '" + item.Name + "'\r\n");
            }

            sb.Append("\r\n");
            sb.Append("item.Id: '" + item.Id + "'\r\n");                        
            sb.Append("\r\n");

            sb.Append("iitem.RetailPrice: '" + item.RetailPrice + "'\r\n");
            sb.Append("iitem.CostPrice: '" + item.CostPrice + "'\r\n");
            sb.Append("iitem.SubTotal: '" + item.SubTotal + "'\r\n");
            sb.Append("iitem.Total: '" + item.Total + "'\r\n");

            await _logger.ErrorAsync(sb.ToString());
        }
    }
}