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
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NopSolutions.NopCommerce.Controls
{
    /// <summary>
    /// Global radio button control
    /// </summary>
    [ToolboxData("<{0}:GlobalRadioButton runat=server></{0}:GlobalRadioButton>")]
    public partial class GlobalRadioButton : RadioButton, IPostBackDataHandler
    {
        private string Value
        {
            get
            {
                string val = Attributes["value"];
                if (val == null)
                    val = UniqueID;
                else
                    val = UniqueID + "_" + val;
                return val;
            }
        }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="output">A System.Web.UI..::.HtmlTextWriter that contains the output stream to render on the client.</param>
        protected override void Render(HtmlTextWriter output)
        {
            RenderInputTag(output);
        }

        private void RenderInputTag(HtmlTextWriter htw)
        {
            htw.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            htw.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            htw.AddAttribute(HtmlTextWriterAttribute.Name, GroupName);
            htw.AddAttribute(HtmlTextWriterAttribute.Value, Value);
            if (Checked)
                htw.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            if (!Enabled)
                htw.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");

            string onClick = Attributes["onclick"];
            if (AutoPostBack)
            {
                if (onClick != null)
                    onClick = String.Empty;
                onClick += Page.GetPostBackClientEvent(this, String.Empty);
                htw.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
                htw.AddAttribute("language", "javascript");
            }
            else
            {
                if (onClick != null)
                    htw.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
            }

            if (AccessKey.Length > 0)
                htw.AddAttribute(HtmlTextWriterAttribute.Accesskey, AccessKey);
            if (TabIndex != 0)
                htw.AddAttribute(HtmlTextWriterAttribute.Tabindex,
                    TabIndex.ToString(NumberFormatInfo.InvariantInfo));
            htw.RenderBeginTag(HtmlTextWriterTag.Input);
            htw.RenderEndTag();
        }

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            OnCheckedChanged(EventArgs.Empty);
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey,
            System.Collections.Specialized.NameValueCollection postCollection)
        {
            bool result = false;
            string value = postCollection[GroupName];
            if ((value != null) && (value == Value))
            {
                if (!Checked)
                {
                    Checked = true;
                    result = true;
                }
            }
            else
            {
                if (Checked)
                    Checked = false;
            }
            return result;
        }
    }
}