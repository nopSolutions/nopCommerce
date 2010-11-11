using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Web.Templates.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.Alipay
{
    public partial class HostedPaymentConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                txtSellerEmail.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Alipay.SellerEmail");
                txtKey.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Alipay.Key");
                txtPartner.Text = IoC.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Alipay.Partner");
                txtAdditionalFee.Value = IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.Alipay.AdditionalFee");
            }
        }

        public void Save()
        {
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Alipay.SellerEmail", txtSellerEmail.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Alipay.Key", txtKey.Text);
            IoC.Resolve<ISettingManager>().SetParam("PaymentMethod.Alipay.Partner", txtPartner.Text);
            IoC.Resolve<ISettingManager>().SetParamNative("PaymentMethod.Alipay.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}