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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Web.Templates.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.eWayUK
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
            txtCustomerId.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.eWayUK.CustomerId");
            txtUsername.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.eWayUK.Username");
            txtPaymentPage.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.eWayUK.PaymentPage");
            txtAdditionalFee.Value = IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.eWayUK.AdditionalFee");
        }

        public void Save()
        {
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.eWayUK.CustomerId", txtCustomerId.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.eWayUK.Username", txtUsername.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.eWayUK.PaymentPage", txtPaymentPage.Text);
            IoC.Resolve<ISettingManager>().SetParamNative("PaymentMethod.eWayUK.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}
