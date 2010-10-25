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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Templates.Payment;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.PayPalStandard
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
            cbUseSandbox.Checked = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.PaypalStandard.UseSandbox");
            txtBusinessEmail.Text = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PaypalStandard.BusinessEmail");
            txtPTIIdentityToken.Text = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.PaypalStandard.PTIIdentityToken");
            cbPassProductNamesAndTotals.Checked = IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("PaymentMethod.PaypalStandard.PassProductNamesAndTotals");
            txtAdditionalFee.Value = IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.PaypalStandard.AdditionalFee");
        }

        public void Save()
        {
            IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.PaypalStandard.UseSandbox", cbUseSandbox.Checked.ToString());
            IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.PaypalStandard.BusinessEmail", txtBusinessEmail.Text);
            IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.PaypalStandard.PTIIdentityToken", txtPTIIdentityToken.Text);
            IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.PaypalStandard.PassProductNamesAndTotals", cbPassProductNamesAndTotals.Checked.ToString());
            IoCFactory.Resolve<ISettingManager>().SetParamNative("PaymentMethod.PaypalStandard.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}
