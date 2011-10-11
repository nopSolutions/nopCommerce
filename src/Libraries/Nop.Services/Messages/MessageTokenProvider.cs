using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Html;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;

namespace Nop.Services.Messages
{
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IWebHelper _webHelper;

        private readonly StoreInformationSettings _storeSettings;
        private readonly MessageTemplatesSettings _templatesSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public MessageTokenProvider(ILanguageService languageService,
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            IPriceFormatter priceFormatter, ICurrencyService currencyService,IWebHelper webHelper,
            StoreInformationSettings storeSettings, MessageTemplatesSettings templatesSettings,
            EmailAccountSettings emailAccountSettings, CatalogSettings catalogSettings,
            TaxSettings taxSettings)
        {
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._emailAccountService = emailAccountService;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._webHelper = webHelper;

            this._storeSettings = storeSettings;
            this._templatesSettings = templatesSettings;
            this._emailAccountSettings = emailAccountSettings;
            this._catalogSettings = catalogSettings;
            this._taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>HTML table of products</returns>
        protected virtual string ProductListToHtmlTable(Order order, int languageId)
        {
            var result = "";

            var language = _languageService.GetLanguageById(languageId);

            var sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");

            #region Products
            sb.AppendLine(string.Format("<tr style=\"background-color:{0};text-align:center;\">", _templatesSettings.Color1));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Messages.Order.Product(s).Name", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Messages.Order.Product(s).Price", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Messages.Order.Product(s).Quantity", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", _localizationService.GetResource("Messages.Order.Product(s).Total", languageId)));
            sb.AppendLine("</tr>");

