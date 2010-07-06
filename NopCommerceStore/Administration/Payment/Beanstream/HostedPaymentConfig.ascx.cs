using System;
using System.Web.UI;
using NopSolutions.NopCommerce.Payment.Methods.Beanstream;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.Beanstream
{
    public partial class HostedPaymentConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        #region Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtMerchantId.Text = HostedPaymentSettings.MerchantId;
                txtGatewayUrl.Text = HostedPaymentSettings.GatewayUrl;
                txtAdditionalFee.Value = HostedPaymentSettings.AdditionalFee;
            }
        }

        public void Save()
        {
            HostedPaymentSettings.MerchantId = txtMerchantId.Text;
            HostedPaymentSettings.GatewayUrl = txtGatewayUrl.Text;
            HostedPaymentSettings.AdditionalFee = txtAdditionalFee.Value;
        }
        #endregion
    }
}