using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.Payment.Methods.USAePay;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.USAePay
{
    public partial class EPaymentFormConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtSourceKey.Text = EPaymentFormSettings.SourceKey;
                txtGatewayUrl.Text = EPaymentFormSettings.GatewayUrl;
                txtServiceUrl.Text = EPaymentFormSettings.ServiceUrl;
                cbAuthorizeOnly.Checked = EPaymentFormSettings.AuthorizeOnly;
                txtPIN.Text = EPaymentFormSettings.PIN;
                cbUsePIN.Checked = EPaymentFormSettings.UsePIN;
                txtAdditionalFee.Value = EPaymentFormSettings.AdditionalFee;
            }
        }

        public void Save()
        {
            EPaymentFormSettings.SourceKey = txtSourceKey.Text;
            EPaymentFormSettings.GatewayUrl = txtGatewayUrl.Text;
            EPaymentFormSettings.ServiceUrl = txtServiceUrl.Text;
            EPaymentFormSettings.AuthorizeOnly = cbAuthorizeOnly.Checked;
            EPaymentFormSettings.PIN = txtPIN.Text;
            EPaymentFormSettings.UsePIN = cbUsePIN.Checked;
            EPaymentFormSettings.AdditionalFee = txtAdditionalFee.Value;
        }
    }
}