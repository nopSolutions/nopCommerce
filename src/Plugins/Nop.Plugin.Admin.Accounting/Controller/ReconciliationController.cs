using AO.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Admin.Accounting.Models;
using Nop.Plugin.POS.Kaching.Models.ReconciliationModels;
using Nop.Plugin.POS.Kaching.Services;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Admin.Accounting.Controller
{
    public class ReconciliationController : BaseAdminController
    {
        private readonly ILogger _logger;
        private readonly ILogService _logService;
        private readonly AccountingSettings _accountingSettings;

        public ReconciliationController(ILogger logger, ILogService logService, AccountingSettings accountingSettings)
        {
            _logger = logger;
            _logService = logService;
            _accountingSettings = accountingSettings;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> ListAsync()
        {
            ReconciliationModel reconciliationModel = new();
            try
            {
                DateTime from = DateTime.Now.AddDays(-_accountingSettings.ReconciliationListDaysBack);
                DateTime to = DateTime.Now;

                var results = await FetchResultAsync(from, to);
                if (results != null && results.Count > 0)
                {
                    var reconciliationList = await FetchDeserializedAsync(results);
                    reconciliationList.RemoveAll(item => item == null);
                    if (reconciliationList != null && reconciliationList.Count > 0)
                    {
                        var enrichedList = await EnrichReconciliationListAsync(reconciliationList);

                        reconciliationModel.ReconciliationList = enrichedList;
                        reconciliationModel.Count = reconciliationList.Count;
                        reconciliationModel.dtFrom = from;
                        reconciliationModel.dtTo = to;
                    }
                }
                else
                {
                    reconciliationModel.ErrorMessage = "Endnu ingen kasselukningsrapporter. Se evt. mere i loggen.";
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.ToString());
                reconciliationModel.ErrorMessage = ex.ToString();
            }

            return View("~/Plugins/Admin.Accounting/Views/List.cshtml", reconciliationModel);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.ADMIN)]
        public async Task<IActionResult> ViewReconciliationAsync(string key, string dtstr)
        {
            Reconciliation reconciliation = null;

            try
            {
                DateTime from = Convert.ToDateTime(dtstr).AddDays(-1);
                DateTime to = from.AddDays(3);

                var results = await FetchResultAsync(from, to);

                if (results != null && results.Count > 0)
                {
                    var reconciliationList = FetchDeserialized(results, key);
                    var enrichedList = await EnrichReconciliationListAsync(reconciliationList);
                    if (enrichedList != null)
                    {
                        reconciliation = enrichedList.FirstOrDefault().Values.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.ToString());
            }

            return View("~/Plugins/Admin.Accounting/Views/ViewReconciliation.cshtml", reconciliation);
        }

        private async Task<List<Dictionary<string, Reconciliation>>> EnrichReconciliationListAsync(List<Dictionary<string, Reconciliation>> reconciliationList)
        {
            foreach (Dictionary<string, Reconciliation> rec in reconciliationList)
            {
                foreach (KeyValuePair<string, Reconciliation> kvp in rec)
                {
                    await SetAllPresentationNumbers(kvp);
                }
            }
            return reconciliationList;
        }

        private async Task SetAllPresentationNumbers(KeyValuePair<string, Reconciliation> kvp)
        {
            if (kvp.Value.Reconciliations == null)
            {
                return;
            }

            string currencyCode = "";
            foreach (ReconciliationElement recElement in kvp.Value.Reconciliations)
            {
                currencyCode = await SetPresentationNumbers(kvp, recElement);
            }

            kvp.Value.ReconciliationTimePresentation = UnixTimeStampToDateTime((long)kvp.Value.ReconciliationTime);
            kvp.Value.GrandTotalPresentation = Utilities.PresentationPrice((decimal)kvp.Value.RegisterSummary.All.Total, currencyCode);
        }

        private async Task<string> SetPresentationNumbers(KeyValuePair<string, Reconciliation> kvp, ReconciliationElement recElement)
        {
            if (recElement.PaymentTypeIdentifier == "cash")
            {
                if (kvp.Value.RegisterSummary?.Sales != null)
                {
                    decimal cashSale = await GetCashSaleAsync(kvp.Value.RegisterSummary.Sales);
                    kvp.Value.TotalCashSalePresentation = Utilities.PresentationPrice(cashSale);
                }

                kvp.Value.TotalCashPresentation = Utilities.PresentationPrice((decimal)recElement.Total);
                kvp.Value.TotalCashCountedPresentation = Utilities.PresentationPrice((decimal)recElement.Counted);
            }
            else
            {
                if (kvp.Value.RegisterSummary?.Sales != null)
                {
                    recElement.TotalSalePresentation = Utilities.PresentationPrice((decimal)recElement.Total);
                    recElement.TotalCountedPresentation = Utilities.PresentationPrice((decimal)recElement.Counted);
                }
            }

            return recElement.CurrencyCode;
        }

        private async Task<decimal> GetCashSaleAsync(All sales)
        {
            decimal cashPayment = 0;

            try
            {
                if (sales != null)
                {
                    foreach (Payment payment in sales?.Payments)
                    {
                        if (payment.Type == "cash" && payment.Totals != null)
                        {
                            cashPayment = (decimal)payment.Totals.Total;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.ToString());
            }

            return cashPayment;
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private static List<Dictionary<string, Reconciliation>> FetchDeserialized(IList<string> results, string key = "")
        {
            var reconciliationList = new List<Dictionary<string, Reconciliation>>();
            foreach (var r in results)
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, Reconciliation>>(r);

                if (string.IsNullOrEmpty(key))
                {
                    reconciliationList.Add(result);
                }
                else if (result.ContainsKey(key))
                {
                    reconciliationList.Add(result);
                    return reconciliationList;
                }
            }

            return reconciliationList;
        }

        private async Task<List<Dictionary<string, Reconciliation>>> FetchDeserializedAsync(IList<string> results)
        {
            var reconciliationList = new List<Dictionary<string, Reconciliation>>();

            foreach (var r in results)
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, Reconciliation>>(r);
                    reconciliationList.Add(result);
                }
                catch (Exception ex)
                {
                    var message = $"Error deserializing reconciliation file, maybe its not a valid file and should just be deleted:{Environment.NewLine}{ex.Message}{Environment.NewLine}{Environment.NewLine}{r}";
                    await _logger.ErrorAsync(message, ex);
                }
            }

            return reconciliationList;
        }

        private async Task<IList<string>> FetchResultAsync(DateTime from, DateTime? to = null)
        {
            if (to.HasValue == false)
            {
                to = DateTime.Now;
            }

            try
            {
                var jsonFiles = await _logService.GetRawJsonFileAsync(from, to.Value);
                List<FileInfo> orderedList = jsonFiles.OrderByDescending(x => x.CreationTime).ToList();
                IList<string> results = new List<string>();
                foreach (var jsonFile in orderedList)
                {
                    results.Add(System.IO.File.ReadAllText(jsonFile.FullName));
                }
                return results;
            }
            catch (FileNotFoundException ex)
            {
                await _logger.ErrorAsync(ex.ToString());
            }
            return null;
        }
    }
}