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
using NopSolutions.NopCommerce.Shipping.Methods.AustraliaPost;

namespace NopSolutions.NopCommerce.Web.Administration.Shipping.AustraliaPostConfigure
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
            txtGatewayUrl.Text = AustraliaPostSettings.GatewayUrl;
            txtAdditionalHandlingCharge.Value = AustraliaPostSettings.AdditionalHandlingCharge;
            txtShippedFromZipPostalCode.Text = AustraliaPostSettings.ShippedFromZipPostalCode;
        }

        public void Save()
        {
            AustraliaPostSettings.ShippedFromZipPostalCode = txtShippedFromZipPostalCode.Text;
            AustraliaPostSettings.GatewayUrl = AustraliaPostSettings.GatewayUrl;
            AustraliaPostSettings.AdditionalHandlingCharge = txtAdditionalHandlingCharge.Value;
        }
    }
}
