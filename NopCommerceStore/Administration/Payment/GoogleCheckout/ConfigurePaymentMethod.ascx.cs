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
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Templates.Payment;
using NopSolutions.NopCommerce.Payment.Methods.GoogleCheckout;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.GoogleCheckout
{
    public partial class ConfigurePaymentMethod : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                BindData();
        }

        private void BindData()
        {
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

            string GoogleEnvironment = config.AppSettings.Settings["GoogleEnvironment"].Value;           
            cbUseSandbox.Checked = GoogleEnvironment == "Sandbox";
            txtGoogleVendorId.Text = config.AppSettings.Settings["GoogleMerchantID"].Value;
            txtGoogleMerchantKey.Text = config.AppSettings.Settings["GoogleMerchantKey"].Value;

            cbAuthenticateCallback.Checked = Convert.ToBoolean(config.AppSettings.Settings["GoogleAuthenticateCallback"].Value);
        }

        public void Save()
        {
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

            if (cbUseSandbox.Checked)
                config.AppSettings.Settings["GoogleEnvironment"].Value = "Sandbox";
            else
                config.AppSettings.Settings["GoogleEnvironment"].Value = "Production";
            config.AppSettings.Settings["GoogleMerchantId"].Value = txtGoogleVendorId.Text;
            config.AppSettings.Settings["GoogleMerchantKey"].Value = txtGoogleMerchantKey.Text;
            config.AppSettings.Settings["GoogleAuthenticateCallback"].Value = cbAuthenticateCallback.Checked.ToString();
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
