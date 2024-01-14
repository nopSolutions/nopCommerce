using AO.Services.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AO.Services.Emails
{
    public class AOMessageTokenProvider : IAOMessageTokenProvider
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly ICurrencyService _currencyService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly MessageTemplatesSettings _templatesSettings;

        public AOMessageTokenProvider(CatalogSettings catalogSettings,
            ICurrencyService currencyService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            MessageTemplatesSettings templatesSettings)
        {
            _catalogSettings = catalogSettings;
            _currencyService = currencyService;
            _languageService = languageService;
            _localizationService = localizationService;
            _orderService = orderService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _templatesSettings = templatesSettings;
        }

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="vendorId">Vendor identifier (used to limit products by vendor</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the hTML table of products
        /// </returns>
        public async Task<string> ProductListToHtmlTableAsync(Order order, AOInvoice invoice, List<AOInvoiceItem> invoiceItems, int languageId)
        {
            var language = await _languageService.GetLanguageByIdAsync(languageId);

            var sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");

            sb.AppendLine($"<tr style=\"background-color:{_templatesSettings.Color1};text-align:center;\">");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Name", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Price", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Quantity", languageId)}</th>");
            sb.AppendLine($"<th>{await _localizationService.GetResourceAsync("Messages.Order.Product(s).Total", languageId)}</th>");
            sb.AppendLine("</tr>");

            var table = invoiceItems;
            for (var i = 0; i <= table.Count - 1; i++)
            {
                var invoiceItem = table[i];

                var orderItem = await _orderService.GetOrderItemByIdAsync(invoiceItem.OrderItemId);
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                if (product == null)
                    continue;

                sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: center;\">");
                //product name
                var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name, languageId);

                sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + WebUtility.HtmlEncode(productName));

                //attributes
                if (!string.IsNullOrEmpty(orderItem.AttributeDescription))
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine(orderItem.AttributeDescription);
                }

                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : string.Empty;
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : string.Empty;
                    var rentalInfo = string.Format(await _localizationService.GetResourceAsync("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                    sb.AppendLine("<br />");
                    sb.AppendLine(rentalInfo);
                }

                sb.AppendLine("</td>");

                string unitPriceStr;
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    unitPriceStr = await _priceFormatter.FormatPriceAsync(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    unitPriceStr = await _priceFormatter.FormatPriceAsync(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                }

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: right;\">{unitPriceStr.Replace(" ", "&nbsp;")}</td>");

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: center;\">{orderItem.Quantity}</td>");

                string priceStr;
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency((orderItem.UnitPriceInclTax * orderItem.Quantity), order.CurrencyRate);
                    priceStr = await _priceFormatter.FormatPriceAsync(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency((orderItem.UnitPriceExclTax * orderItem.Quantity), order.CurrencyRate);
                    priceStr = await _priceFormatter.FormatPriceAsync(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                }

                sb.AppendLine($"<td style=\"padding: 0.6em 0.4em;text-align: right;\">{priceStr.Replace(" ", "&nbsp;")}</td>");

                sb.AppendLine("</tr>");
            }

            // Totals, tax, shipping all together

            // Tax
            sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: right;\">");
            string totalShippingStr = await _priceFormatter.FormatPriceAsync(invoice.InvoiceShipping, true, order.CustomerCurrencyCode, languageId, false);
            sb.AppendLine($"<td></td><td>{await _localizationService.GetResourceAsync("Messages.Order.Shipping", languageId)}</td><td colspan='2' style=\"padding: 0.6em 0.4em;text-align: right;white-space: nowrap;\">" + totalShippingStr.Replace(" ", "&nbsp;") + "</td>");
            sb.AppendLine("</tr>");

            // Tax
            sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: right;\">");
            string totalTaxStr = await _priceFormatter.FormatPriceAsync(invoice.InvoiceTax, true, order.CustomerCurrencyCode, languageId, false);
            sb.AppendLine($"<td></td><td>{await _localizationService.GetResourceAsync("Messages.Order.Tax", languageId)}</td><td colspan='2' style=\"padding: 0.6em 0.4em;text-align: right;white-space: nowrap;\">" + totalTaxStr.Replace(" ", "&nbsp;") + "</td>");
            sb.AppendLine("</tr>");

            // Total
            sb.AppendLine($"<tr style=\"background-color: {_templatesSettings.Color2};text-align: right;\">");
            string totalPriceStr = await _priceFormatter.FormatPriceAsync(invoice.InvoiceTotal, true, order.CustomerCurrencyCode, languageId, false);
            sb.AppendLine($"<td></td><td>{await _localizationService.GetResourceAsync("Nop.Plugin.Admin.OrderManagementList.InvoiceTotal", languageId)}:</td><td colspan='2' style=\"padding: 0.6em 0.4em;text-align: right;white-space: nowrap;\">" + totalPriceStr.Replace(" ", "&nbsp;") + "</td>");
            sb.AppendLine("</tr>");

            sb.AppendLine("</table>");
            var result = sb.ToString();
            return result;
        }
    }
}