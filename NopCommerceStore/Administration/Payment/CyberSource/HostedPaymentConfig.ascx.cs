using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.Payment.Methods.CyberSource;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.CyberSource
{
    public partial class HostedPaymentConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtMerchantId.Text = HostedPaymentSettings.MerchantId;
                txtPublicKey.Text = HostedPaymentSettings.PublicKey;
                txtGatewayUrl.Text = HostedPaymentSettings.GatewayUrl;
                txtSerialNumber.Text = HostedPaymentSettings.SerialNumber;
                txtAdditionalFee.Value = HostedPaymentSettings.AdditionalFee;
            }
        }

        public void Save()
        {
            HostedPaymentSettings.MerchantId = txtMerchantId.Text;
            HostedPaymentSettings.PublicKey = txtPublicKey.Text;
            HostedPaymentSettings.GatewayUrl = txtGatewayUrl.Text;
            HostedPaymentSettings.AdditionalFee = txtAdditionalFee.Value;
            HostedPaymentSettings.SerialNumber = txtSerialNumber.Text;
        }
    }
}
