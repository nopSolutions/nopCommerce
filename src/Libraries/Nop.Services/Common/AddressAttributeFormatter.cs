using System;
using System.Text;
using System.Web;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Html;
using Nop.Services.Localization;
using Nop.Services.Tax;

namespace Nop.Services.Common
{
    /// <summary>
    /// Address attribute helper
    /// </summary>
    public partial class AddressAttributeFormatter : IAddressAttributeFormatter
    {
        private readonly IWorkContext _workContext;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly IAddressAttributeParser _addressAttributeParser;

        public AddressAttributeFormatter(IWorkContext workContext,
            IAddressAttributeService addressAttributeService,
            IAddressAttributeParser addressAttributeParser)
        {
            this._workContext = workContext;
            this._addressAttributeService = addressAttributeService;
            this._addressAttributeParser = addressAttributeParser;
        }
        
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="attributes">Attributes</param>
        /// <param name="serapator">Serapator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>Attributes</returns>
        public string FormatAttributes(string attributes,
            string serapator = "<br />", 
            bool htmlEncode = true)
        {
            var result = new StringBuilder();

            var aaCollection = _addressAttributeParser.ParseAddressAttributes(attributes);
            for (int i = 0; i < aaCollection.Count; i++)
            {
                var aa = aaCollection[i];
                var valuesStr = _addressAttributeParser.ParseValues(attributes, aa.Id);
                for (int j = 0; j < valuesStr.Count; j++)
                {
                    string valueStr = valuesStr[j];
                    string aaAttribute = "";
                    if (!aa.ShouldHaveValues())
                    {
                        //no values
                        if (aa.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            //multiline textbox
                            var attributeName = aa.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id);
                            //encode (if required)
                            if (htmlEncode)
                                attributeName = HttpUtility.HtmlEncode(attributeName);
                            aaAttribute = string.Format("{0}: {1}", attributeName, HtmlHelper.FormatText(valueStr, false, true, false, false, false, false));
                            //we never encode multiline textbox input
                        }
                        else if (aa.AttributeControlType == AttributeControlType.FileUpload)
                        {
                            //file upload
                            //not supported for address attributes
                        }
                        else
                        {
                            //other attributes (textbox, datepicker)
                            aaAttribute = string.Format("{0}: {1}", aa.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), valueStr);
                            //encode (if required)
                            if (htmlEncode)
                                aaAttribute = HttpUtility.HtmlEncode(aaAttribute);
                        }
                    }
                    else
                    {
                        int aaId;
                        if (int.TryParse(valueStr, out aaId))
                        {
                            var aaValue = _addressAttributeService.GetAddressAttributeValueById(aaId);
                            if (aaValue != null)
                            {
                                aaAttribute = string.Format("{0}: {1}", aa.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id), aaValue.GetLocalized(a => a.Name, _workContext.WorkingLanguage.Id));
                            }
                            //encode (if required)
                            if (htmlEncode)
                                aaAttribute = HttpUtility.HtmlEncode(aaAttribute);
                        }
                    }

                    if (!String.IsNullOrEmpty(aaAttribute))
                    {
                        if (i != 0 || j != 0)
                            result.Append(serapator);
                        result.Append(aaAttribute);
                    }
                }
            }

            return result.ToString();
        }
    }
}
