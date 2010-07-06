using System;
using System.Web.UI;
using NopSolutions.NopCommerce.Payment.Methods.Amazon;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.Amazon
{
    public partial class SimplePayConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        #region Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtAccountId.Text = SimplePaySettings.AccountId;
                txtGatewayUrl.Text = SimplePaySettings.GatewayUrl;
                cbSettleImmediately.Checked = SimplePaySettings.SettleImmediately;
                txtAccessKey.Text = SimplePaySettings.AccessKey;
                txtSecretKey.Text = SimplePaySettings.SecretKey;
                txtAdditionalFee.Value = SimplePaySettings.AdditionalFee;
            }
        }

        public void Save()
        {
            SimplePaySettings.AccountId = txtAccountId.Text;
            SimplePaySettings.GatewayUrl = txtGatewayUrl.Text;
            SimplePaySettings.SettleImmediately = cbSettleImmediately.Checked;
            SimplePaySettings.AccessKey = txtAccessKey.Text;
            SimplePaySettings.SecretKey = txtSecretKey.Text;
            SimplePaySettings.AdditionalFee = txtAdditionalFee.Value;
        }
        #endregion
    }
}