            var table = order.OrderProductVariants.ToList();
            for (int i = 0; i <= table.Count - 1; i++)
            {
                var opv = table[i];
                var productVariant = opv.ProductVariant;
                if (productVariant == null)
                    continue;

                sb.AppendLine(string.Format("<tr style=\"background-color: {0};text-align: center;\">", _templatesSettings.Color2));
                //product name
                string productName = "";
                //product name
                if (!String.IsNullOrEmpty(opv.ProductVariant.GetLocalized(x => x.Name, languageId)))
                    productName = string.Format("{0} ({1})", opv.ProductVariant.Product.GetLocalized(x => x.Name, languageId), opv.ProductVariant.GetLocalized(x => x.Name, languageId));
                else
                    productName = opv.ProductVariant.Product.GetLocalized(x => x.Name, languageId);

                sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + HttpUtility.HtmlEncode(productName));
                //TODO add download link
                //if (IoC.Resolve<IOrderService>().IsDownloadAllowed(opv))
                //{
                //    string downloadUrl = string.Format("<a class=\"link\" href=\"{0}\" >{1}</a>", IoC.Resolve<IDownloadService>().GetDownloadUrl(opv), _localizationService.GetResource("Messages.Order.Products(s).Download", languageId));
                //    sb.AppendLine("&nbsp;&nbsp;(");
                //    sb.AppendLine(downloadUrl);
                //    sb.AppendLine(")");
                //}
                //attributes
                if (!String.IsNullOrEmpty(opv.AttributeDescription))
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine(opv.AttributeDescription);
                }
                //sku
                if (_catalogSettings.ShowProductSku)
                {
                    if (!String.IsNullOrEmpty(opv.ProductVariant.Sku))
                    {
                        sb.AppendLine("<br />");
                        string sku = string.Format(_localizationService.GetResource("Messages.Order.Product(s).SKU", languageId), HttpUtility.HtmlEncode(opv.ProductVariant.Sku));
                        sb.AppendLine(sku);
                    }
                }
                sb.AppendLine("</td>");

                string unitPriceStr = string.Empty;
                switch (order.CustomerTaxDisplayType)
                {
                    case TaxDisplayType.ExcludingTax:
                        {
                            var opvUnitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(opv.UnitPriceExclTax, order.CurrencyRate);
                            unitPriceStr = _priceFormatter.FormatPrice(opvUnitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                        }
                        break;
                    case TaxDisplayType.IncludingTax:
                        {
                            var opvUnitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(opv.UnitPriceInclTax, order.CurrencyRate);
                            unitPriceStr = _priceFormatter.FormatPrice(opvUnitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                        }
                        break;
                }
                sb.AppendLine(string.Format("<td style=\"padding: 0.6em 0.4em;text-align: right;\">{0}</td>", unitPriceStr));

                sb.AppendLine(string.Format("<td style=\"padding: 0.6em 0.4em;text-align: center;\">{0}</td>", opv.Quantity));

                string priceStr = string.Empty;
                switch (order.CustomerTaxDisplayType)
                {
                    case TaxDisplayType.ExcludingTax:
                        {
                            var opvPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(opv.PriceExclTax, order.CurrencyRate);
                            priceStr = _priceFormatter.FormatPrice(opvPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                        }
                        break;
                    case TaxDisplayType.IncludingTax:
                        {
                            var opvPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(opv.PriceInclTax, order.CurrencyRate);
                            priceStr = _priceFormatter.FormatPrice(opvPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                        }
                        break;
                }
                sb.AppendLine(string.Format("<td style=\"padding: 0.6em 0.4em;text-align: right;\">{0}</td>", priceStr));

                sb.AppendLine("</tr>");
            }
            #endregion

            #region Checkout Attributes

            if (!String.IsNullOrEmpty(order.CheckoutAttributeDescription))
            {
                sb.AppendLine("<tr><td style=\"text-align:right;\" colspan=\"1\">&nbsp;</td><td colspan=\"3\" style=\"text-align:right\">");
                sb.AppendLine(order.CheckoutAttributeDescription);
                sb.AppendLine("</td></tr>");
            }

            #endregion

            #region Totals

            string cusSubTotal = string.Empty;
            bool dislaySubTotalDiscount = false;
            string cusSubTotalDiscount = string.Empty;
            string cusShipTotal = string.Empty;
            string cusPaymentMethodAdditionalFee = string.Empty;
            var taxRates = new SortedDictionary<decimal, decimal>();
            string cusTaxTotal = string.Empty;
            string cusDiscount = string.Empty;
            string cusTotal = string.Empty;
            //subtotal, shipping, payment method fee
            switch (order.CustomerTaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    {
                        //subtotal
                        var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                        cusSubTotal = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                        //discount (applied to order subtotal)
                        var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                        if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                        {
                            cusSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                            dislaySubTotalDiscount = true;
                        }
                        //shipping
                        var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                        cusShipTotal = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                        //payment method additional fee
                        var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                        cusPaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                    }
                    break;
                case TaxDisplayType.IncludingTax:
                    {
                        //subtotal
                        var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                        cusSubTotal = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                        //discount (applied to order subtotal)
                        var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                        if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                        {
                            cusSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                            dislaySubTotalDiscount = true;
                        }
                        //shipping
                        var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                        cusShipTotal = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                        //payment method additional fee
                        var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                        cusPaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                    }
                    break;
            }

            //shipping
            bool dislayShipping = order.ShippingStatus != ShippingStatus.ShippingNotRequired;

            //payment method fee
            bool displayPaymentMethodFee = true;
            if (order.PaymentMethodAdditionalFeeExclTax == decimal.Zero)
            {
                displayPaymentMethodFee = false;
            }

            //tax
            bool displayTax = true;
            bool displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    taxRates = new SortedDictionary<decimal, decimal>();
                    foreach (var tr in order.TaxRatesDictionary)
                        taxRates.Add(tr.Key, _currencyService.ConvertCurrency(tr.Value, order.CurrencyRate));

                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Count > 0;
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    string taxStr = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                    cusTaxTotal = taxStr;
                }
            }

            //discount
            bool dislayDiscount = false;
            if (order.OrderDiscount > decimal.Zero)
            {
                var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
                cusDiscount = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                dislayDiscount = true;
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            //TODO pass languageId to _priceFormatter.FormatPrice
            cusTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false);




            //subtotal
            sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Order.SubTotal", languageId), cusSubTotal));

            //discount (applied to order subtotal)
            if (dislaySubTotalDiscount)
            {
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Order.SubTotalDiscount", languageId), cusSubTotalDiscount));
            }


            //shipping
            if (dislayShipping)
            {
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Order.Shipping", languageId), cusShipTotal));
            }

            //payment method fee
            if (displayPaymentMethodFee)
            {
                string paymentMethodFeeTitle = _localizationService.GetResource("Messages.Order.PaymentMethodAdditionalFee", languageId);
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, paymentMethodFeeTitle, cusPaymentMethodAdditionalFee));
            }

            //tax
            if (displayTax)
            {
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Order.Tax", languageId), cusTaxTotal));
            }
            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    string taxRate = String.Format(_localizationService.GetResource("Messages.Order.TaxRateLine"), _priceFormatter.FormatTaxRate(item.Key));
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    string taxValue = _priceFormatter.FormatPrice(item.Value, true, order.CustomerCurrencyCode, false);
                    sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, taxRate, taxValue));
                }
            }

            //discount
            if (dislayDiscount)
            {
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Order.TotalDiscount", languageId), cusDiscount));
            }

            //gift cards
            var gcuhC = order.GiftCardUsageHistory;
            foreach (var gcuh in gcuhC)
            {
                string giftCardText = String.Format(_localizationService.GetResource("Messages.Order.GiftCardInfo", languageId), HttpUtility.HtmlEncode(gcuh.GiftCard.GiftCardCouponCode));
                string giftCardAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false);
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, giftCardText, giftCardAmount));
            }

            //reward points
            if (order.RedeemedRewardPointsEntry != null)
            {
                string rpTitle = string.Format(_localizationService.GetResource("Messages.Order.RewardPoints", languageId), -order.RedeemedRewardPointsEntry.Points);
                string rpAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(order.RedeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false);
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, rpTitle, rpAmount));
            }

            //total
            sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", _templatesSettings.Color3, _localizationService.GetResource("Messages.Order.OrderTotal", languageId), cusTotal));
            #endregion

            sb.AppendLine("</table>");
            result = sb.ToString();
            return result;
        }

        #endregion

        #region Methods

        public virtual void AddStoreTokens(IList<Token> tokens)
        {
            tokens.Add(new Token("Store.Name", _storeSettings.StoreName));
            tokens.Add(new Token("Store.URL", _storeSettings.StoreUrl));
            var defaultEmailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
            if (defaultEmailAccount == null)
                defaultEmailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            tokens.Add(new Token("Store.Email", defaultEmailAccount.Email));
        }

        public virtual void AddOrderTokens(IList<Token> tokens, Order order, int languageId)
        {
            tokens.Add(new Token("Order.OrderNumber", order.Id.ToString()));

            tokens.Add(new Token("Order.CustomerFullName", HttpUtility.HtmlEncode(string.Format("{0} {1}", order.BillingAddress.FirstName, order.BillingAddress.LastName))));
            tokens.Add(new Token("Order.CustomerEmail", HttpUtility.HtmlEncode(order.BillingAddress.Email)));


            tokens.Add(new Token("Order.BillingFirstName", HttpUtility.HtmlEncode(order.BillingAddress.FirstName)));
            tokens.Add(new Token("Order.BillingLastName", HttpUtility.HtmlEncode(order.BillingAddress.LastName)));
            tokens.Add(new Token("Order.BillingPhoneNumber", HttpUtility.HtmlEncode(order.BillingAddress.PhoneNumber)));
            tokens.Add(new Token("Order.BillingEmail", HttpUtility.HtmlEncode(order.BillingAddress.Email)));
            tokens.Add(new Token("Order.BillingFaxNumber", HttpUtility.HtmlEncode(order.BillingAddress.FaxNumber)));
            tokens.Add(new Token("Order.BillingCompany", HttpUtility.HtmlEncode(order.BillingAddress.Company)));
            tokens.Add(new Token("Order.BillingAddress1", HttpUtility.HtmlEncode(order.BillingAddress.Address1)));
            tokens.Add(new Token("Order.BillingAddress2", HttpUtility.HtmlEncode(order.BillingAddress.Address2)));
            tokens.Add(new Token("Order.BillingCity", HttpUtility.HtmlEncode(order.BillingAddress.City)));
            tokens.Add(new Token("Order.BillingStateProvince", order.BillingAddress.StateProvince != null ? HttpUtility.HtmlEncode(order.BillingAddress.StateProvince.Name) : ""));
            tokens.Add(new Token("Order.BillingZipPostalCode", HttpUtility.HtmlEncode(order.BillingAddress.ZipPostalCode)));
            tokens.Add(new Token("Order.BillingCountry", order.BillingAddress.Country!=null ? HttpUtility.HtmlEncode(order.BillingAddress.Country.Name) : ""));

            tokens.Add(new Token("Order.ShippingMethod", HttpUtility.HtmlEncode(order.ShippingMethod)));
            tokens.Add(new Token("Order.ShippingFirstName", order.ShippingAddress != null ? HttpUtility.HtmlEncode(order.ShippingAddress.FirstName) : ""));
            tokens.Add(new Token("Order.ShippingLastName", order.ShippingAddress != null ? HttpUtility.HtmlEncode(order.ShippingAddress.LastName) : ""));
            tokens.Add(new Token("Order.ShippingPhoneNumber", order.ShippingAddress != null ? HttpUtility.HtmlEncode(order.ShippingAddress.PhoneNumber) : ""));
            tokens.Add(new Token("Order.ShippingEmail", order.ShippingAddress != null ? HttpUtility.HtmlEncode(order.ShippingAddress.Email) : ""));
            tokens.Add(new Token("Order.ShippingFaxNumber", order.ShippingAddress != null ? HttpUtility.HtmlEncode(order.ShippingAddress.FaxNumber) : ""));
            tokens.Add(new Token("Order.ShippingCompany", order.ShippingAddress != null ? HttpUtility.HtmlEncode(order.ShippingAddress.Company) : ""));
            tokens.Add(new Token("Order.ShippingAddress1", order.ShippingAddress != null ? HttpUtility.HtmlEncode(order.ShippingAddress.Address1) : ""));
            tokens.Add(new Token("Order.ShippingAddress2", order.ShippingAddress != null ? HttpUtility.HtmlEncode(order.ShippingAddress.Address2) : ""));
            tokens.Add(new Token("Order.ShippingCity", order.ShippingAddress != null ? HttpUtility.HtmlEncode(order.ShippingAddress.City) : ""));
            tokens.Add(new Token("Order.ShippingStateProvince", order.ShippingAddress != null && order.ShippingAddress.StateProvince != null ? HttpUtility.HtmlEncode(order.ShippingAddress.StateProvince.Name) : ""));
            tokens.Add(new Token("Order.ShippingZipPostalCode", order.ShippingAddress != null ? HttpUtility.HtmlEncode(order.ShippingAddress.ZipPostalCode) : ""));
            tokens.Add(new Token("Order.ShippingCountry", order.ShippingAddress != null && order.ShippingAddress.Country != null ? HttpUtility.HtmlEncode(order.ShippingAddress.Country.Name) : ""));


            tokens.Add(new Token("Order.TrackingNumber", HttpUtility.HtmlEncode(order.TrackingNumber)));
            tokens.Add(new Token("Order.VatNumber", HttpUtility.HtmlEncode(order.VatNumber)));

            tokens.Add(new Token("Order.Product(s)", ProductListToHtmlTable(order, languageId)));

            var language = _languageService.GetLanguageById(languageId);
            if (language != null && !String.IsNullOrEmpty(language.LanguageCulture))
            {
                DateTime createdOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, TimeZoneInfo.Utc, _dateTimeHelper.GetCustomerTimeZone(order.Customer));
                tokens.Add(new Token("Order.CreatedOn", createdOn.ToString("D", new CultureInfo(language.LanguageCulture))));
            }
            else
            {
                tokens.Add(new Token("Order.CreatedOn", order.CreatedOnUtc.ToString("D")));
            }

            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            tokens.Add(new Token("Order.OrderURLForCustomer", string.Format("{0}orderdetails/{1}", _webHelper.GetStoreLocation(false), order.Id)));
        }

        public virtual void AddReturnRequestTokens(IList<Token> tokens, ReturnRequest returnRequest, OrderProductVariant opv)
        {

            tokens.Add(new Token("ReturnRequest.ID", returnRequest.Id.ToString()));
            tokens.Add(new Token("ReturnRequest.Product.Quantity", returnRequest.Quantity.ToString()));
            tokens.Add(new Token("ReturnRequest.Product.Name", HttpUtility.HtmlEncode(opv.ProductVariant.FullProductName)));
            tokens.Add(new Token("ReturnRequest.Reason", HttpUtility.HtmlEncode(returnRequest.ReasonForReturn)));
            tokens.Add(new Token("ReturnRequest.RequestedAction", HttpUtility.HtmlEncode(returnRequest.RequestedAction)));
            tokens.Add(new Token("ReturnRequest.CustomerComment", HtmlHelper.FormatText(returnRequest.CustomerComments, false, true, false, false, false, false)));
        }

        public virtual void AddGiftCardTokens(IList<Token> tokens, GiftCard giftCard)
        {
            tokens.Add(new Token("GiftCard.SenderName", HttpUtility.HtmlEncode(giftCard.SenderName)));
            tokens.Add(new Token("GiftCard.SenderEmail",HttpUtility.HtmlEncode(giftCard.SenderEmail)));
            tokens.Add(new Token("GiftCard.RecipientName", HttpUtility.HtmlEncode(giftCard.RecipientName)));
            tokens.Add(new Token("GiftCard.RecipientEmail", HttpUtility.HtmlEncode(giftCard.RecipientEmail)));
            tokens.Add(new Token("GiftCard.Amount", HttpUtility.HtmlEncode(_priceFormatter.FormatPrice(giftCard.Amount, true, false))));
            tokens.Add(new Token("GiftCard.CouponCode", HttpUtility.HtmlEncode(giftCard.GiftCardCouponCode)));

            var giftCardMesage = !String.IsNullOrWhiteSpace(giftCard.Message) ? 
                HtmlHelper.FormatText(giftCard.Message, false, true, false, false, false, false) : "";

            tokens.Add(new Token("GiftCard.Message", giftCardMesage));
        }

        public virtual void AddCustomerTokens(IList<Token> tokens, Customer customer)
        {
            tokens.Add(new Token("Customer.Email", HttpUtility.HtmlEncode(customer.Email)));
            tokens.Add(new Token("Customer.Username", HttpUtility.HtmlEncode(customer.Username)));
            tokens.Add(new Token("Customer.FullName", HttpUtility.HtmlEncode(customer.GetFullName())));
            tokens.Add(new Token("Customer.VatNumber", HttpUtility.HtmlEncode(customer.VatNumber)));
            tokens.Add(new Token("Customer.VatNumberStatus", HttpUtility.HtmlEncode(customer.VatNumberStatus.ToString())));


            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            tokens.Add(new Token("Customer.PasswordRecoveryURL", string.Format("{0}passwordrecovery/confirm?token={1}&email={2}", _webHelper.GetStoreLocation(false), customer.GetAttribute<string>(SystemCustomerAttributeNames.PasswordRecoveryToken), customer.Email)));
            tokens.Add(new Token("Customer.AccountActivationURL", string.Format("{0}customer/activation?token={1}&email={2}", _webHelper.GetStoreLocation(false), customer.GetAttribute<string>(SystemCustomerAttributeNames.AccountActivationToken), customer.Email)));
            tokens.Add(new Token("Wishlist.URLForCustomer", string.Format("{0}wishlist/{1}", _webHelper.GetStoreLocation(false), customer.CustomerGuid)));
        }

        public virtual void AddNewsLetterSubscriptionTokens(IList<Token> tokens, NewsLetterSubscription subscription)
        {
            tokens.Add(new Token("NewsLetterSubscription.Email", subscription.Email));


            const string urlFormat = "{0}newsletter/subscriptionactivation/{1}/{2}";

            var activationUrl = String.Format(urlFormat, _webHelper.GetStoreLocation(false), subscription.NewsLetterSubscriptionGuid, "true");
            tokens.Add(new Token("NewsLetterSubscription.ActivationUrl", activationUrl));

            var deActivationUrl = String.Format(urlFormat, _webHelper.GetStoreLocation(false), subscription.NewsLetterSubscriptionGuid, "false");
            tokens.Add(new Token("NewsLetterSubscription.DeactivationUrl", deActivationUrl));
        }

        public virtual void AddProductReviewTokens(IList<Token> tokens, ProductReview productReview)
        {
            tokens.Add(new Token("ProductReview.ProductName", HttpUtility.HtmlEncode(productReview.Product.Name)));
        }

        public virtual void AddBlogCommentTokens(IList<Token> tokens, BlogComment blogComment)
        {
            tokens.Add(new Token("BlogComment.BlogPostTitle", HttpUtility.HtmlEncode(blogComment.BlogPost.Title)));
        }

        public virtual void AddNewsCommentTokens(IList<Token> tokens, NewsComment newsComment)
        {
            tokens.Add(new Token("NewsComment.NewsTitle", HttpUtility.HtmlEncode(newsComment.NewsItem.Title)));
        }

        public virtual void AddProductTokens(IList<Token> tokens, Product product)
        {
            tokens.Add(new Token("Product.Name", HttpUtility.HtmlEncode(product.Name)));
            tokens.Add(new Token("Product.ShortDescription", product.ShortDescription));

            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            var productUrl = string.Format("{0}p/{1}/{2}", _webHelper.GetStoreLocation(false), product.Id, product.GetSeName());
            tokens.Add(new Token("Product.ProductURLForCustomer", productUrl));
        }

        public virtual void AddProductVariantTokens(IList<Token> tokens, ProductVariant productVariant)
        {
            tokens.Add(new Token("ProductVariant.ID", HttpUtility.HtmlEncode(productVariant.Id)));
            tokens.Add(new Token("ProductVariant.FullProductName", HttpUtility.HtmlEncode(productVariant.FullProductName)));
            tokens.Add(new Token("ProductVariant.StockQuantity", HttpUtility.HtmlEncode(productVariant.StockQuantity)));
        }

        public virtual void AddForumTopicTokens(IList<Token> tokens, ForumTopic forumTopic)
        {
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            var topicUrl = string.Format("{0}boards/topic/{1}/{2}", _webHelper.GetStoreLocation(false), forumTopic.Id, forumTopic.GetSeName());
            tokens.Add(new Token("Forums.TopicURL", topicUrl));
            tokens.Add(new Token("Forums.TopicName", HttpUtility.HtmlEncode(forumTopic.Subject)));
        }

        public virtual void AddForumPostTokens(IList<Token> tokens, ForumPost forumPost)
        {
            tokens.Add(new Token("Forums.PostAuthor", HttpUtility.HtmlEncode(forumPost.Customer.FormatUserName())));
            tokens.Add(new Token("Forums.PostBody", forumPost.FormatPostText()));
        }

        public virtual void AddForumTokens(IList<Token> tokens, Forum forum)
        {
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            var forumUrl = string.Format("{0}boards/forum/{1}/{2}", _webHelper.GetStoreLocation(false), forum.Id, forum.GetSeName());
            tokens.Add(new Token("Forums.ForumURL", forumUrl));
            tokens.Add(new Token("Forums.ForumName", HttpUtility.HtmlEncode(forum.Name)));
        }

        public virtual void AddPrivateMessageTokens(IList<Token> tokens, PrivateMessage privateMessage)
        {
            tokens.Add(new Token("PrivateMessage.Subject", HttpUtility.HtmlEncode(privateMessage.Subject)));
            tokens.Add(new Token("PrivateMessage.Text",  privateMessage.FormatPrivateMessageText()));
        }

        /// <summary>
        /// Gets list of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>List of allowed (supported) message tokens for campaigns</returns>
        public virtual string[] GetListOfCampaignAllowedTokens()
        {
            var allowedTokens = new List<string>()
            {
                "%Store.Name%",
                "%Store.URL%",
                "%Store.Email%",
                "%NewsLetterSubscription.Email%",
                "%NewsLetterSubscription.ActivationUrl%",
                "%NewsLetterSubscription.DeactivationUrl%"
            };
            return allowedTokens.ToArray();
        }

        public virtual string[] GetListOfAllowedTokens()
        {
            var allowedTokens = new List<string>()
            {
                "%Store.Name%",
                "%Store.URL%",
                "%Store.Email%",
                "%Order.OrderNumber%",
                "%Order.CustomerFullName%",
                "%Order.CustomerEmail%",
                "%Order.BillingFirstName%",
                "%Order.BillingLastName%",
                "%Order.BillingPhoneNumber%",
                "%Order.BillingEmail%",
                "%Order.BillingFaxNumber%",
                "%Order.BillingCompany%",
                "%Order.BillingAddress2%",
                "%Order.BillingCity%",
                "%Order.BillingStateProvince%",
                "%Order.BillingZipPostalCode%",
                "%Order.BillingCountry%",
                "%Order.ShippingMethod%",
                "%Order.ShippingFirstName%",
                "%Order.ShippingLastName%",
                "%Order.ShippingPhoneNumber%",
                "%Order.ShippingEmail%",
                "%Order.ShippingFaxNumber%",
                "%Order.ShippingCompany%",
                "%Order.ShippingAddress1%",
                "%Order.ShippingAddress2%",
                "%Order.ShippingCity%",
                "%Order.ShippingStateProvince%",
                "%Order.ShippingZipPostalCode%", 
                "%Order.ShippingCountry%",
                "%Order.TrackingNumber%",
                "%Order.VatNumber%", 
                "%Order.Product(s)%",
                "%Order.CreatedOn%",
                "%Order.OrderURLForCustomer%",
                "%ReturnRequest.ID%", 
                "%ReturnRequest.Product.Quantity%",
                "%ReturnRequest.Product.Name%", 
                "%ReturnRequest.Reason%", 
                "%ReturnRequest.RequestedAction%", 
                "%ReturnRequest.CustomerComment%", 
                "%GiftCard.SenderName%", 
                "%GiftCard.SenderEmail%",
                "%GiftCard.RecipientName%", 
                "%GiftCard.RecipientEmail%", 
                "%GiftCard.Amount%", 
                "%GiftCard.CouponCode%",
                "%GiftCard.Message%",
                "%Customer.Email%", 
                "%Customer.Username%", 
                "%Customer.FullName%", 
                "%Customer.VatNumber%",
                "%Customer.VatNumberStatus%", 
                "%Customer.PasswordRecoveryURL%", 
                "%Customer.AccountActivationURL%", 
                "%Wishlist.URLForCustomer%", 
                "NewsLetterSubscription.Email%", 
                "%NewsLetterSubscription.ActivationUrl%",
                "%NewsLetterSubscription.DeactivationUrl%", 
                "%ProductReview.ProductName%", 
                "%BlogComment.BlogPostTitle%", 
                "%NewsComment.NewsTitle%",
                "%Product.Name%",
                "%Product.ShortDescription%", 
                "%Product.ProductURLForCustomer%",
                "%ProductVariant.ID%", 
                "%ProductVariant.FullProductName%", 
                "%ProductVariant.StockQuantity%", 
                "%Forums.TopicURL%",
                "%Forums.TopicName%", 
                "%Forums.PostAuthor%",
                "%Forums.PostBody%",
                "%Forums.ForumURL%", 
                "%Forums.ForumName%", 
                "%PrivateMessage.Subject%", 
                "%PrivateMessage.Text%"
            };
            return allowedTokens.ToArray();
        }
        
        #endregion
    }
}
