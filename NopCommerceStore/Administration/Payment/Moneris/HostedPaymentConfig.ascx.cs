using System;
using System.Web.UI;
using NopSolutions.NopCommerce.Payment.Methods.Moneris;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.Moneris
{
    public partial class HostedPaymentConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        #region Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtHppId.Text = HostedPaymentSettings.HppId;
                txtGatewayUrl.Text = HostedPaymentSettings.GatewayUrl;
                txtHppKey.Text = HostedPaymentSettings.HppKey;
                cbAuthorizeOnly.Checked = HostedPaymentSettings.AuthorizeOnly;
                txtAdditionalFee.Value = HostedPaymentSettings.AdditionalFee;
            }
        }

        public void Save()
        {
            HostedPaymentSettings.HppId = txtHppId.Text;
            HostedPaymentSettings.GatewayUrl = txtGatewayUrl.Text;
            HostedPaymentSettings.HppKey = txtHppKey.Text;
            HostedPaymentSettings.AuthorizeOnly = cbAuthorizeOnly.Checked;
            HostedPaymentSettings.AdditionalFee = txtAdditionalFee.Value;
        }
        #endregion
    }
}