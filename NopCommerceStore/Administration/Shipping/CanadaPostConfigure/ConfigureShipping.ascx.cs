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
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Administration.Modules;
using NopSolutions.NopCommerce.Web.Templates.Shipping;

namespace NopSolutions.NopCommerce.Web.Administration.Shipping.CanadaPostConfigure
{
    public partial class ConfigureShipping : BaseNopAdministrationUserControl, IConfigureShippingRateComputationMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            txtCustomerId.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.CanadaPost.CustomerId");
            txtURL.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.CanadaPost.URL");
            txtPort.Text = SettingManager.GetSettingValue("ShippingRateComputationMethod.CanadaPost.Port");
        }

        public void Save()
        {
            SettingManager.SetParam("ShippingRateComputationMethod.CanadaPost.CustomerId", txtCustomerId.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.CanadaPost.URL", txtURL.Text);
            SettingManager.SetParam("ShippingRateComputationMethod.CanadaPost.Port", txtPort.Text);
        }
    }
}
