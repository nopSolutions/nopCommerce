using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.Web.Templates.Payment;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Web.Administration.Payment.Alipay
{
    public partial class HostedPaymentConfig : BaseNopAdministrationUserControl, IConfigurePaymentMethodModule
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                txtSellerEmail.Text = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Alipay.SellerEmail");
                txtKey.Text = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Alipay.Key");
                txtPartner.Text = IoCFactory.Resolve<ISettingManager>().GetSettingValue("PaymentMethod.Alipay.Partner");
                txtAdditionalFee.Value = IoCFactory.Resolve<ISettingManager>().GetSettingValueDecimalNative("PaymentMethod.Alipay.AdditionalFee");
            }
        }

        public void Save()
        {
            IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Alipay.SellerEmail", txtSellerEmail.Text);
            IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Alipay.Key", txtKey.Text);
            IoCFactory.Resolve<ISettingManager>().SetParam("PaymentMethod.Alipay.Partner", txtPartner.Text);
            IoCFactory.Resolve<ISettingManager>().SetParamNative("PaymentMethod.Alipay.AdditionalFee", txtAdditionalFee.Value);
        }
    }
}