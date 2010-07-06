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
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class ConfirmationBox : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            lConfirmText.Text = this.ConfirmText;
        }

        [DefaultValue("Are you sure?")]
        public string ConfirmText
        {
            get
            {
                object obj2 = this.ViewState["ConfirmText"];
                if (obj2 != null)
                    return (string)obj2;
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["ConfirmText"] = value;
            }
        }
        
        public string YesText
        {
            get
            {
                return btnYES.Text;
            }
            set
            {
                btnYES.Text = value;
            }
        }

        public string NoText
        {
            get
            {
                return btnNO.Text;
            }
            set
            {
                btnNO.Text = value;
            }
        }

        public string TargetControlID
        {
            get
            {
                return cbe.TargetControlID;
            }
            set
            {
                cbe.TargetControlID = value;
                mpe.TargetControlID = value;
            }
        }
    }
}
