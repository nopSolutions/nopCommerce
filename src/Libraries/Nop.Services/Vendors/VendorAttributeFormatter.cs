using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Html;
using Nop.Services.Localization;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents a vendor attribute formatter implementation
    /// </summary>
    public partial class VendorAttributeFormatter : IVendorAttributeFormatter
    {
        #region Fields

        protected ILocalizationService LocalizationService { get; }
        private readonly INopHtmlHelper _nopHtmlHelper;
        protected IVendorAttributeParser VendorAttributeParser { get; }
        protected IVendorAttributeService VendorAttributeService { get; }
        protected IWorkContext WorkContext { get; }

        #endregion

        #region Ctor

        public VendorAttributeFormatter(ILocalizationService localizationService,
            INopHtmlHelper nopHtmlHelper,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeService vendorAttributeService,
            IWorkContext workContext)
        {
            LocalizationService = localizationService;
            _nopHtmlHelper = nopHtmlHelper;
            VendorAttributeParser = vendorAttributeParser;
            VendorAttributeService = vendorAttributeService;
            WorkContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Format vendor attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the formatted attributes
        /// </returns>
        public virtual async Task<string> FormatAttributesAsync(string attributesXml, string separator = "<br />", bool htmlEncode = true)
        {
            var result = new StringBuilder();
            var currentLanguage = await WorkContext.GetWorkingLanguageAsync();
            var attributes = await VendorAttributeParser.ParseVendorAttributesAsync(attributesXml);
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var valuesStr = VendorAttributeParser.ParseValues(attributesXml, attribute.Id);
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
                            formattedAttribute = $"{attributeName}: {_nopHtmlHelper.FormatText(valueStr, false, true, false, false, false, false)}";
                            //we never encode multiline textbox input
                        }
                        else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                        {
                            //file upload
                            //not supported for vendor attributes
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
                            var attributeValue = await VendorAttributeService.GetVendorAttributeValueByIdAsync(attributeValueId);
                            if (attributeValue != null)
                            {
                                formattedAttribute = $"{await LocalizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await LocalizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";
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