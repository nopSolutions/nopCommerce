using System;
using System.Text;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Html;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Tax;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Checkout attribute helper
    /// </summary>
    public partial class CheckoutAttributeFormatter : ICheckoutAttributeFormatter
    {
        private readonly IWorkContext _workContext;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly ITaxService _taxService;
        private readonly IPriceFormatter _priceFormatter;

        public CheckoutAttributeFormatter(IWorkContext workContext,
            ICheckoutAttributeService checkoutAttributeService,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            ITaxService taxService,
            IPriceFormatter priceFormatter)
        {
            this._workContext = workContext;
            this._checkoutAttributeService = checkoutAttributeService;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._currencyService = currencyService;
            this._taxService = taxService;
            this._priceFormatter = priceFormatter;
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <returns>Attributes</returns>
        public string FormatAttributes(string attributes)
        {
            var customer = _workContext.CurrentCustomer;
            return FormatAttributes(attributes, customer);
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <returns>Attributes</returns>
        public string FormatAttributes(string attributes,
            Customer customer, 
            string serapator = "<br />", 
            bool htmlEncode = true, 
            bool renderPrices = true)
        {
            var result = new StringBuilder();

            var caCollection = _checkoutAttributeParser.ParseCheckoutAttributes(attributes);
            for (int i = 0; i < caCollection.Count; i++)
            {
                var ca = caCollection[i];
                var valuesStr = _checkoutAttributeParser.ParseValues(attributes, ca.Id);
                for (int j = 0; j < valuesStr.Count; j++)
                {
                    string valueStr = valuesStr[j];
                    string caAttribute = string.Empty;
                    if (!ca.ShouldHaveValues())
                    {
                        if (ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            caAttribute = string.Format("{0}: {1}", ca.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), HtmlHelper.FormatText(valueStr, false, true, true, false, false, false));
                        }
                        else
                        {
                            caAttribute = string.Format("{0}: {1}", ca.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), valueStr);
                        }
                    }
                    else
                    {
                        int caId = 0;
                        if (int.TryParse(valueStr, out caId))
                        {
                            var caValue = _checkoutAttributeService.GetCheckoutAttributeValueById(caId);
                            if (caValue != null)
                            {
                                caAttribute = string.Format("{0}: {1}", ca.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), caValue.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id));
                                if (renderPrices)
                                {
                                    decimal priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(caValue, customer);
                                    decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                                    if (priceAdjustmentBase > 0)
                                    {
                                        string priceAdjustmentStr = _priceFormatter.FormatPrice(priceAdjustment);
                                        caAttribute += string.Format(" [+{0}]", priceAdjustmentStr);
                                    }
                                }
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(caAttribute))
                    {
                        if (i != 0 || j != 0)
                        {
                            result.Append(serapator);
                        }
                        
                        //we don't encode multiline textbox input
                        if (htmlEncode &&
                            ca.AttributeControlType != AttributeControlType.MultilineTextbox)
                        {
                            result.Append(HttpUtility.HtmlEncode(caAttribute));
                        }
                        else
                        {
                            result.Append(caAttribute);
                        }
                    }
                }
            }

            return result.ToString();
        }
    }
}
