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
using NopSolutions.NopCommerce.Payment.Methods.iDeal;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.iDeal
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
            txtMerchantId.Text = iDealBasicPaymentSettings.MerchantId;
            txtSubId.Text = iDealBasicPaymentSettings.SubId;
            txtHashKey.Text = iDealBasicPaymentSettings.HashKey;
            txtUrl.Text = iDealBasicPaymentSettings.Url;
            txtAdditionalFee.Value = iDealBasicPaymentSettings.AdditionalFee;
        }

        public void Save()
        {
            iDealBasicPaymentSettings.MerchantId = txtMerchantId.Text;
            iDealBasicPaymentSettings.SubId = txtSubId.Text;
            iDealBasicPaymentSettings.HashKey = txtHashKey.Text;
            iDealBasicPaymentSettings.Url = txtUrl.Text;
            iDealBasicPaymentSettings.AdditionalFee = txtAdditionalFee.Value;
        }
    }
}
