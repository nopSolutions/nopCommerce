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

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NopSolutions.NopCommerce.Controls
{
    public class ImageCheckBox : CheckBox
    {
        protected override void Render(HtmlTextWriter output)
        {
            var image = new Image();
            image.ID = this.ClientID + "_Image";
            image.AlternateText = this.Checked.ToString();
            if (!this.ShowCheckBox)
            {
                base.Style.Add("display", "none");
            }
            if (this.Checked)
            {
                image.ImageUrl = this.ImageChecked;
            }
            else
            {
                image.ImageUrl = this.ImageUnchecked;
            }
            image.RenderControl(output);
            base.Render(output);
        }

        [Bindable(true), DefaultValue(0), Description("Specifies Image for the Checked Image"), Category("Appearance")]
        public string ImageChecked
        {
            get
            {
                string str = (string)this.ViewState["ImageChecked"];
                if (str == null)
                {
                    return "~/images/checked.gif";
                }
                return str;
            }
            set
            {
                this.ViewState["ImageChecked"] = value;
            }
        }

        [Category("Appearance"), Description("Specifies Image for the Checked Image"), Bindable(true), DefaultValue(0)]
        public string ImageUnchecked
        {
            get
            {
                string str = (string)this.ViewState["ImageUnchecked"];
                if (str == null)
                {
                    return "~/images/unchecked.gif";
                }
                return str;
            }
            set
            {
                this.ViewState["ImageUnchecked"] = value;
            }
        }

        [Bindable(true), Category("Appearance"), DefaultValue(0), Description("Show or hide the checkbox")]
        public bool ShowCheckBox
        {
            get
            {
                if (this.ViewState["ShowCheckBox"] == null)
                {
                    return false;
                }
                return (bool)this.ViewState["ShowCheckBox"];
            }
            set
            {
                this.ViewState["ShowCheckBox"] = value;
            }
        }
    }
}