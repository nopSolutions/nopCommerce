using AO.Services;
using AO.Services.Emails;
using AO.Services.Services;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.POS.Kaching.Models.ReconciliationModels;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.POS.Kaching.Services
{
    public class ReconciliationService : IReconciliationService
    {
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly POSKachingSettings _kachingSettings;
        private readonly ILogService _logService;
        private readonly string _currencyCode;
        private readonly IMessageService _messageService;
        private readonly IAOInvoiceService _aOInvoiceService;
        private readonly IOrderService _orderService;

        public ReconciliationService(ILogger logger,
                                    POSKachingSettings kachingSettings,
                                    IWorkContext workContext,
                                    IMessageService messageService,
                                    ILogService logService,
                                    IAOInvoiceService aOInvoiceService,
                                    IOrderService orderService)
        {
            _logger = logger;
            _kachingSettings = kachingSettings;
            _workContext = workContext;
            _messageService = messageService;
            _logService = logService;
            _currencyCode = _workContext.GetWorkingCurrencyAsync().Result.CurrencyCode;
            _aOInvoiceService = aOInvoiceService;
            _orderService = orderService;
        }

        public async Task<bool> HandleReconciliationsAsync(Dictionary<string, Reconciliation> reconciliation, string accountingFileName)
        {
            var sb = new StringBuilder();
            decimal totalSales = 0, totalDiscounts = 0, totalTax = 0;
            double reconciliationTime = 0;

            foreach (KeyValuePair<string, Reconciliation> kvp in reconciliation)
            {
                if (kvp.Value.Reconciliations == null)
                {
                    // Not a reconciliation call
                    return false;
                }

                reconciliationTime = kvp.Value.ReconciliationTime;

                sb.Append("Reconciliation with key: " + kvp.Key + " received from Kaching" + Environment.NewLine + Environment.NewLine);
                sb.Append("Register name: " + kvp.Value.Source.RegisterName + Environment.NewLine);
                sb.Append("Shop name: " + kvp.Value.Source.ShopName + Environment.NewLine);
                sb.Append("Cashier name: " + kvp.Value.Source.CashierName + Environment.NewLine);
                sb.Append("<hr />" + Environment.NewLine);

                foreach (ReconciliationElement recElem in kvp.Value.Reconciliations)
                {
                    if (recElem.Total != recElem.Counted)
                    {
                        sb.Append("<span style='color: red;'>");
                        sb.Append("-");
                        sb.Append("</span>");
                    }

                    sb.Append("<table style='width: 400px'><tr><td colspan='2'><b>Payment type: " + recElem.PaymentTypeIdentifier + Environment.NewLine + "</b></td></tr>");

                    if (recElem.PaymentTypeIdentifier == "cash")
                    {
                        if (kvp.Value.RegisterSummary?.Sales != null)
                        {
                            decimal cashSale = await GetCashSaleAsync(kvp.Value.RegisterSummary.Sales);
                            sb.Append("<tr><td style='width:200px;'>Total sale:</td><td>" + Utilities.PresentationPrice(cashSale, _currencyCode) + "</td></tr>");
                        }
                    }

                    sb.Append("<tr><td style='width:200px;'>Total:</td><td>" + Utilities.PresentationPrice((decimal)recElem.Total, _currencyCode) + "</td></tr>");
                    sb.Append("<tr><td>Counted:</td><td>" + Utilities.PresentationPrice((decimal)recElem.Counted, _currencyCode) + "</td></tr>");


                    if (recElem.DepositedAmount != null)
                    {
                        sb.Append("<tr><td>Deposited amount:</td><td>" + Utilities.PresentationPrice((decimal)recElem.DepositedAmount, _currencyCode) + "</td></tr>");
                    }
                    if (recElem.Depositing != null)
                    {
                        sb.Append("<tr><td>Depositing:</td><td>" + recElem.Depositing + "</td></tr>");
                    }
                    if (recElem.CurrencyCode != null)
                    {
                        sb.Append("<tr><td>Currency code:</td><td>" + recElem.CurrencyCode + "</td></tr>");
                    }

                    sb.Append("<tr><td>Should be reconciled:</td><td>" + recElem.ShouldBeReconciled + "</td></tr>");

                    sb.Append("</table>");
                    sb.Append("<hr /><br /><br /><hr />");
                }

                sb.Append("<table style='width: 480px'><tr><td colspan='2'><b>Summary all</b></td></tr>");
                sb.Append("<tr><td>Total:</td><td><b>" + Utilities.PresentationPrice((decimal)kvp.Value.RegisterSummary.All.Total, _currencyCode) + "</b></td></tr>");
                sb.Append("<tr><td>Total discounts:</td><td>" + Utilities.PresentationPrice(kvp.Value.RegisterSummary.All.TotalDiscounts, _currencyCode) + "</td></tr>");
                sb.Append("<tr><td>Total tax amount:</td><td>" + Utilities.PresentationPrice((decimal)kvp.Value.RegisterSummary.All.TotalTaxAmount, _currencyCode) + "</td></tr>");
                sb.Append("<tr><td>Total sales:</td><td>" + kvp.Value.RegisterSummary.All.Count + "</td></tr>");
                sb.Append("<tr><td colspan='2'><hr /></td></tr>");
                sb.Append("<tr><td>Number of aborted sales:</td><td>" + kvp.Value.RegisterSummary.NumberOfAbortedSales + "</td></tr>");
                sb.Append("<tr><td>Number of initiated sales:</td><td>" + kvp.Value.RegisterSummary.NumberOfInitiatedSales + "</td></tr>");
                sb.Append("<tr><td>Cash drawer open count:</td><td>" + kvp.Value.RegisterSummary.CashDrawerOpenCount + "</td></tr>");
                DateTime registerOpenedAt = UnixTimeStampToDateTime(kvp.Value.RegisterSummary.OpenedAt);
                sb.Append("<tr><td>Cash drawer opened at:</td><td>" + registerOpenedAt.ToShortDateString() + " <b>" + registerOpenedAt.ToShortTimeString() + "</b></td></tr>");
                sb.Append("<tr><td colspan='2'><hr /><b>Returns</b></td></tr>");
                sb.Append("<tr><td>Total returns:</td><td>" + kvp.Value.RegisterSummary.Returns.Total + "</td></tr>");
                sb.Append("<tr><td>Subtotal returns:</td><td>" + kvp.Value.RegisterSummary.Returns.SubTotal + "</td></tr>");
                sb.Append("<tr><td>Taxamount returns:</td><td>" + kvp.Value.RegisterSummary.Returns.TotalTaxAmount + "</td></tr>");
                sb.Append("<tr><td>Discounts returns:</td><td>" + kvp.Value.RegisterSummary.Returns.TotalDiscounts + "</td></tr>");
                sb.Append("<tr><td colspan='2'><hr /></td></tr>");
                sb.Append("</table>");

                if (!string.IsNullOrEmpty(kvp.Value.Comment))
                {
                    sb.Append("<br /><br />Comment:");
                    sb.Append("<span style='color: red; font-size: 12px;'>");
                    sb.Append("<br />");
                    sb.Append(kvp.Value.Comment);
                    sb.Append("</span>");
                }

                totalSales += (decimal)kvp.Value.RegisterSummary.All.Total;
                totalDiscounts += kvp.Value.RegisterSummary.All.TotalDiscounts;
                totalTax += (decimal)kvp.Value.RegisterSummary.All.TotalTaxAmount;
            }

            _logService.RenameFile(accountingFileName, reconciliationTime);

            try
            {
                await _messageService.QueueAdminEmailAsync(_kachingSettings.POSKaChingReconciliationMailAddresses, _kachingSettings.POSKaChingReconciliationMailName, sb.ToString());
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            try
            {
                await GenerateInvoiceAsync(totalSales, totalDiscounts, totalTax);
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync(ex.Message, ex);
            }

            return true;
        }

        private async Task<decimal> GetCashSaleAsync(All sales)
        {
            decimal cashPayment = 0;

            try
            {
                if (sales != null)
                {
                    foreach (Models.ReconciliationModels.Payment payment in sales?.Payments)
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

        private async Task GenerateInvoiceAsync(decimal totalSales, decimal totalDiscounts, decimal totalTax)
        {
            var order = new Order()
            {
                OrderTotal = totalSales,
                OrderDiscount = totalDiscounts,
                OrderTax = totalTax,
                CustomOrderNumber = string.Empty,
                BillingAddressId = 2,
                ShippingAddressId = 2,
                OrderStatusId = 30, // Complete
                PaidDateUtc = DateTime.UtcNow,
                CustomerId = 1,
                PaymentMethodSystemName = "Kasselukning",
                CustomerCurrencyCode = "DKK"                
            };

            await _orderService.InsertOrderAsync(order);

            var orderItems = new List<OrderItem>()
            {
                new OrderItem()
                {
                     Quantity = 1,
                     PriceInclTax = totalSales,
                     UnitPriceInclTax = totalSales,
                     UnitPriceExclTax = totalSales * (decimal)0.8,
                     OrderId = order.Id,
                     ProductId = _kachingSettings.ReconciliationInvoiceProductId
                }
            };
            await _orderService.InsertOrderItemAsync(orderItems[0]);

            var invoice = await _aOInvoiceService.CreateInvoiceAsync(orderItems, order, false, false, DateTime.UtcNow, DateTime.UtcNow, true, false);

        }
    }
}