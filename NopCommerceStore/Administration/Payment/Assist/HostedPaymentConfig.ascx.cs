using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.Payment.Methods.Assist;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.Assist
{
    public partial class HostedPaymentConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtShopId.Text = HostedPaymentSettings.ShopId;
                txtGatewayUrl.Text = HostedPaymentSettings.GatewayUrl;
                cbAuthorizeOnly.Checked = HostedPaymentSettings.AuthorizeOnly;
                cbTestMode.Checked = HostedPaymentSettings.TestMode;
                txtAdditionalFee.Value = HostedPaymentSettings.AdditionalFee;
            }
        }

        public void Save()
        {
            HostedPaymentSettings.ShopId = txtShopId.Text;
            HostedPaymentSettings.GatewayUrl = txtGatewayUrl.Text;
            HostedPaymentSettings.AuthorizeOnly = cbAuthorizeOnly.Checked;
            HostedPaymentSettings.TestMode = cbTestMode.Checked;
            HostedPaymentSettings.AdditionalFee = txtAdditionalFee.Value;
        }
    }
}