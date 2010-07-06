using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.Payment.Methods.Svea;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.Svea
{
    public partial class HostedPaymentConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        #region Handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtUsername.Text = HostedPaymentSettings.Username;
                txtPassword.Text = HostedPaymentSettings.Password;
                txtGateway.Text = HostedPaymentSettings.GatewayUrl;
                cbUseSandbox.Checked = HostedPaymentSettings.UseSandbox;
                txtPaymentMethod.Text = HostedPaymentSettings.PaymentMethod;
                txtAdditionalFee.Value = HostedPaymentSettings.AdditionalFee;
            }
        }
        #endregion

        #region Methods
        public void Save()
        {
            HostedPaymentSettings.Username = txtUsername.Text;
            HostedPaymentSettings.Password = txtPassword.Text;
            HostedPaymentSettings.GatewayUrl = txtGateway.Text;
            HostedPaymentSettings.UseSandbox = cbUseSandbox.Checked;
            HostedPaymentSettings.PaymentMethod = txtPaymentMethod.Text;
            HostedPaymentSettings.AdditionalFee = txtAdditionalFee.Value;
        }
        #endregion
    }
}