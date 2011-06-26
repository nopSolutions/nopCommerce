using System;
using System.Text;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Html;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Tax;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute formatter
    /// </summary>
    public partial class ProductAttributeFormatter : IProductAttributeFormatter
    {
        private readonly IWorkContext _workContext;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly IPriceFormatter _priceFormatter;

        public ProductAttributeFormatter(IWorkContext workContext,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            ITaxService taxService,
            IPriceFormatter priceFormatter)
        {
            this._workContext = workContext;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._priceFormatter = priceFormatter;
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="attributes">Attributes</param>
        /// <returns>Attributes</returns>
        public string FormatAttributes(ProductVariant productVariant, string attributes)
        {
            var customer = _workContext.CurrentCustomer;
            return FormatAttributes(productVariant, attributes, customer);
        }
        
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="attributes">Attributes</param>
        /// <param name="customer">Customer</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="renderProductAttributes">A value indicating whether to render product attributes</param>
        /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
        /// <returns>Attributes</returns>
        public string FormatAttributes(ProductVariant productVariant, string attributes,
            Customer customer, string serapator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderProductAttributes = true, bool renderGiftCardAttributes = true)
        {
            var result = new StringBuilder();

            //attributes
            if (renderProductAttributes)
            {
                var pvaCollection = _productAttributeParser.ParseProductVariantAttributes(attributes);
                for (int i = 0; i < pvaCollection.Count; i++)
                {
                    var pva = pvaCollection[i];
                    var valuesStr = _productAttributeParser.ParseValues(attributes, pva.Id);
                    for (int j = 0; j < valuesStr.Count; j++)
                    {
                        string valueStr = valuesStr[j];
                        string pvaAttribute = string.Empty;
                        if (!pva.ShouldHaveValues())
                        {
                            if (pva.AttributeControlType == AttributeControlType.MultilineTextbox)
                            {
                                pvaAttribute = string.Format("{0}: {1}", pva.ProductAttribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), HtmlHelper.FormatText(valueStr, false, true, true, false, false, false));
                            }
                            else
                            {
                                pvaAttribute = string.Format("{0}: {1}", pva.ProductAttribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), valueStr);
                            }
                        }
                        else
                        {
                            int pvaId = 0;
                            if (int.TryParse(valueStr, out pvaId))
                            {
                                var pvaValue = _productAttributeService.GetProductVariantAttributeValueById(pvaId);
                                if (pvaValue != null)
                                {
                                    pvaAttribute = string.Format("{0}: {1}", pva.ProductAttribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), pvaValue.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id));
                                    if (renderPrices)
                                    {
                                        decimal taxRate = decimal.Zero;
                                        decimal priceAdjustmentBase = _taxService.GetProductPrice(productVariant, pvaValue.PriceAdjustment, customer, out taxRate);
                                        decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                                        if (priceAdjustmentBase > 0)
                                        {
                                            string priceAdjustmentStr = _priceFormatter.FormatPrice(priceAdjustment, false, false);
                                            pvaAttribute += string.Format(" [+{0}]", priceAdjustmentStr);
                                        }
                                        else if (priceAdjustmentBase < decimal.Zero)
                                        {
                                            string priceAdjustmentStr = _priceFormatter.FormatPrice(-priceAdjustment, false, false);
                                            pvaAttribute += string.Format(" [-{0}]", priceAdjustmentStr);
                                        }
                                    }
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(pvaAttribute))
                        {
                            if (i != 0 || j != 0)
                            {
                                result.Append(serapator);
                            }

                            //we don't encode multiline textbox input
                            if (htmlEncode &&
                                pva.AttributeControlType != AttributeControlType.MultilineTextbox)
                            {
                                result.Append(HttpUtility.HtmlEncode(pvaAttribute));
                            }
                            else
                            {
                                result.Append(pvaAttribute);
                            }
                        }
                    }
                }
            }

            //gift cards
            if (renderGiftCardAttributes)
            {
                if (productVariant.IsGiftCard)
                {
                    string giftCardRecipientName = string.Empty;
                    string giftCardRecipientEmail = string.Empty;
                    string giftCardSenderName = string.Empty;
                    string giftCardSenderEmail = string.Empty;
                    string giftCardMessage = string.Empty;
                    _productAttributeParser.GetGiftCardAttribute(attributes, out giftCardRecipientName, out giftCardRecipientEmail,
                        out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                    if (!String.IsNullOrEmpty(result.ToString()))
                    {
                        result.Append(serapator);
                    }

                    if (htmlEncode)
                    {
                        result.Append(HttpUtility.HtmlEncode(string.Format(_localizationService.GetResource("GiftCardAttribute.For"), giftCardRecipientName)));
                        result.Append(serapator);
                        result.Append(HttpUtility.HtmlEncode(string.Format(_localizationService.GetResource("GiftCardAttribute.From"), giftCardSenderName)));
                    }
                    else
                    {
                        result.Append(string.Format(_localizationService.GetResource("GiftCardAttribute.For"), giftCardRecipientName));
                        result.Append(serapator);
                        result.Append(string.Format(_localizationService.GetResource("GiftCardAttribute.From"), giftCardSenderName));
                    }
                }
            }
            return result.ToString();
        }
    }
}
