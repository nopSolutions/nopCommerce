//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Messages
{
    /// <summary>
    /// Message service
    /// </summary>
    public partial class MessageService : IMessageService
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        private readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public MessageService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Utilities

        private string Replace(string original, string pattern, string replacement)
        {
            if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("MessageTemplates.CaseInvariantReplacement"))
            {
                int count = 0;
                int position0 = 0;
                int position1 = 0;
                string upperString = original.ToUpper();
                string upperPattern = pattern.ToUpper();
                int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
                char[] chars = new char[original.Length + Math.Max(0, inc)];
                while((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
                {
                    for(int i = position0; i < position1; ++i)
                        chars[count++] = original[i];
                    for(int i = 0; i < replacement.Length; ++i)
                        chars[count++] = replacement[i];
                    position0 = position1 + pattern.Length;
                }
                if(position0 == 0) return original;
                for(int i = position0; i < original.Length; ++i)
                    chars[count++] = original[i];
                return new string(chars, 0, count);
            }
            else
            {
                return original.Replace(pattern, replacement);
            }
        }

        /// <summary>
        /// Convert a collection to a HTML table
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>HTML table of products</returns>
        private string ProductListToHtmlTable(Order order, int languageId)
        {
            string result = string.Empty;
            
            var language = IoC.Resolve<ILanguageService>().GetLanguageById(languageId);
            if (language == null)
            {
                language = NopContext.Current.WorkingLanguage;
                languageId = language.LanguageId;
            }

            var localizationManager = IoC.Resolve<ILocalizationManager>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table border=\"0\" style=\"width:100%;\">");
            string color1 = IoC.Resolve<ISettingManager>().GetSettingValue("MessageTemplate.Color1", "#b9babe");
            string color2 = IoC.Resolve<ISettingManager>().GetSettingValue("MessageTemplate.Color2", "#ebecee");
            string color3 = IoC.Resolve<ISettingManager>().GetSettingValue("MessageTemplate.Color3", "#dde2e6");
           
            #region Products
            sb.AppendLine(string.Format("<tr style=\"background-color:{0};text-align:center;\">", color1));
            sb.AppendLine(string.Format("<th>{0}</th>", localizationManager.GetLocaleResourceString("Order.ProductsGrid.Name", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", localizationManager.GetLocaleResourceString("Order.ProductsGrid.Price", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", localizationManager.GetLocaleResourceString("Order.ProductsGrid.Quantity", languageId)));
            sb.AppendLine(string.Format("<th>{0}</th>", localizationManager.GetLocaleResourceString("Order.ProductsGrid.Total", languageId)));
            sb.AppendLine("</tr>");

            var table = order.OrderProductVariants;
            for (int i = 0; i <= table.Count - 1; i++)
            {
                var opv = table[i];
                var productVariant = opv.ProductVariant;
                if (productVariant == null)
                    continue;

                sb.AppendLine(string.Format("<tr style=\"background-color: {0};text-align: center;\">", color2));
                //product name
                sb.AppendLine("<td style=\"padding: 0.6em 0.4em;text-align: left;\">" + HttpUtility.HtmlEncode(productVariant.GetLocalizedFullProductName(languageId)));
                //download link
                if (IoC.Resolve<IOrderService>().IsDownloadAllowed(opv))
                {
                    string downloadUrl = string.Format("<a class=\"link\" href=\"{0}\" >{1}</a>", IoC.Resolve<IDownloadService>().GetDownloadUrl(opv), localizationManager.GetLocaleResourceString("Order.Download", languageId));
                    sb.AppendLine("&nbsp;&nbsp;(");
                    sb.AppendLine(downloadUrl);
                    sb.AppendLine(")");
                }
                //attributes
                if (!String.IsNullOrEmpty(opv.AttributeDescription))
                {
                    sb.AppendLine("<br />");
                    sb.AppendLine(opv.AttributeDescription);
                }
                //sku
                if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Display.Products.ShowSKU"))
                {
                    if (!String.IsNullOrEmpty(opv.ProductVariant.SKU))
                    {
                        sb.AppendLine("<br />");
                        string sku = string.Format(localizationManager.GetLocaleResourceString("MessageToken.OrderProducts.SKU", languageId), HttpUtility.HtmlEncode(opv.ProductVariant.SKU));
                        sb.AppendLine(sku);
                    }
                }
                sb.AppendLine("</td>");

                string unitPriceStr = string.Empty;
                switch (order.CustomerTaxDisplayType)
                {
                    case TaxDisplayTypeEnum.ExcludingTax:
                        unitPriceStr = PriceHelper.FormatPrice(opv.UnitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                        break;
                    case TaxDisplayTypeEnum.IncludingTax:
                        unitPriceStr = PriceHelper.FormatPrice(opv.UnitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                        break;
                }
                sb.AppendLine(string.Format("<td style=\"padding: 0.6em 0.4em;text-align: right;\">{0}</td>", unitPriceStr));

                sb.AppendLine(string.Format("<td style=\"padding: 0.6em 0.4em;text-align: center;\">{0}</td>",opv.Quantity));

                string priceStr = string.Empty;
                switch (order.CustomerTaxDisplayType)
                {
                    case TaxDisplayTypeEnum.ExcludingTax:
                        priceStr = PriceHelper.FormatPrice(opv.PriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                        break;
                    case TaxDisplayTypeEnum.IncludingTax:
                        priceStr = PriceHelper.FormatPrice(opv.PriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
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
            SortedDictionary<decimal, decimal> taxRates = new SortedDictionary<decimal, decimal>();
            string cusTaxTotal = string.Empty;
            string cusDiscount = string.Empty;
            string cusTotal = string.Empty;
            //subtotal, shipping, payment method fee
            switch (order.CustomerTaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    {
                        //subtotal
                        cusSubTotal = PriceHelper.FormatPrice(order.OrderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                        //discount (applied to order subtotal)
                        if (order.OrderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                        {
                            cusSubTotalDiscount = PriceHelper.FormatPrice(-order.OrderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                            dislaySubTotalDiscount = true;
                        }
                        //shipping
                        cusShipTotal = PriceHelper.FormatShippingPrice(order.OrderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                        //payment method additional fee
                        cusPaymentMethodAdditionalFee = PriceHelper.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, false);
                    }
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    {
                        //subtotal
                        cusSubTotal = PriceHelper.FormatPrice(order.OrderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                        //discount (applied to order subtotal)
                        if (order.OrderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                        {
                            cusSubTotalDiscount = PriceHelper.FormatPrice(-order.OrderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                            dislaySubTotalDiscount = true;
                        }
                        //shipping
                        cusShipTotal = PriceHelper.FormatShippingPrice(order.OrderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                        //payment method additional fee
                        cusPaymentMethodAdditionalFee = PriceHelper.FormatPaymentMethodAdditionalFee(order.PaymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, language, true);
                    }
                    break;
            }

            //shipping
            bool dislayShipping = order.ShippingStatus != ShippingStatusEnum.ShippingNotRequired;

            //payment method fee
            bool displayPaymentMethodFee = true;
            if (order.PaymentMethodAdditionalFeeExclTaxInCustomerCurrency == decimal.Zero)
            {
                displayPaymentMethodFee = false;
            }

            //tax
            bool displayTax = true;
            bool displayTaxRates = true;
            if (IoC.Resolve<ITaxService>().HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayTypeEnum.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && IoC.Resolve<ITaxService>().HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    taxRates = order.TaxRatesDictionaryInCustomerCurrency;
                   
                    displayTaxRates = IoC.Resolve<ITaxService>().DisplayTaxRates && taxRates.Count > 0;
                    displayTax = !displayTaxRates;

                    string taxStr = PriceHelper.FormatPrice(order.OrderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                    cusTaxTotal = taxStr;
                }
            }

            //discount
            bool dislayDiscount = false;
            if (order.OrderDiscountInCustomerCurrency > decimal.Zero)
            {
                cusDiscount = PriceHelper.FormatPrice(-order.OrderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                dislayDiscount = true;
            }
            
            //total
            cusTotal = PriceHelper.FormatPrice(order.OrderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false);




            //subtotal
            sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", color3, localizationManager.GetLocaleResourceString("Order.Sub-Total", languageId), cusSubTotal));
            
            //discount (applied to order subtotal)
            if (dislaySubTotalDiscount)
            {
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", color3, localizationManager.GetLocaleResourceString("Order.Discount", languageId), cusSubTotalDiscount));
            }
            

            //shipping
            if (dislayShipping)
            {
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", color3, localizationManager.GetLocaleResourceString("Order.Shipping", languageId), cusShipTotal));
            }

            //payment method fee
            if (displayPaymentMethodFee)
            {
                string paymentMethodFeeTitle = localizationManager.GetLocaleResourceString("Order.PaymentMethodAdditionalFee", languageId);
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", color3, paymentMethodFeeTitle, cusPaymentMethodAdditionalFee));
            }

            //tax
            if (displayTax)
            {
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", color3, localizationManager.GetLocaleResourceString("Order.Tax", languageId), cusTaxTotal));
            }
            if (displayTaxRates)
            {
                foreach (var item in taxRates)
                {
                    string taxRate = String.Format(localizationManager.GetLocaleResourceString("Order.Totals.TaxRate"), PriceHelper.FormatTaxRate(item.Key));
                    string taxValue = PriceHelper.FormatPrice(item.Value, true, order.CustomerCurrencyCode, false);
                    sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", color3, taxRate, taxValue));
                }
            }

            //discount
            if (dislayDiscount)
            {
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", color3, localizationManager.GetLocaleResourceString("Order.Discount", languageId), cusDiscount));
            }
            
            //gift cards
            var gcuhC = IoC.Resolve<IOrderService>().GetAllGiftCardUsageHistoryEntries(null, null, order.OrderId);
            foreach (var giftCardUsageHistory in gcuhC)
            {
                string giftCardText = String.Format(localizationManager.GetLocaleResourceString("Order.GiftCardInfo", languageId), HttpUtility.HtmlEncode(giftCardUsageHistory.GiftCard.GiftCardCouponCode));
                string giftCardAmount = PriceHelper.FormatPrice(-giftCardUsageHistory.UsedValueInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", color3, giftCardText, giftCardAmount));
            }

            //reward points
            if (order.RedeemedRewardPoints != null)
            {
                string rpTitle = string.Format(localizationManager.GetLocaleResourceString("Order.Totals.RewardPoints", languageId), -order.RedeemedRewardPoints.Points);
                string rpAmount = PriceHelper.FormatPrice(-order.RedeemedRewardPoints.UsedAmountInCustomerCurrency, true, order.CustomerCurrencyCode, false);
                sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", color3, rpTitle, rpAmount));
            }

            //total
            sb.AppendLine(string.Format("<tr style=\"text-align:right;\"><td>&nbsp;</td><td colspan=\"2\" style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{1}</strong></td> <td style=\"background-color: {0};padding:0.6em 0.4 em;\"><strong>{2}</strong></td></tr>", color3, localizationManager.GetLocaleResourceString("Order.OrderTotal", languageId), cusTotal));
            #endregion
            
            sb.AppendLine("</table>");
            result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="template">Template</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>New template</returns>
        public string ReplaceMessageTemplateTokens(Order order,
            string template, int languageId)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("Order.OrderNumber", order.OrderId.ToString());

            tokens.Add("Order.CustomerFullName", HttpUtility.HtmlEncode(order.BillingFullName));
            tokens.Add("Order.CustomerEmail", HttpUtility.HtmlEncode(order.BillingEmail));


            tokens.Add("Order.BillingFirstName", HttpUtility.HtmlEncode(order.BillingFirstName));
            tokens.Add("Order.BillingLastName", HttpUtility.HtmlEncode(order.BillingLastName));
            tokens.Add("Order.BillingPhoneNumber", HttpUtility.HtmlEncode(order.BillingPhoneNumber));
            tokens.Add("Order.BillingEmail", HttpUtility.HtmlEncode(order.BillingEmail.ToString()));
            tokens.Add("Order.BillingFaxNumber", HttpUtility.HtmlEncode(order.BillingFaxNumber));
            tokens.Add("Order.BillingCompany", HttpUtility.HtmlEncode(order.BillingCompany));
            tokens.Add("Order.BillingAddress1", HttpUtility.HtmlEncode(order.BillingAddress1));
            tokens.Add("Order.BillingAddress2", HttpUtility.HtmlEncode(order.BillingAddress2));
            tokens.Add("Order.BillingCity", HttpUtility.HtmlEncode(order.BillingCity));
            tokens.Add("Order.BillingStateProvince", HttpUtility.HtmlEncode(order.BillingStateProvince));
            tokens.Add("Order.BillingZipPostalCode", HttpUtility.HtmlEncode(order.BillingZipPostalCode));
            tokens.Add("Order.BillingCountry", HttpUtility.HtmlEncode(order.BillingCountry));

            tokens.Add("Order.ShippingMethod", HttpUtility.HtmlEncode(order.ShippingMethod));

            tokens.Add("Order.ShippingFirstName", HttpUtility.HtmlEncode(order.ShippingFirstName));
            tokens.Add("Order.ShippingLastName", HttpUtility.HtmlEncode(order.ShippingLastName));
            tokens.Add("Order.ShippingPhoneNumber", HttpUtility.HtmlEncode(order.ShippingPhoneNumber));
            tokens.Add("Order.ShippingEmail", HttpUtility.HtmlEncode(order.ShippingEmail.ToString()));
            tokens.Add("Order.ShippingFaxNumber", HttpUtility.HtmlEncode(order.ShippingFaxNumber));
            tokens.Add("Order.ShippingCompany", HttpUtility.HtmlEncode(order.ShippingCompany));
            tokens.Add("Order.ShippingAddress1", HttpUtility.HtmlEncode(order.ShippingAddress1));
            tokens.Add("Order.ShippingAddress2", HttpUtility.HtmlEncode(order.ShippingAddress2));
            tokens.Add("Order.ShippingCity", HttpUtility.HtmlEncode(order.ShippingCity));
            tokens.Add("Order.ShippingStateProvince", HttpUtility.HtmlEncode(order.ShippingStateProvince));
            tokens.Add("Order.ShippingZipPostalCode", HttpUtility.HtmlEncode(order.ShippingZipPostalCode));
            tokens.Add("Order.ShippingCountry", HttpUtility.HtmlEncode(order.ShippingCountry));

            tokens.Add("Order.TrackingNumber", HttpUtility.HtmlEncode(order.TrackingNumber));
            tokens.Add("Order.VatNumber", HttpUtility.HtmlEncode(order.VatNumber));

            tokens.Add("Order.Product(s)", ProductListToHtmlTable(order, languageId));

            var language = IoC.Resolve<ILanguageService>().GetLanguageById(languageId);
            if (language != null && !String.IsNullOrEmpty(language.LanguageCulture))
            {
                DateTime createdOn = DateTimeHelper.ConvertToUserTime(order.CreatedOn, TimeZoneInfo.Utc, DateTimeHelper.GetCustomerTimeZone(order.Customer));
                tokens.Add("Order.CreatedOn", createdOn.ToString("D", new CultureInfo(language.LanguageCulture)));
            }
            else
            {
                tokens.Add("Order.CreatedOn", order.CreatedOn.ToString("D"));
            }
            tokens.Add("Order.OrderURLForCustomer", string.Format("{0}orderdetails.aspx?orderid={1}", IoC.Resolve<ISettingManager>().StoreUrl, order.OrderId));

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="template">Template</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(Customer customer,
            string template)
        {
            return ReplaceMessageTemplateTokens(customer, template, null);
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="template">Template</param>
        /// <param name="additinalKeys">Additinal keys</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(Customer customer,
            string template, NameValueCollection additinalKeys)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("Customer.Email", HttpUtility.HtmlEncode(customer.Email));
            tokens.Add("Customer.Username", HttpUtility.HtmlEncode(customer.Username));
            tokens.Add("Customer.FullName", HttpUtility.HtmlEncode(customer.FullName));
            tokens.Add("Customer.VatNumber", HttpUtility.HtmlEncode(customer.VatNumber));
            tokens.Add("Customer.VatNumberStatus", HttpUtility.HtmlEncode(customer.VatNumberStatus.ToString()));

            string passwordRecoveryUrl = string.Empty;
            passwordRecoveryUrl = string.Format("{0}passwordrecovery.aspx?prt={1}&email={2}", IoC.Resolve<ISettingManager>().StoreUrl, customer.PasswordRecoveryToken, customer.Email);
            tokens.Add("Customer.PasswordRecoveryURL", passwordRecoveryUrl);

            string accountActivationUrl = string.Empty;
            accountActivationUrl = string.Format("{0}accountactivation.aspx?act={1}&email={2}", IoC.Resolve<ISettingManager>().StoreUrl, customer.AccountActivationToken, customer.Email);
            tokens.Add("Customer.AccountActivationURL", accountActivationUrl);

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            if (additinalKeys != null)
            {
                foreach (string token in additinalKeys.Keys)
                {
                    template = Replace(template, String.Format(@"%{0}%", token), additinalKeys[token]);
                }
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="product">Product instance</param>
        /// <param name="template">Template</param>
        /// <param name="additinalKeys">Additinal keys</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(Customer customer, Product product,
            string template, NameValueCollection additinalKeys)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("Customer.Email", HttpUtility.HtmlEncode(customer.Email));
            tokens.Add("Customer.Username", HttpUtility.HtmlEncode(customer.Username));
            tokens.Add("Customer.FullName", HttpUtility.HtmlEncode(customer.FullName));
            tokens.Add("Customer.VatNumber", HttpUtility.HtmlEncode(customer.VatNumber));
            tokens.Add("Customer.VatNumberStatus", HttpUtility.HtmlEncode(customer.VatNumberStatus.ToString()));

            tokens.Add("Product.Name", HttpUtility.HtmlEncode(product.Name));
            tokens.Add("Product.ShortDescription", product.ShortDescription);
            tokens.Add("Product.ProductURLForCustomer", SEOHelper.GetProductUrl(product));

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            if (additinalKeys != null)
            {
                foreach (string token in additinalKeys.Keys)
                {
                    template = Replace(template, String.Format(@"%{0}%", token), additinalKeys[token]);
                }
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="cart">Shopping cart</param>
        /// <param name="template">Template</param>
        /// <param name="additinalKeys">Additinal keys</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(Customer customer,
            ShoppingCart cart, string template, NameValueCollection additinalKeys)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("Customer.Email", HttpUtility.HtmlEncode(customer.Email));
            tokens.Add("Customer.Username", HttpUtility.HtmlEncode(customer.Username));
            tokens.Add("Customer.FullName", HttpUtility.HtmlEncode(customer.FullName));
            tokens.Add("Customer.VatNumber", HttpUtility.HtmlEncode(customer.VatNumber));
            tokens.Add("Customer.VatNumberStatus", HttpUtility.HtmlEncode(customer.VatNumberStatus.ToString()));

            tokens.Add("Wishlist.URLForCustomer", SEOHelper.GetWishlistUrl(customer.CustomerGuid));
            //UNDONE add a wishlist content token

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            if (additinalKeys != null)
            {
                foreach (string token in additinalKeys.Keys)
                {
                    template = Replace(template, String.Format(@"%{0}%", token), additinalKeys[token]);
                }
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="template">Template</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(Customer customer,
            ForumPost forumPost, ForumTopic forumTopic, Forum forum, string template)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("Customer.Email", HttpUtility.HtmlEncode(customer.Email));
            tokens.Add("Customer.Username", HttpUtility.HtmlEncode(customer.Username));
            tokens.Add("Customer.FullName", HttpUtility.HtmlEncode(customer.FullName));
            tokens.Add("Customer.VatNumber", HttpUtility.HtmlEncode(customer.VatNumber));
            tokens.Add("Customer.VatNumberStatus", HttpUtility.HtmlEncode(customer.VatNumberStatus.ToString()));

            if (forumPost != null)
            {
                tokens.Add("Forums.PostAuthor", HttpUtility.HtmlEncode(forumPost.User.FormatUserName()));
                tokens.Add("Forums.PostBody", forumPost.FormatPostText());
            }
            if (forumTopic != null)
            {
                tokens.Add("Forums.TopicURL", SEOHelper.GetForumTopicUrl(forumTopic));
                tokens.Add("Forums.TopicName", HttpUtility.HtmlEncode(forumTopic.Subject));
            }
            if (forum != null)
            {
                tokens.Add("Forums.ForumURL", SEOHelper.GetForumUrl(forum));
                tokens.Add("Forums.ForumName", HttpUtility.HtmlEncode(forum.Name));
            }
            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="template">Template</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(ProductVariant productVariant,
            string template, int languageId)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("ProductVariant.ID", productVariant.ProductVariantId.ToString());
            tokens.Add("ProductVariant.FullProductName", HttpUtility.HtmlEncode(productVariant.FullProductName));
            tokens.Add("ProductVariant.StockQuantity", productVariant.StockQuantity.ToString());

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="template">Template</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(NewsComment newsComment,
            string template)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("NewsComment.NewsTitle", HttpUtility.HtmlEncode(newsComment.News.Title));

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="template">Template</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(BlogComment blogComment,
            string template)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("BlogComment.BlogPostTitle", HttpUtility.HtmlEncode(blogComment.BlogPost.BlogPostTitle));

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="template">Template</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(ProductReview productReview,
            string template)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("ProductReview.ProductName", HttpUtility.HtmlEncode(productReview.Product.Name));

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <param name="template">Template</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(PrivateMessage privateMessage,
            string template)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("PrivateMessage.Subject", HttpUtility.HtmlEncode(privateMessage.Subject));
            tokens.Add("PrivateMessage.Text", privateMessage.FormatPrivateMessageText());

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <param name="template">Template</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(GiftCard giftCard,
            string template)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("GiftCard.SenderName", HttpUtility.HtmlEncode(giftCard.SenderName));
            tokens.Add("GiftCard.SenderEmail", HttpUtility.HtmlEncode(giftCard.SenderEmail));
            tokens.Add("GiftCard.RecipientName", HttpUtility.HtmlEncode(giftCard.RecipientName));
            tokens.Add("GiftCard.RecipientEmail", HttpUtility.HtmlEncode(giftCard.RecipientEmail));
            tokens.Add("GiftCard.Amount", HttpUtility.HtmlEncode(PriceHelper.FormatPrice(giftCard.Amount, true, false)));
            tokens.Add("GiftCard.CouponCode", HttpUtility.HtmlEncode(giftCard.GiftCardCouponCode));
            tokens.Add("GiftCard.Message", giftCard.FormatGiftCardMessageText());

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            return template;
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="template">Template</param>
        /// <returns>New template</returns>
        private string ReplaceMessageTemplateTokens(ReturnRequest returnRequest,
            string template)
        {
            var tokens = new NameValueCollection();
            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);

            tokens.Add("Customer.Email", HttpUtility.HtmlEncode(returnRequest.Customer.Email));
            tokens.Add("Customer.FullName", HttpUtility.HtmlEncode(returnRequest.Customer.FullName));

            tokens.Add("ReturnRequest.ID", returnRequest.ReturnRequestId.ToString());
            tokens.Add("ReturnRequest.Product.Quantity", returnRequest.Quantity.ToString());
            tokens.Add("ReturnRequest.Product.Name", HttpUtility.HtmlEncode(returnRequest.OrderProductVariant.ProductVariant.FullProductName));
            tokens.Add("ReturnRequest.Reason", HttpUtility.HtmlEncode(returnRequest.ReasonForReturn));
            tokens.Add("ReturnRequest.RequestedAction", HttpUtility.HtmlEncode(returnRequest.RequestedAction));
            tokens.Add("ReturnRequest.CustomerComment", returnRequest.FormatReturnRequestCommentsText());

            foreach (string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            return template;
        }

        #endregion

        #region Methods

        #region Repository methods

        /// <summary>
        /// Gets a message template by template identifier
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <returns>Message template</returns>
        public MessageTemplate GetMessageTemplateById(int messageTemplateId)
        {
            if (messageTemplateId == 0)
                return null;

            
            var query = from mt in _context.MessageTemplates
                        where mt.MessageTemplateId == messageTemplateId
                        select mt;
            var messageTemplate = query.SingleOrDefault();

            return messageTemplate;
        }

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <returns>Message template collection</returns>
        public List<MessageTemplate> GetAllMessageTemplates()
        {
            
            var query = from mt in _context.MessageTemplates
                        orderby mt.Name
                        select mt;
            var collection = query.ToList();

            return collection;
        }
         
        /// <summary>
        /// Gets a localized message template by identifier
        /// </summary>
        /// <param name="localizedMessageTemplateId">Localized message template identifier</param>
        /// <returns>Localized message template</returns>
        public LocalizedMessageTemplate GetLocalizedMessageTemplateById(int localizedMessageTemplateId)
        {
            if (localizedMessageTemplateId == 0)
                return null;

            
            var query = from lmt in _context.LocalizedMessageTemplates
                        where lmt.MessageTemplateLocalizedId == localizedMessageTemplateId
                        select lmt;
            var localizedMessageTemplate = query.SingleOrDefault(); 
            
            return localizedMessageTemplate;
        }

        /// <summary>
        /// Gets a localized message template by name and language identifier
        /// </summary>
        /// <param name="name">Message template name</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized message template</returns>
        public LocalizedMessageTemplate GetLocalizedMessageTemplate(string name, int languageId)
        {
            
            var query = from lmt in _context.LocalizedMessageTemplates
                        join mt in _context.MessageTemplates on lmt.MessageTemplateId equals mt.MessageTemplateId
                        where lmt.LanguageId == languageId &&
                        mt.Name == name
                        select lmt;
            var localizedMessageTemplate = query.FirstOrDefault();

            return localizedMessageTemplate;

        }

        /// <summary>
        /// Deletes a localized message template
        /// </summary>
        /// <param name="localizedMessageTemplateId">Message template identifier</param>
        public void DeleteLocalizedMessageTemplate(int localizedMessageTemplateId)
        {
            var localizedMessageTemplate = GetLocalizedMessageTemplateById(localizedMessageTemplateId);
            if (localizedMessageTemplate == null)
                return;

            
            if (!_context.IsAttached(localizedMessageTemplate))
                _context.LocalizedMessageTemplates.Attach(localizedMessageTemplate);
            _context.DeleteObject(localizedMessageTemplate);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all localized message templates
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <returns>Localized message template collection</returns>
        public List<LocalizedMessageTemplate> GetAllLocalizedMessageTemplates(string messageTemplateName)
        {
            
            var query = from lmt in _context.LocalizedMessageTemplates
                        join mt in _context.MessageTemplates on lmt.MessageTemplateId equals mt.MessageTemplateId
                        where mt.Name == messageTemplateName
                        orderby lmt.LanguageId
                        select lmt;
            var localizedMessageTemplates = query.ToList();
            return localizedMessageTemplates;
        }

        /// <summary>
        /// Inserts a localized message template
        /// </summary>
        /// <param name="localizedMessageTemplate">Localized message template</param>
        public void InsertLocalizedMessageTemplate(LocalizedMessageTemplate localizedMessageTemplate)
        {
            if (localizedMessageTemplate == null)
                throw new ArgumentNullException("localizedMessageTemplate");

            localizedMessageTemplate.BccEmailAddresses = CommonHelper.EnsureNotNull(localizedMessageTemplate.BccEmailAddresses);
            localizedMessageTemplate.BccEmailAddresses = CommonHelper.EnsureMaximumLength(localizedMessageTemplate.BccEmailAddresses, 200);
            localizedMessageTemplate.Subject = CommonHelper.EnsureNotNull(localizedMessageTemplate.Subject);
            localizedMessageTemplate.Subject = CommonHelper.EnsureMaximumLength(localizedMessageTemplate.Subject, 200);
            localizedMessageTemplate.Body = CommonHelper.EnsureNotNull(localizedMessageTemplate.Body);
           
            

            _context.LocalizedMessageTemplates.AddObject(localizedMessageTemplate);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the localized message template
        /// </summary>
        /// <param name="localizedMessageTemplate">Localized message template</param>
        public void UpdateLocalizedMessageTemplate(LocalizedMessageTemplate localizedMessageTemplate)
        {
            if (localizedMessageTemplate == null)
                throw new ArgumentNullException("localizedMessageTemplate");

            localizedMessageTemplate.BccEmailAddresses = CommonHelper.EnsureNotNull(localizedMessageTemplate.BccEmailAddresses);
            localizedMessageTemplate.BccEmailAddresses = CommonHelper.EnsureMaximumLength(localizedMessageTemplate.BccEmailAddresses, 200);
            localizedMessageTemplate.Subject = CommonHelper.EnsureNotNull(localizedMessageTemplate.Subject);
            localizedMessageTemplate.Subject = CommonHelper.EnsureMaximumLength(localizedMessageTemplate.Subject, 200);
            localizedMessageTemplate.Body = CommonHelper.EnsureNotNull(localizedMessageTemplate.Body);
           
            
            if (!_context.IsAttached(localizedMessageTemplate))
                _context.LocalizedMessageTemplates.Attach(localizedMessageTemplate);

            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Email item identifier</param>
        /// <returns>Email item</returns>
        public QueuedEmail GetQueuedEmailById(int queuedEmailId)
        {
            if (queuedEmailId == 0)
                return null;

            
            var query = from qe in _context.QueuedEmails
                        where qe.QueuedEmailId == queuedEmailId
                        select qe;
            var queuedEmail = query.SingleOrDefault();
            return queuedEmail;
        }

        /// <summary>
        /// Deletes a queued email
        /// </summary>
        /// <param name="queuedEmailId">Email item identifier</param>
        public void DeleteQueuedEmail(int queuedEmailId)
        {
            var queuedEmail = GetQueuedEmailById(queuedEmailId);
            if (queuedEmail == null)
                return;

            
            if (!_context.IsAttached(queuedEmail))
                _context.QueuedEmails.Attach(queuedEmail);
            _context.DeleteObject(queuedEmail);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all queued emails
        /// </summary>
        /// <param name="queuedEmailCount">Email item count. 0 if you want to get all items</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <returns>Email item collection</returns>
        public List<QueuedEmail> GetAllQueuedEmails(int queuedEmailCount, 
            bool loadNotSentItemsOnly, int maxSendTries)
        {
            return GetAllQueuedEmails(string.Empty, string.Empty, null, null, 
                queuedEmailCount, loadNotSentItemsOnly, maxSendTries);
        }

        /// <summary>
        /// Gets all queued emails
        /// </summary>
        /// <param name="fromEmail">From Email</param>
        /// <param name="toEmail">To Email</param>
        /// <param name="startTime">The start time</param>
        /// <param name="endTime">The end time</param>
        /// <param name="queuedEmailCount">Email item count. 0 if you want to get all items</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <returns>Email item collection</returns>
        public List<QueuedEmail> GetAllQueuedEmails(string fromEmail,
            string toEmail, DateTime? startTime, DateTime? endTime,
            int queuedEmailCount, bool loadNotSentItemsOnly, int maxSendTries)
        {
            if (fromEmail == null)
                fromEmail = string.Empty;
            fromEmail = fromEmail.Trim();

            if (toEmail == null)
                toEmail = string.Empty;
            toEmail = toEmail.Trim();

            
            var query = (IQueryable<QueuedEmail>)_context.QueuedEmails;
            if (!String.IsNullOrEmpty(fromEmail))
                query = query.Where(qe => qe.From.Contains(fromEmail));
            if (!String.IsNullOrEmpty(toEmail))
                query = query.Where(qe => qe.To.Contains(toEmail));
            if (startTime.HasValue)
                query = query.Where(qe => startTime.Value <= qe.CreatedOn);
            if (endTime.HasValue)
                query = query.Where(qe => endTime.Value >= qe.CreatedOn);
            if (loadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOn.HasValue);
            query = query.Where(qe => qe.SendTries < maxSendTries);
            if (queuedEmailCount > 0)
                query = query.Take(queuedEmailCount);
            query = query.OrderByDescending(qe => qe.Priority).ThenBy(qe => qe.CreatedOn);

            var queuedEmails = query.ToList();
            
            return queuedEmails;
        }

        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="priority">The priority</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="cc">CC</param>
        /// <param name="bcc">BCC</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="createdOn">The date and time of item creation</param>
        /// <param name="sendTries">The send tries</param>
        /// <param name="sentOn">The sent date and time. Null if email is not sent yet</param>
        /// <param name="emailAccountId">Email account identifer</param>
        /// <returns>Queued email</returns>
        public QueuedEmail InsertQueuedEmail(int priority, MailAddress from,
            MailAddress to, string cc, string bcc,
            string subject, string body, DateTime createdOn, int sendTries,
            DateTime? sentOn, int emailAccountId)
        {
            return InsertQueuedEmail(priority, from.Address, from.DisplayName,
              to.Address, to.DisplayName, cc, bcc, subject, body, createdOn, 
              sendTries, sentOn, emailAccountId);
        }

        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="priority">The priority</param>
        /// <param name="from">From</param>
        /// <param name="fromName">From name</param>
        /// <param name="to">To</param>
        /// <param name="toName">To name</param>
        /// <param name="cc">Cc</param>
        /// <param name="bcc">Bcc</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="createdOn">The date and time of item creation</param>
        /// <param name="sendTries">The send tries</param>
        /// <param name="sentOn">The sent date and time. Null if email is not sent yet</param>
        /// <param name="emailAccountId">Email account identifer</param>
        /// <returns>Queued email</returns>
        public QueuedEmail InsertQueuedEmail(int priority, string from,
            string fromName, string to, string toName, string cc, string bcc,
            string subject, string body, DateTime createdOn, int sendTries,
            DateTime? sentOn, int emailAccountId)
        {
            from = CommonHelper.EnsureNotNull(from);
            from = CommonHelper.EnsureMaximumLength(from, 500);
            fromName = CommonHelper.EnsureNotNull(fromName);
            fromName = CommonHelper.EnsureMaximumLength(fromName, 500);
            to = CommonHelper.EnsureNotNull(to);
            to = CommonHelper.EnsureMaximumLength(to, 500);
            toName = CommonHelper.EnsureNotNull(toName);
            toName = CommonHelper.EnsureMaximumLength(toName, 500);
            cc = CommonHelper.EnsureNotNull(cc);
            cc = CommonHelper.EnsureMaximumLength(cc, 500);
            bcc = CommonHelper.EnsureNotNull(bcc);
            bcc = CommonHelper.EnsureMaximumLength(bcc, 500);
            subject = CommonHelper.EnsureNotNull(subject);
            subject = CommonHelper.EnsureMaximumLength(subject, 500);
            body = CommonHelper.EnsureNotNull(body);

            

            var queuedEmail = _context.QueuedEmails.CreateObject();
            queuedEmail.Priority = priority;
            queuedEmail.From = from;
            queuedEmail.FromName = fromName;
            queuedEmail.To = to;
            queuedEmail.ToName = toName;
            queuedEmail.CC = cc;
            queuedEmail.Bcc = bcc;
            queuedEmail.Subject = subject;
            queuedEmail.Body = body;
            queuedEmail.CreatedOn = createdOn;
            queuedEmail.SendTries = sendTries;
            queuedEmail.SentOn = sentOn;
            queuedEmail.EmailAccountId = emailAccountId;

            _context.QueuedEmails.AddObject(queuedEmail);
            _context.SaveChanges();

            return queuedEmail;
        }

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public void UpdateQueuedEmail(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");
            
            queuedEmail.From = CommonHelper.EnsureNotNull(queuedEmail.From);
            queuedEmail.From = CommonHelper.EnsureMaximumLength(queuedEmail.From, 500);
            queuedEmail.FromName = CommonHelper.EnsureNotNull(queuedEmail.FromName);
            queuedEmail.FromName = CommonHelper.EnsureMaximumLength(queuedEmail.FromName, 500);
            queuedEmail.To = CommonHelper.EnsureNotNull(queuedEmail.To);
            queuedEmail.To = CommonHelper.EnsureMaximumLength(queuedEmail.To, 500);
            queuedEmail.ToName = CommonHelper.EnsureNotNull(queuedEmail.ToName);
            queuedEmail.ToName = CommonHelper.EnsureMaximumLength(queuedEmail.ToName, 500);
            queuedEmail.CC = CommonHelper.EnsureNotNull(queuedEmail.CC);
            queuedEmail.CC = CommonHelper.EnsureMaximumLength(queuedEmail.CC, 500);
            queuedEmail.Bcc = CommonHelper.EnsureNotNull(queuedEmail.Bcc);
            queuedEmail.Bcc = CommonHelper.EnsureMaximumLength(queuedEmail.Bcc, 500);
            queuedEmail.Subject = CommonHelper.EnsureNotNull(queuedEmail.Subject);
            queuedEmail.Subject = CommonHelper.EnsureMaximumLength(queuedEmail.Subject, 500);
            queuedEmail.Body = CommonHelper.EnsureNotNull(queuedEmail.Body);

            
            if (!_context.IsAttached(queuedEmail))
                _context.QueuedEmails.Attach(queuedEmail);

            _context.SaveChanges();
        }

        /// <summary>
        /// Inserts the new newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetterSubscription entity</param>
        public void InsertNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription)
        {
            if (newsLetterSubscription == null)
                throw new ArgumentNullException("newsLetterSubscription");

            if (!CommonHelper.IsValidEmail(newsLetterSubscription.Email))
            {
                throw new NopException("Email is not valid.");
            }

            newsLetterSubscription.Email = CommonHelper.EnsureNotNull(newsLetterSubscription.Email);
            newsLetterSubscription.Email = newsLetterSubscription.Email.Trim();
            newsLetterSubscription.Email = CommonHelper.EnsureMaximumLength(newsLetterSubscription.Email, 255);

            
            
            _context.NewsLetterSubscriptions.AddObject(newsLetterSubscription);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets the newsletter subscription by newsletter subscription identifier
        /// </summary>
        /// <param name="newsLetterSubscriptionId">The newsletter subscription identifier</param>
        /// <returns>NewsLetterSubscription entity</returns>
        public NewsLetterSubscription GetNewsLetterSubscriptionById(int newsLetterSubscriptionId)
        {
            
            var query = from n in _context.NewsLetterSubscriptions
                        where n.NewsLetterSubscriptionId == newsLetterSubscriptionId
                        select n;
            var newsLetterSubscription = query.SingleOrDefault();

            return newsLetterSubscription;
        }

        /// <summary>
        /// Gets the newsletter subscription by newsletter subscription GUID
        /// </summary>
        /// <param name="newsLetterSubscriptionGuid">The newsletter subscription GUID</param>
        /// <returns>NewsLetterSubscription entity</returns>
        public NewsLetterSubscription GetNewsLetterSubscriptionByGuid(Guid newsLetterSubscriptionGuid)
        {
            if(newsLetterSubscriptionGuid == null || newsLetterSubscriptionGuid == Guid.Empty)
            {
                return null;
            }

            
            var query = from n in _context.NewsLetterSubscriptions
                        where n.NewsLetterSubscriptionGuid == newsLetterSubscriptionGuid
                        orderby n.NewsLetterSubscriptionId
                        select n;
            var newsLetterSubscription = query.FirstOrDefault();

            return newsLetterSubscription;
        }

        /// <summary>
        /// Gets the newsletter subscription by email
        /// </summary>
        /// <param name="email">The Email</param>
        /// <returns>NewsLetterSubscription entity</returns>
        public NewsLetterSubscription GetNewsLetterSubscriptionByEmail(string email)
        {
            if(!CommonHelper.IsValidEmail(email))
            {
                return null;
            }

            email = email.Trim();

            
            var query = from n in _context.NewsLetterSubscriptions
                        where n.Email == email
                        orderby n.NewsLetterSubscriptionId
                        select n;
            var newsLetterSubscription = query.FirstOrDefault();

            return newsLetterSubscription;
        }

        /// <summary>
        /// Gets the newsletter subscription collection
        /// </summary>
        /// <param name="email">E,ail to search or string.Empty to load all records</param>
        /// <param name="showHidden">A value indicating whether the not active subscriptions should be loaded</param>
        /// <returns>NewsLetterSubscription entity collection</returns>
        public List<NewsLetterSubscription> GetAllNewsLetterSubscriptions(string email, bool showHidden)
        {

            var query1 = from nls in _context.NewsLetterSubscriptions
                         from c in _context.Customers
                         .Where(c => c.Email == nls.Email)
                         .DefaultIfEmpty()
                         where
                         (showHidden || nls.Active) &&
                         (c == null || c.CustomerId == 0 || (c.Active && !c.Deleted)) &&
                         (String.IsNullOrEmpty(email) || nls.Email.Contains(email))
                         select nls.NewsLetterSubscriptionId;

            var query2 = from nls in _context.NewsLetterSubscriptions
                         where query1.Contains(nls.NewsLetterSubscriptionId)
                         orderby nls.Email
                         select nls;

            var newsletterSubscriptions = query2.ToList();
            return newsletterSubscriptions;
        }

        /// <summary>
        /// Updates the newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetterSubscription entity</param>
        public void UpdateNewsLetterSubscription(NewsLetterSubscription newsLetterSubscription)
        {
            if (newsLetterSubscription == null)
                throw new ArgumentNullException("newsLetterSubscription");

            newsLetterSubscription.Email = CommonHelper.EnsureNotNull(newsLetterSubscription.Email);
            newsLetterSubscription.Email = newsLetterSubscription.Email.Trim();
            newsLetterSubscription.Email = CommonHelper.EnsureMaximumLength(newsLetterSubscription.Email, 255);

            if (!CommonHelper.IsValidEmail(newsLetterSubscription.Email))
            {
                throw new NopException("Email is not valid.");
            }

            
            if (!_context.IsAttached(newsLetterSubscription))
                _context.NewsLetterSubscriptions.Attach(newsLetterSubscription);

            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes the newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscriptionId">The newsletter subscription identifier</param>
        public void DeleteNewsLetterSubscription(int newsLetterSubscriptionId)
        {
            var newsLetterSubscription = GetNewsLetterSubscriptionById(newsLetterSubscriptionId);
            if (newsLetterSubscription == null)
                return;

            
            if (!_context.IsAttached(newsLetterSubscription))
                _context.NewsLetterSubscriptions.Attach(newsLetterSubscription);
            _context.DeleteObject(newsLetterSubscription);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets a email account by identifier
        /// </summary>
        /// <param name="emailAccountId">The email account identifier</param>
        /// <returns>Email account</returns>
        public EmailAccount GetEmailAccountById(int emailAccountId)
        {
            if (emailAccountId == 0)
                return null;

            
            var query = from ea in _context.EmailAccounts
                        where ea.EmailAccountId == emailAccountId
                        select ea;
            var emailAccount = query.SingleOrDefault();
            return emailAccount;
        }

        /// <summary>
        /// Deletes the email account
        /// </summary>
        /// <param name="emailAccountId">The email account identifier</param>
        public void DeleteEmailAccount(int emailAccountId)
        {
            var emailAccount = GetEmailAccountById(emailAccountId);
            if (emailAccount == null)
                return;

            if (GetAllEmailAccounts().Count == 1)
                throw new NopException("You cannot delete this email account. At least one account is required.");

            
            if (!_context.IsAttached(emailAccount))
                _context.EmailAccounts.Attach(emailAccount);
            _context.DeleteObject(emailAccount);
            _context.SaveChanges();
        }

        /// <summary>
        /// Inserts an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public void InsertEmailAccount(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            emailAccount.Email = CommonHelper.EnsureNotNull(emailAccount.Email);
            emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
            emailAccount.Host = CommonHelper.EnsureNotNull(emailAccount.Host);
            emailAccount.Username = CommonHelper.EnsureNotNull(emailAccount.Username);
            emailAccount.Password = CommonHelper.EnsureNotNull(emailAccount.Password);

            emailAccount.Email = emailAccount.Email.Trim();
            emailAccount.DisplayName = emailAccount.DisplayName.Trim();
            emailAccount.Host = emailAccount.Host.Trim();
            emailAccount.Username = emailAccount.Username.Trim();
            emailAccount.Password = emailAccount.Password.Trim();

            emailAccount.Email = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
            emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
            emailAccount.Host = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
            emailAccount.Username = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
            emailAccount.Password = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

            
            
            _context.EmailAccounts.AddObject(emailAccount);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public void UpdateEmailAccount(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            emailAccount.Email = CommonHelper.EnsureNotNull(emailAccount.Email);
            emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
            emailAccount.Host = CommonHelper.EnsureNotNull(emailAccount.Host);
            emailAccount.Username = CommonHelper.EnsureNotNull(emailAccount.Username);
            emailAccount.Password = CommonHelper.EnsureNotNull(emailAccount.Password);

            emailAccount.Email = emailAccount.Email.Trim();
            emailAccount.DisplayName = emailAccount.DisplayName.Trim();
            emailAccount.Host = emailAccount.Host.Trim();
            emailAccount.Username = emailAccount.Username.Trim();
            emailAccount.Password = emailAccount.Password.Trim();

            emailAccount.Email = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
            emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
            emailAccount.Host = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
            emailAccount.Username = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
            emailAccount.Password = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

            
            if (!_context.IsAttached(emailAccount))
                _context.EmailAccounts.Attach(emailAccount);

            _context.SaveChanges();
        }

        /// <summary>
        /// Gets all email accounts
        /// </summary>
        /// <returns>Email accounts</returns>
        public List<EmailAccount> GetAllEmailAccounts()
        {
            
            var query = from ea in _context.EmailAccounts
                        orderby ea.EmailAccountId
                        select ea;

            var emailAccounts = query.ToList();

            return emailAccounts;
        }

        #endregion

        #region Workflow methods

        /// <summary>
        /// Sends an order completed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderCompletedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            string templateName = "OrderCompleted.CustomerNotification";
            LocalizedMessageTemplate localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;
                
            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Subject, languageId);
            string body = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Body, languageId);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(order.BillingEmail, order.BillingFullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends an order placed notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderPlacedStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");


            string templateName = "OrderPlaced.StoreOwnerNotification";
            LocalizedMessageTemplate localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Subject, languageId);
            string body = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Body, languageId);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendQuantityBelowStoreOwnerNotification(ProductVariant productVariant, int languageId)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            string templateName = "QuantityBelow.StoreOwnerNotification";
            LocalizedMessageTemplate localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(productVariant, localizedMessageTemplate.Subject, languageId);
            string body = ReplaceMessageTemplateTokens(productVariant, localizedMessageTemplate.Body, languageId);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends an order placed notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderPlacedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            string templateName = "OrderPlaced.CustomerNotification";
            LocalizedMessageTemplate localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Subject, languageId);
            string body = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Body, languageId);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(order.BillingEmail, order.BillingFullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends an order shipped notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderShippedCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            string templateName = "OrderShipped.CustomerNotification";
            LocalizedMessageTemplate localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Subject, languageId);
            string body = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Body, languageId);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(order.BillingEmail, order.BillingFullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends an order delivered notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderDeliveredCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            string templateName = "OrderDelivered.CustomerNotification";
            LocalizedMessageTemplate localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Subject, languageId);
            string body = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Body, languageId);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(order.BillingEmail, order.BillingFullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends an order cancelled notification to a customer
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendOrderCancelledCustomerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            string templateName = "OrderCancelled.CustomerNotification";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Subject, languageId);
            string body = ReplaceMessageTemplateTokens(order, localizedMessageTemplate.Body, languageId);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(order.BillingEmail, order.BillingFullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a welcome message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendCustomerWelcomeMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");


            string templateName = "Customer.WelcomeMessage";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(customer, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(customer, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(customer.Email, customer.FullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends an email validation message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendCustomerEmailValidationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");


            string templateName = "Customer.EmailValidationMessage";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(customer, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(customer, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(customer.Email, customer.FullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends password recovery message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendCustomerPasswordRecoveryMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            string templateName = "Customer.PasswordRecovery";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(customer, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(customer, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(customer.Email, customer.FullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends 'New customer' notification message to a store owner
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewCustomerNotificationMessage(Customer customer, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            string templateName = "NewCustomer.Notification";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(customer, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(customer, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a "new VAT sumitted" notification to a store owner
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewVATSubmittedStoreOwnerNotification(Customer customer, 
            string vatName, string vatAddress, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            string templateName = "NewVATSubmitted.StoreOwnerNotification";
            LocalizedMessageTemplate localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;
            var additinalKeys = new NameValueCollection();
            additinalKeys.Add("VatValidationResult.Name", vatName);
            additinalKeys.Add("VatValidationResult.Address", vatAddress);
            string subject = ReplaceMessageTemplateTokens(customer, localizedMessageTemplate.Subject, additinalKeys);
            string body = ReplaceMessageTemplateTokens(customer, localizedMessageTemplate.Body, additinalKeys);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends "email a friend" message
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="product">Product instance</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public int SendProductEmailAFriendMessage(Customer customer, int languageId, 
            Product product, string friendsEmail, string personalMessage)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
            if (product == null)
                throw new ArgumentNullException("product");

            string templateName = "Service.EmailAFriend";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            var additinalKeys = new NameValueCollection();
            additinalKeys.Add("EmailAFriend.PersonalMessage", personalMessage);
            string subject = ReplaceMessageTemplateTokens(customer, product, localizedMessageTemplate.Subject, additinalKeys);
            string body = ReplaceMessageTemplateTokens(customer, product, localizedMessageTemplate.Body, additinalKeys);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(friendsEmail);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends wishlist "email a friend" message
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="cart">Shopping cart</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public int SendWishlistEmailAFriendMessage(Customer customer, 
            ShoppingCart cart, int languageId,
            string friendsEmail, string personalMessage)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");
            if (cart == null)
                throw new ArgumentNullException("cart");

            string templateName = "Wishlist.EmailAFriend";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            var additinalKeys = new NameValueCollection();
            additinalKeys.Add("EmailAFriend.PersonalMessage", personalMessage);
            string subject = ReplaceMessageTemplateTokens(customer, cart, localizedMessageTemplate.Subject, additinalKeys);
            string body = ReplaceMessageTemplateTokens(customer, cart, localizedMessageTemplate.Body, additinalKeys);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(friendsEmail);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a forum subscription message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewForumTopicMessage(Customer customer, 
            ForumTopic forumTopic, Forum forum, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            string templateName = "Forums.NewForumTopic";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(customer, null, forumTopic, forum, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(customer, null, forumTopic, forum, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(customer.Email, customer.FullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a forum subscription message to a customer
        /// </summary>
        /// <param name="customer">Customer instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewForumPostMessage(Customer customer,
            ForumPost forumPost, ForumTopic forumTopic, 
            Forum forum, int languageId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            string templateName = "Forums.NewForumPost";
            LocalizedMessageTemplate localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(customer, forumPost, forumTopic, forum, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(customer, forumPost, forumTopic, forum, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(customer.Email, customer.FullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a news comment notification message to a store owner
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewsCommentNotificationMessage(NewsComment newsComment, int languageId)
        {
            if (newsComment == null)
                throw new ArgumentNullException("newsComment");

            string templateName = "News.NewsComment";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(newsComment, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(newsComment, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a blog comment notification message to a store owner
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendBlogCommentNotificationMessage(BlogComment blogComment, int languageId)
        {
            if (blogComment == null)
                throw new ArgumentNullException("blogComment");

            string templateName = "Blog.BlogComment";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(blogComment, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(blogComment, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a product review notification message to a store owner
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendProductReviewNotificationMessage(ProductReview productReview,
            int languageId)
        {
            if (productReview == null)
                throw new ArgumentNullException("productReview");

            string templateName = "Product.ProductReview";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(productReview, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(productReview, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="newsLetterSubscriptionId">Newsletter subscription identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewsLetterSubscriptionActivationMessage(int newsLetterSubscriptionId,
            int languageId)
        {
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate("NewsLetterSubscription.ActivationMessage", languageId);
            var subscription = GetNewsLetterSubscriptionById(newsLetterSubscriptionId);

            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive || subscription == null)
            {
                return 0;
            }

            var emailAccount = localizedMessageTemplate.EmailAccount;

            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(subscription.Email);
            string subject = ReplaceMessageTemplateTokens(subscription, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(subscription, localizedMessageTemplate.Body);

            var queuedEmail = InsertQueuedEmail(5, from, to, String.Empty,
                localizedMessageTemplate.BccEmailAddresses, subject, body, 
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);

            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="newsLetterSubscriptionId">Newsletter subscription identifier</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewsLetterSubscriptionDeactivationMessage(int newsLetterSubscriptionId, 
            int languageId)
        {
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate("NewsLetterSubscription.DeactivationMessage", languageId);
            var subscription = GetNewsLetterSubscriptionById(newsLetterSubscriptionId);

            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive || subscription == null)
            {
                return 0;
            }

            var emailAccount = localizedMessageTemplate.EmailAccount;

            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(subscription.Email);
            string subject = ReplaceMessageTemplateTokens(subscription, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(subscription, localizedMessageTemplate.Body);

            var queuedEmail = InsertQueuedEmail(5, from, to, String.Empty, 
                localizedMessageTemplate.BccEmailAddresses, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);

            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a gift card notification
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendGiftCardNotification(GiftCard giftCard, int languageId)
        {
            if (giftCard == null)
                throw new ArgumentNullException("giftCard");

            string templateName = "GiftCard.Notification";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if(localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(giftCard, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(giftCard, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(giftCard.RecipientEmail, giftCard.RecipientName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends a private message notification
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendPrivateMessageNotification(PrivateMessage privateMessage, int languageId)
        {
            if (privateMessage == null)
                throw new ArgumentNullException("privateMessage");

            string templateName = "Customer.NewPM";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(privateMessage, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(privateMessage, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);

            var recipient = privateMessage.ToUser;
            if (recipient == null)
                return 0;

            var to = new MailAddress(recipient.Email, recipient.FullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends 'New Return Request' message to a store owner
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendNewReturnRequestStoreOwnerNotification(ReturnRequest returnRequest, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            string templateName = "NewReturnRequest.StoreOwnerNotification";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(returnRequest, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(returnRequest, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Sends 'Return Request status changed' message to a customer
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public int SendReturnRequestStatusChangedCustomerNotification(ReturnRequest returnRequest, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException("returnRequest");

            if (returnRequest.Customer == null)
                throw new NopException("Customer of return request could not be loaded");

            string templateName = "ReturnRequestStatusChanged.CustomerNotification";
            var localizedMessageTemplate = this.GetLocalizedMessageTemplate(templateName, languageId);
            if (localizedMessageTemplate == null || !localizedMessageTemplate.IsActive)
                return 0;

            var emailAccount = localizedMessageTemplate.EmailAccount;

            string subject = ReplaceMessageTemplateTokens(returnRequest, localizedMessageTemplate.Subject);
            string body = ReplaceMessageTemplateTokens(returnRequest, localizedMessageTemplate.Body);
            string bcc = localizedMessageTemplate.BccEmailAddresses;
            var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
            var to = new MailAddress(returnRequest.Customer.Email, returnRequest.Customer.FullName);
            var queuedEmail = InsertQueuedEmail(5, from, to, string.Empty, bcc, subject, body,
                DateTime.UtcNow, 0, null, emailAccount.EmailAccountId);
            return queuedEmail.QueuedEmailId;
        }

        /// <summary>
        /// Gets list of allowed (supported) message tokens
        /// </summary>
        /// <returns></returns>
        public string[] GetListOfAllowedTokens()
        {
            var allowedTokens = new List<string>();

            allowedTokens.Add("%Store.Name%");
            allowedTokens.Add("%Store.URL%");
            allowedTokens.Add("%Store.Email%");

            allowedTokens.Add("%Order.OrderNumber%");
            allowedTokens.Add("%Order.CustomerFullName%");
            allowedTokens.Add("%Order.CustomerEmail%");
            allowedTokens.Add("%Order.BillingFirstName%");
            allowedTokens.Add("%Order.BillingLastName%");
            allowedTokens.Add("%Order.BillingPhoneNumber%");
            allowedTokens.Add("%Order.BillingEmail%");
            allowedTokens.Add("%Order.BillingFaxNumber%");
            allowedTokens.Add("%Order.BillingCompany%");
            allowedTokens.Add("%Order.BillingAddress1%");
            allowedTokens.Add("%Order.BillingAddress2%");
            allowedTokens.Add("%Order.BillingCity%");
            allowedTokens.Add("%Order.BillingStateProvince%");
            allowedTokens.Add("%Order.BillingZipPostalCode%");
            allowedTokens.Add("%Order.BillingCountry%");
            allowedTokens.Add("%Order.ShippingMethod%");
            allowedTokens.Add("%Order.ShippingFirstName%");
            allowedTokens.Add("%Order.ShippingLastName%");
            allowedTokens.Add("%Order.ShippingPhoneNumber%");
            allowedTokens.Add("%Order.ShippingEmail%");
            allowedTokens.Add("%Order.ShippingFaxNumber%");
            allowedTokens.Add("%Order.ShippingCompany%");
            allowedTokens.Add("%Order.ShippingAddress1%");
            allowedTokens.Add("%Order.ShippingAddress2%");
            allowedTokens.Add("%Order.ShippingCity%");
            allowedTokens.Add("%Order.ShippingStateProvince%");
            allowedTokens.Add("%Order.ShippingZipPostalCode%");
            allowedTokens.Add("%Order.ShippingCountry%");
            allowedTokens.Add("%Order.TrackingNumber%");
            allowedTokens.Add("%Order.VatNumber%");
            allowedTokens.Add("%Order.Product(s)%");
            allowedTokens.Add("%Order.CreatedOn%");
            allowedTokens.Add("%Order.OrderURLForCustomer%");

            allowedTokens.Add("%Customer.Email%");
            allowedTokens.Add("%Customer.Username%");
            allowedTokens.Add("%Customer.PasswordRecoveryURL%");
            allowedTokens.Add("%Customer.AccountActivationURL%");
            allowedTokens.Add("%Customer.FullName%");
            allowedTokens.Add("%Customer.VatNumber%");
            allowedTokens.Add("%Customer.VatNumberStatus%");

            allowedTokens.Add("%Product.Name%");
            allowedTokens.Add("%Product.ShortDescription%");
            allowedTokens.Add("%Product.ProductURLForCustomer%");
            allowedTokens.Add("%ProductVariant.FullProductName%");
            allowedTokens.Add("%ProductVariant.StockQuantity%");

            allowedTokens.Add("%Wishlist.URLForCustomer%");

            allowedTokens.Add("%NewsComment.NewsTitle%");
            allowedTokens.Add("%BlogComment.BlogPostTitle%");

            allowedTokens.Add("%NewsLetterSubscription.Email%");
            allowedTokens.Add("%NewsLetterSubscription.ActivationUrl%");
            allowedTokens.Add("%NewsLetterSubscription.DeactivationUrl%");

            allowedTokens.Add("%GiftCard.SenderName%");
            allowedTokens.Add("%GiftCard.SenderEmail%");
            allowedTokens.Add("%GiftCard.RecipientName%");
            allowedTokens.Add("%GiftCard.RecipientEmail%");
            allowedTokens.Add("%GiftCard.Amount%");
            allowedTokens.Add("%GiftCard.CouponCode%");
            allowedTokens.Add("%GiftCard.Message%");

            allowedTokens.Add("%Forums.TopicURL%");
            allowedTokens.Add("%Forums.TopicName%");
            allowedTokens.Add("%Forums.ForumURL%");
            allowedTokens.Add("%Forums.ForumName%");
            allowedTokens.Add("%Forums.PostAuthor%");
            allowedTokens.Add("%Forums.PostBody%");

            allowedTokens.Add("%PrivateMessage.Subject%");
            allowedTokens.Add("%PrivateMessage.Text%");

            allowedTokens.Add("%ReturnRequest.ID%");
            allowedTokens.Add("%ReturnRequest.Product.Quantity%");
            allowedTokens.Add("%ReturnRequest.Product.Name%");
            allowedTokens.Add("%ReturnRequest.Reason%");
            allowedTokens.Add("%ReturnRequest.RequestedAction%");
            allowedTokens.Add("%ReturnRequest.CustomerComment%");

            allowedTokens.Add("%Wishlist.URLForCustomer%");

            allowedTokens.Add("%VatValidationResult.Name%");
            allowedTokens.Add("%VatValidationResult.Address%");

            return allowedTokens.ToArray();
        }

        /// <summary>
        /// Gets list of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>List of allowed (supported) message tokens for campaigns</returns>
        public string[] GetListOfCampaignAllowedTokens()
        {
            var allowedTokens = new List<string>();
            allowedTokens.Add("%Store.Name%");
            allowedTokens.Add("%Store.URL%");
            allowedTokens.Add("%Store.Email%");
            allowedTokens.Add("%NewsLetterSubscription.Email%");
            allowedTokens.Add("%NewsLetterSubscription.ActivationUrl%");
            allowedTokens.Add("%NewsLetterSubscription.DeactivationUrl%");
            return allowedTokens.ToArray();
        }

        /// <summary>
        /// Replaces a message template tokens
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="template">Template</param>
        /// <returns>New template</returns>
        public string ReplaceMessageTemplateTokens(NewsLetterSubscription subscription, 
            string template)
        {
            var tokens = new NameValueCollection();

            tokens.Add("Store.Name", IoC.Resolve<ISettingManager>().StoreName);
            tokens.Add("Store.URL", IoC.Resolve<ISettingManager>().StoreUrl);
            tokens.Add("Store.Email", this.DefaultEmailAccount.Email);
            tokens.Add("NewsLetterSubscription.Email", HttpUtility.HtmlEncode(subscription.Email));
            tokens.Add("NewsLetterSubscription.ActivationUrl", String.Format("{0}newslettersubscriptionactivation.aspx?t={1}&active=1", IoC.Resolve<ISettingManager>().StoreUrl, subscription.NewsLetterSubscriptionGuid));
            tokens.Add("NewsLetterSubscription.DeactivationUrl", String.Format("{0}newslettersubscriptionactivation.aspx?t={1}&active=0", IoC.Resolve<ISettingManager>().StoreUrl, subscription.NewsLetterSubscriptionGuid));

            var customer = subscription.Customer;
            if(customer != null)
            {
                template = ReplaceMessageTemplateTokens(customer, template);
            }

            foreach(string token in tokens.Keys)
            {
                template = Replace(template, String.Format(@"%{0}%", token), tokens[token]);
            }

            return template;
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="emailAccount">Email account to use</param>
        public void SendEmail(string subject, string body, string from, string to, 
            EmailAccount emailAccount)
        {
            SendEmail(subject, body, new MailAddress(from), new MailAddress(to),
                new List<String>(), new List<String>(), emailAccount);
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="emailAccount">Email account to use</param>
        public void SendEmail(string subject, string body, MailAddress from,
            MailAddress to, EmailAccount emailAccount)
        {
            SendEmail(subject, body, from, to, new List<String>(), new List<String>(), emailAccount);
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="bcc">BCC</param>
        /// <param name="cc">CC</param>
        /// <param name="emailAccount">Email account to use</param>
        public void SendEmail(string subject, string body,
            MailAddress from, MailAddress to, List<string> bcc, 
            List<string> cc, EmailAccount emailAccount)
        {
            var message = new MailMessage();
            message.From = from;
            message.To.Add(to);
            if (null != bcc)
                foreach (string address in bcc)
                {
                    if (address != null)
                    {
                        if (!String.IsNullOrEmpty(address.Trim()))
                        {
                            message.Bcc.Add(address.Trim());
                        }
                    }
                }
            if (null != cc)
                foreach (string address in cc)
                {
                    if (address != null)
                    {
                        if (!String.IsNullOrEmpty(address.Trim()))
                        {
                            message.CC.Add(address.Trim());
                        }
                    }
                }
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient();
            smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
            smtpClient.Host = emailAccount.Host;
            smtpClient.Port = emailAccount.Port;
            smtpClient.EnableSsl = emailAccount.EnableSSL;
            if (emailAccount.UseDefaultCredentials)
                smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
            else
                smtpClient.Credentials = new NetworkCredential(emailAccount.Username, emailAccount.Password);
            smtpClient.Send(message);
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a primary store currency
        /// </summary>
        public EmailAccount DefaultEmailAccount
        {
            get
            {
                int defaultEmailAccountId = IoC.Resolve<ISettingManager>().GetSettingValueInteger("EmailAccount.DefaultEmailAccountId");
                var emailAccount = GetEmailAccountById(defaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = GetAllEmailAccounts().FirstOrDefault();

                return emailAccount;
            }
            set
            {
                if (value != null)
                    IoC.Resolve<ISettingManager>().SetParam("EmailAccount.DefaultEmailAccountId", value.EmailAccountId.ToString());
            }
        }

        #endregion
    }
}