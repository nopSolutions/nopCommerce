using System.Net;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Tax;

namespace Nop.Services.Orders;

/// <summary>
/// Checkout attribute helper
/// </summary>
public partial class CheckoutAttributeFormatter : ICheckoutAttributeFormatter
{
    #region Fields

    protected readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    protected readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    protected readonly ICurrencyService _currencyService;
    protected readonly IDownloadService _downloadService;
    protected readonly IHtmlFormatter _htmlFormatter;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly ITaxService _taxService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CheckoutAttributeFormatter(IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        ICurrencyService currencyService,
        IDownloadService downloadService,
        IHtmlFormatter htmlFormatter,
        ILocalizationService localizationService,
        IPriceFormatter priceFormatter,
        ITaxService taxService,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        _checkoutAttributeParser = checkoutAttributeParser;
        _checkoutAttributeService = checkoutAttributeService;
        _currencyService = currencyService;
        _downloadService = downloadService;
        _htmlFormatter = htmlFormatter;
        _localizationService = localizationService;
        _priceFormatter = priceFormatter;
        _taxService = taxService;
        _webHelper = webHelper;
        _workContext = workContext;
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
        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
        var attributes = await _checkoutAttributeParser.ParseAttributesAsync(attributesXml);
        for (var i = 0; i < attributes.Count; i++)
        {
            var attribute = attributes[i];
            var valuesStr = _checkoutAttributeParser.ParseValues(attributesXml, attribute.Id);
            for (var j = 0; j < valuesStr.Count; j++)
            {
                var valueStr = valuesStr[j];
                var formattedAttribute = string.Empty;
                if (!attribute.ShouldHaveValues)
                {
                    //no values
                    if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        //multiline textbox
                        var attributeName = await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
                        //encode (if required)
                        if (htmlEncode)
                            attributeName = WebUtility.HtmlEncode(attributeName);
                        formattedAttribute = $"{attributeName}: {_htmlFormatter.FormatText(valueStr, false, true, false, false, false, false)}";
                        //we never encode multiline textbox input
                    }
                    else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                    {
                        //file upload
                        _ = Guid.TryParse(valueStr, out var downloadGuid);
                        var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
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
                                var downloadLink = $"{_webHelper.GetStoreLocation()}download/getfileupload/?downloadId={download.DownloadGuid}";
                                attributeText = $"<a href=\"{downloadLink}\" class=\"fileuploadattribute\">{fileName}</a>";
                            }
                            else
                            {
                                //hyperlinks aren't allowed
                                attributeText = fileName;
                            }

                            var attributeName = await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
                            //encode (if required)
                            if (htmlEncode)
                                attributeName = WebUtility.HtmlEncode(attributeName);
                            formattedAttribute = $"{attributeName}: {attributeText}";
                        }
                    }
                    else
                    {
                        //other attributes (textbox, datepicker)
                        formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {valueStr}";
                        //encode (if required)
                        if (htmlEncode)
                            formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                    }
                }
                else
                {
                    if (int.TryParse(valueStr, out var attributeValueId))
                    {
                        var attributeValue = await _checkoutAttributeService.GetAttributeValueByIdAsync(attributeValueId);

                        if (attributeValue != null)
                        {
                            formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";
                            if (renderPrices)
                            {
                                var priceAdjustmentBase = (await _taxService.GetCheckoutAttributePriceAsync(attribute, attributeValue, customer)).price;
                                var priceAdjustment = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase, await _workContext.GetWorkingCurrencyAsync());
                                if (priceAdjustmentBase > 0)
                                {
                                    formattedAttribute += string.Format(
                                        await _localizationService.GetResourceAsync("FormattedAttributes.PriceAdjustment"),
                                        "+", await _priceFormatter.FormatPriceAsync(priceAdjustment), string.Empty);
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