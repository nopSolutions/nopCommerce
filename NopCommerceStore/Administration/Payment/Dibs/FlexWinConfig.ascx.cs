using System;
using System.Web.UI;
using NopSolutions.NopCommerce.Payment.Methods.Dibs;
using NopSolutions.NopCommerce.Web.Templates.Payment;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.Dibs
{
    public partial class FlexWinConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                txtMerchantId.Text = FlexWinSettings.MerchantId.ToString();
                txtGatewayUrl.Text = FlexWinSettings.GatewayUrl;
                cbUseSandbox.Checked = FlexWinSettings.UseSandbox;
                txtMD5Key1.Text = FlexWinSettings.MD5Key1;
                txtMD5Key2.Text = FlexWinSettings.MD5Key2;
                txtAdditionalFee.Value = FlexWinSettings.AdditionalFee;
            }
        }

        public void Save()
        {
            FlexWinSettings.MerchantId = Int32.Parse(txtMerchantId.Text);
            FlexWinSettings.GatewayUrl = txtGatewayUrl.Text;
            FlexWinSettings.UseSandbox = cbUseSandbox.Checked;
            FlexWinSettings.MD5Key1 = txtMD5Key1.Text;
            FlexWinSettings.MD5Key2 = txtMD5Key2.Text;
            FlexWinSettings.AdditionalFee = txtAdditionalFee.Value;
        }
    }
}