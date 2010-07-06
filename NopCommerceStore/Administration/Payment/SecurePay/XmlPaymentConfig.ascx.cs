using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.Payment.Methods.SecurePay;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.SecurePay
{
    public partial class XmlPaymentConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtMerchantId.Text = SecurePaySettings.MerchantId;
                txtMerchantPassword.Text = SecurePaySettings.MerchantPassword;
                cbTestMode.Checked = XmlPaymentSettings.TestMode;
                cbAuthorizeOnly.Checked = XmlPaymentSettings.AuthorizeOnly;
                txtAdditionalFee.Value = SecurePaySettings.AdditionalFee;
            }
        }

        public void Save()
        {
            SecurePaySettings.MerchantId = txtMerchantId.Text;
            SecurePaySettings.MerchantPassword = txtMerchantPassword.Text;
            XmlPaymentSettings.AuthorizeOnly = cbAuthorizeOnly.Checked;
            XmlPaymentSettings.TestMode = cbTestMode.Checked;
            SecurePaySettings.AdditionalFee = txtAdditionalFee.Value;
        }
    }
}