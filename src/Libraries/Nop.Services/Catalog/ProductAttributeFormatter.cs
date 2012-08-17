using System;
using System.Text;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Html;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
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
        private readonly IDownloadService _downloadService;
        private readonly IWebHelper _webHelper;

        public ProductAttributeFormatter(IWorkContext workContext,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            ITaxService taxService,
            IPriceFormatter priceFormatter,
            IDownloadService downloadService,
            IWebHelper webHelper)
        {
            this._workContext = workContext;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._priceFormatter = priceFormatter;
            this._downloadService = downloadService;
            this._webHelper = webHelper;
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
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>Attributes</returns>
        public string FormatAttributes(ProductVariant productVariant, string attributes,
            Customer customer, string serapator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderProductAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true)
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
                            //no values
                            if (pva.AttributeControlType == AttributeControlType.MultilineTextbox)
                            {
                                //multiline textbox
                                var attributeName = pva.ProductAttribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id);
                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = HttpUtility.HtmlEncode(attributeName);
                                pvaAttribute = string.Format("{0}: {1}", attributeName, HtmlHelper.FormatText(valueStr, false, true, false, false, false, false));
                                //we never encode multiline textbox input
                            }
                            else if (pva.AttributeControlType == AttributeControlType.FileUpload)
                            {
                                //file upload
                                Guid downloadGuid;
                                Guid.TryParse(valueStr, out downloadGuid);
                                var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                {
                                    //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                                    string attributeText = "";
                                    var fileName = string.Format("{0}{1}", 
                                        download.Filename ?? download.DownloadGuid.ToString(),
                                        download.Extension);
                                    //encode (if required)
                                    if (htmlEncode)
                                        fileName = HttpUtility.HtmlEncode(fileName);
                                    if (allowHyperlinks)
                                    {
                                        //hyperlinks are allowed
                                        var downloadLink = string.Format("{0}download/getfileupload/?downloadId={1}", _webHelper.GetStoreLocation(false), download.DownloadGuid);
                                        attributeText = string.Format("<a href=\"{0}\" class=\"fileuploadattribute\">{1}</a>", downloadLink, fileName);
                                    }
                                    else
                                    {
                                        //hyperlinks aren't allowed
                                        attributeText = fileName;
                                    }
                                    var attributeName = pva.ProductAttribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id);
                                    //encode (if required)
                                    if (htmlEncode)
                                        attributeName = HttpUtility.HtmlEncode(attributeName);
                                    pvaAttribute = string.Format("{0}: {1}", attributeName, attributeText);
                                }
                            }
                            else
                            {
                                //other attributes (textbox, datepicker)
                                pvaAttribute = string.Format("{0}: {1}", pva.ProductAttribute.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), valueStr);
                                //encode (if required)
                                if (htmlEncode)
                                    pvaAttribute = HttpUtility.HtmlEncode(pvaAttribute);
                            }
                        }
                        else
                        {
                            //attributes with values
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
                                //encode (if required)
                                if (htmlEncode)
                                    pvaAttribute = HttpUtility.HtmlEncode(pvaAttribute);
                            }
                        }

                        if (!String.IsNullOrEmpty(pvaAttribute))
                        {
                            if (i != 0 || j != 0)
                                result.Append(serapator);
                            result.Append(pvaAttribute);
                        }
                    }
                }
            }

            //gift cards
            if (renderGiftCardAttributes)
            {
                if (productVariant.IsGiftCard)
                {
                    string giftCardRecipientName = "";
                    string giftCardRecipientEmail = "";
                    string giftCardSenderName = "";
                    string giftCardSenderEmail = "";
                    string giftCardMessage = "";
                    _productAttributeParser.GetGiftCardAttribute(attributes, out giftCardRecipientName, out giftCardRecipientEmail,
                        out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                    if (!String.IsNullOrEmpty(result.ToString()))
                    {
                        result.Append(serapator);
                    }
                    //encode (if required)
                    if (htmlEncode)
                    {
                        giftCardRecipientName = HttpUtility.HtmlEncode(giftCardRecipientName);
                        giftCardSenderName = HttpUtility.HtmlEncode(giftCardSenderName);
                    }

                    result.Append(string.Format(_localizationService.GetResource("GiftCardAttribute.For"), giftCardRecipientName));
                    result.Append(serapator);
                    result.Append(string.Format(_localizationService.GetResource("GiftCardAttribute.From"), giftCardSenderName));
                }
            }
            return result.ToString();
        }
    }
}
