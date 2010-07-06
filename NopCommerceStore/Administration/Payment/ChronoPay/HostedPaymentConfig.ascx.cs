using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.Payment.Methods.ChronoPay;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.ChronoPay
{
    public partial class HostedPaymentConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtProductId.Text = HostedPaymentSettings.ProductId;
                txtProductName.Text = HostedPaymentSettings.ProductName;
                txtGatewayUrl.Text = HostedPaymentSettings.GatewayUrl;
                txtSharedSecrect.Text = HostedPaymentSettings.SharedSecrect;
                txtAdditionalFee.Value = HostedPaymentSettings.AdditionalFee;
            }
        }

        public void Save()
        {
            HostedPaymentSettings.ProductId = txtProductId.Text;
            HostedPaymentSettings.ProductName = txtProductName.Text;
            HostedPaymentSettings.GatewayUrl = txtGatewayUrl.Text;
            HostedPaymentSettings.AdditionalFee = txtAdditionalFee.Value;
            HostedPaymentSettings.SharedSecrect = txtSharedSecrect.Text;
        }
    }
}
