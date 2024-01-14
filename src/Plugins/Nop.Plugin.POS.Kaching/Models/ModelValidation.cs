using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Nop.Plugin.POS.Kaching.Models.ReconciliationModels;
using Nop.Services.Logging;

namespace Nop.Plugin.POS.Kaching.Models
{
    public class ModelValidation : IModelValidation
    {
        private readonly ILogger _logger;

        public ModelValidation(ILogger logger)
        {
            _logger = logger;
        }

        //public async Task Validate(Dictionary<string, Sale> sales)
        //{
        //    if (sales == null)
        //    {
        //        var errortext = "You must specify the sale details in the request body";
        //        await _logger.ErrorAsync(errortext);
        //        throw new HttpRequestException(errortext);
        //    }

        //    if (sales.Count == 0)
        //    {
        //        var errortext = "No sales found in request";
        //        await _logger.ErrorAsync(errortext);
        //        throw new HttpRequestException(errortext);
        //    }

        //    foreach (KeyValuePair<string, Sale> kvp in sales)
        //    {
        //        if (string.IsNullOrEmpty(kvp.Key))
        //        {
        //            var errortext = "No key for current sale found in request";
        //            await _logger.ErrorAsync(errortext);
        //            throw new HttpRequestException(errortext);
        //        }

        //        if (string.IsNullOrEmpty(kvp.Value.Identifier))
        //        {
        //            var errortext = "No Identifier for current sale found in request";
        //            await _logger.ErrorAsync(errortext);
        //            throw new HttpRequestException(errortext);
        //        }

        //        if (kvp.Value.Summary == null)
        //        {
        //            var errortext = "No Summary for current sale found in request";
        //            await _logger.ErrorAsync(errortext);
        //            throw new HttpRequestException(errortext);
        //        }

        //        if (kvp.Value.Timing == null)
        //        {
        //            var errortext = "No Timing for current sale found in request";
        //            await _logger.ErrorAsync(errortext);
        //            throw new HttpRequestException(errortext);
        //        }

        //        if (kvp.Value.Summary.LineItems == null || kvp.Value.Summary.LineItems.Length == 0)
        //        {
        //            var errortext = "No LineItems for current sale found in request";
        //            await _logger.ErrorAsync(errortext);
        //            throw new HttpRequestException(errortext);
        //        }

        //        foreach (var item in kvp.Value.Summary.LineItems)
        //        {
        //            if (item.Behavior != null && !string.IsNullOrEmpty(item.EcomId))
        //            {
        //                // Behavior has Shipping information. EcomId means the product did not exist in store, place a weborder for that.
        //                if (item.Behavior.Shipping == null)
        //                {
        //                    var errortext = "No Shipping information for current sale found in request. This is a ecom sale so we need that information";
        //                    await _logger.ErrorAsync(errortext);
        //                    throw new HttpRequestException(errortext);
        //                }

        //                if (item.Behavior.Shipping.Address == null)
        //                {
        //                    var errortext = "No Address information for current sale found in Shipping in request. This is a ecom sale so we need that information";
        //                    await _logger.ErrorAsync(errortext);
        //                    throw new HttpRequestException(errortext);
        //                }

        //                if (item.Behavior.Shipping.CustomerInfo == null)
        //                {
        //                    var errortext = "No CustomerInfo for current sale found in Shipping in request. This is a ecom sale so we need that information";
        //                    await _logger.ErrorAsync(errortext);
        //                    throw new HttpRequestException(errortext);
        //                }

        //                // Dont validate further, the rest is for POS sales
        //                continue;
        //            }


        //            if (item.Name.IsNull)
        //            {
        //                var errortext = "No Name for current sale found in request. we expect Name as a object with translations";
        //                await _logger.ErrorAsync(errortext);
        //                throw new HttpRequestException(errortext);
        //            }

        //            if (string.IsNullOrEmpty(item.Name.NameClass?.Da) && string.IsNullOrEmpty(item.Name.String))
        //            {
        //                var errortext = "No Name (Da) for current sale found in request";
        //                await _logger.ErrorAsync(errortext);
        //                throw new HttpRequestException(errortext);
        //            }

        //            if (string.IsNullOrEmpty(item.Barcode))
        //            {
        //                var itemName = item.Name.String;
        //                if (item.Name.NameClass != null && string.IsNullOrEmpty(item.Name.NameClass.Da) == false)
        //                {
        //                    itemName = item.Name.NameClass.Da;
        //                }
        //                var errortext = "No Barcode for item " + itemName + " found in request. Could be manual product";
        //                await _logger.ErrorAsync(errortext);
        //            }

        //            if (item.Quantity == 0)
        //            {
        //                var errortext = "Quantity for item " + item.Name + " is 0 in request";
        //                await _logger.ErrorAsync(errortext);
        //                throw new HttpRequestException(errortext);
        //            }
        //        }
        //    }
        //}

        public async Task Validate(Dictionary<string, Reconciliation> reconciliation)
        {
            if (reconciliation == null)
            {
                var errortext = "You must specify the Reconciliation details in the request body";
                await _logger.ErrorAsync(errortext);
                throw new HttpRequestException(errortext);
            }

            if (reconciliation.Count == 0)
            {
                var errortext = "No Reconciliations found in request";
                await _logger.ErrorAsync(errortext);
                throw new HttpRequestException(errortext);
            }

            foreach (KeyValuePair<string, Reconciliation> kvp in reconciliation)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                {
                    var errortext = "No key for current Reconciliation found in request";
                    await _logger.ErrorAsync(errortext);
                    throw new HttpRequestException(errortext);
                }
            }
        }

        public async Task Validate(Reconciliation reconciliation)
        {
            if (reconciliation == null)
            {
                var errortext = "You must specify the Reconciliation details in the request body";
                await _logger.ErrorAsync(errortext);
                throw new HttpRequestException(errortext);
            }               
        }
    }

    public interface IModelValidation
    {
        //Task Validate(Dictionary<string, Sale> sales);        
        Task Validate(Dictionary<string, Reconciliation> reconciliation);
        Task Validate(Reconciliation reconciliation);
    }
}
