using System.Net;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Attributes;
using Nop.Core.Domain.Catalog;
using Nop.Services.Html;
using Nop.Services.Localization;

namespace Nop.Services.Attributes
{
    /// <summary>
    /// Attribute formatter
    /// </summary>
    public partial class
        AttributeFormatter<TAttribute, TAttributeValue> : IAttributeFormatter<TAttribute, TAttributeValue>
        where TAttribute : BaseAttribute
        where TAttributeValue : BaseAttributeValue
    {
        #region Fields

        protected readonly IAttributeParser<TAttribute, TAttributeValue> _attributeParser;
        protected readonly IAttributeService<TAttribute, TAttributeValue> _attributeService;
        protected readonly IHtmlFormatter _htmlFormatter;
        protected readonly ILocalizationService _localizationService;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AttributeFormatter(IAttributeParser<TAttribute, TAttributeValue> attributeParser,
            IAttributeService<TAttribute, TAttributeValue> attributeService,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _attributeParser = attributeParser;
            _attributeService = attributeService;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the attributes
        /// </returns>
        public virtual async Task<string> FormatAttributesAsync(string attributesXml,
            string separator = "<br />",
            bool htmlEncode = true)
        {
            var result = new StringBuilder();
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            var attributes = await _attributeParser.ParseAttributesAsync(attributesXml);

            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var valuesStr = _attributeParser.ParseValues(attributesXml, attribute.Id);

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
                            var attributeName =
                                await _localizationService.GetLocalizedAsync(attribute, a => a.Name,
                                    currentLanguage.Id);

                            //encode (if required)
                            if (htmlEncode)
                                attributeName = WebUtility.HtmlEncode(attributeName);

                            formattedAttribute =
                                $"{attributeName}: {_htmlFormatter.FormatText(valueStr, false, true, false, false, false, false)}";
                            //we never encode multiline textbox input
                        }
                        else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                        {
                            //file upload
                            //not supported for this attributes
                        }
                        else
                        {
                            //other attributes (textbox, datepicker)
                            formattedAttribute =
                                $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {valueStr}";

                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                        }
                    }
                    else
                    {
                        if (int.TryParse(valueStr, out var attributeValueId))
                        {
                            var attributeValue = await _attributeService.GetAttributeValueByIdAsync(attributeValueId);

                            if (attributeValue != null)
                                formattedAttribute =
                                    $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";

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