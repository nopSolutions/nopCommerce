using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Html;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Tax;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Checkout attribute helper
    /// </summary>
    public partial class CheckoutAttributeFormatter : ICheckoutAttributeFormatter
    {
        #region Fields

        protected ICheckoutAttributeParser CheckoutAttributeParser { get; }
        protected ICheckoutAttributeService CheckoutAttributeService { get; }
        protected ICurrencyService CurrencyService { get; }
        protected IDownloadService DownloadService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IPriceFormatter PriceFormatter { get; }
        protected ITaxService TaxService { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public CheckoutAttributeFormatter(ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IPriceFormatter priceFormatter,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            CheckoutAttributeParser = checkoutAttributeParser;
            CheckoutAttributeService = checkoutAttributeService;
            CurrencyService = currencyService;
            DownloadService = downloadService;
            LocalizationService = localizationService;
            PriceFormatter = priceFormatter;
            TaxService = taxService;
            WebHelper = webHelper;
            WorkContext = workContext;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customer">Customer</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperlink tags could be rendered (if required)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the attributes
        /// </returns>
        public virtual async Task<string> FormatAttributesAsync(string attributesXml,
            Customer customer,
            string separator = "<br />",
            bool htmlEncode = true,
            bool renderPrices = true,
            bool allowHyperlinks = true)
        {
            var result = new StringBuilder();
            var currentLanguage = WorkContext.GetWorkingLanguageAsync();
            var attributes = await CheckoutAttributeParser.ParseCheckoutAttributesAsync(attributesXml);
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var valuesStr = CheckoutAttributeParser.ParseValues(attributesXml, attribute.Id);
                for (var j = 0; j < valuesStr.Count; j++)
                {
                    var valueStr = valuesStr[j];
                    var formattedAttribute = string.Empty;
                    if (!attribute.ShouldHaveValues())
                    {
                        //no values
                        if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            //multiline textbox
                            var attributeName = await LocalizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
                            //encode (if required)
                            if (htmlEncode)
                                attributeName = WebUtility.HtmlEncode(attributeName);
                            formattedAttribute = $"{attributeName}: {HtmlHelper.FormatText(valueStr, false, true, false, false, false, false)}";
                            //we never encode multiline textbox input
                        }
                        else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                        {
                            //file upload
                            Guid.TryParse(valueStr, out var downloadGuid);
                            var download = await DownloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                            {
                                string attributeText;
                                var fileName = $"{download.Filename ?? download.DownloadGuid.ToString()}{download.Extension}";
                                //encode (if required)
                                if (htmlEncode)
                                    fileName = WebUtility.HtmlEncode(fileName);
                                if (allowHyperlinks)
                                {
                                    //hyperlinks are allowed
                                    var downloadLink = $"{WebHelper.GetStoreLocation()}download/getfileupload/?downloadId={download.DownloadGuid}";
                                    attributeText = $"<a href=\"{downloadLink}\" class=\"fileuploadattribute\">{fileName}</a>";
                                }
                                else
                                {
                                    //hyperlinks aren't allowed
                                    attributeText = fileName;
                                }

                                var attributeName = await LocalizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = WebUtility.HtmlEncode(attributeName);
                                formattedAttribute = $"{attributeName}: {attributeText}";
                            }
                        }
                        else
                        {
                            //other attributes (textbox, datepicker)
                            formattedAttribute = $"{await LocalizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {valueStr}";
                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                        }
                    }
                    else
                    {
                        if (int.TryParse(valueStr, out var attributeValueId))
                        {
                            var attributeValue = await CheckoutAttributeService.GetCheckoutAttributeValueByIdAsync(attributeValueId);

                            if (attributeValue != null)
                            {
                                formattedAttribute = $"{await LocalizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await LocalizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";
                                if (renderPrices)
                                {
                                    var priceAdjustmentBase = (await TaxService.GetCheckoutAttributePriceAsync(attribute, attributeValue, customer)).price;
                                    var priceAdjustment = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase, await WorkContext.GetWorkingCurrencyAsync());
                                    if (priceAdjustmentBase > 0)
                                    {
                                        formattedAttribute += string.Format(
                                                await LocalizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                                "+", await PriceFormatter.FormatPriceAsync(priceAdjustment), string.Empty);
                                    }
                                }
                            }

                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                        }
                    }

                    if (string.IsNullOrEmpty(formattedAttribute))
                        continue;

                    if (i != 0 || j != 0)
                        result.Append(separator);
                    result.Append(formattedAttribute);
                }
            }

            return result.ToString();
        }

        #endregion
    }
}