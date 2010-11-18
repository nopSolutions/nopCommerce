//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerLoginControl: BaseNopFrontendUserControl
    {
        private void ApplyLocalization()
        {
            var lblUsernameOrEmail = LoginForm.FindControl("lblUsernameOrEmail") as Label;
            if (lblUsernameOrEmail != null)
            {
                if (IoC.Resolve<ICustomerService>().UsernamesEnabled)
                {
                    lblUsernameOrEmail.Text = GetLocaleResourceString("Login.Username");
                }
                else
                {
                    lblUsernameOrEmail.Text = GetLocaleResourceString("Login.E-MailAddress");
                }
            }
            var UserNameOrEmailRequired = LoginForm.FindControl("UserNameOrEmailRequired") as RequiredFieldValidator;
            if (UserNameOrEmailRequired != null)
            {
                if (IoC.Resolve<ICustomerService>().UsernamesEnabled)
                {
                    UserNameOrEmailRequired.ErrorMessage = GetLocaleResourceString("Login.UserNameRequired");
                    UserNameOrEmailRequired.ToolTip = GetLocaleResourceString("Login.UserNameRequired");
                }
                else
                {
                    UserNameOrEmailRequired.ErrorMessage = GetLocaleResourceString("Login.E-MailRequired");
                    UserNameOrEmailRequired.ToolTip = GetLocaleResourceString("Login.E-MailRequired");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();

            var CaptchaCtrl = LoginForm.FindControl("CaptchaCtrl") as CaptchaControl;
            if (CaptchaCtrl != null)
            {
                CaptchaCtrl.Visible = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.LoginCaptchaImageEnabled");
            }

            if(IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.LoginCaptchaImageEnabled"))
            {
                pnlLogin.CssClass = "login-block captcha-enabled";
                pnlRegisterBlock.Attributes["class"] = "register-block captcha-enabled";
            }
        }

        protected void OnLoggingIn(object sender, LoginCancelEventArgs e)
        {
            if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.LoginCaptchaImageEnabled"))
            {
                var CaptchaCtrl = LoginForm.FindControl("CaptchaCtrl") as CaptchaControl;
                if (CaptchaCtrl != null)
                {
                    if (!CaptchaCtrl.ValidateCaptcha())
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        protected void OnLoggedIn(object sender, EventArgs e)
        {
            //string str = string.Empty;
            //str = Page.Request.QueryString["ReturnUrl"];
            //if (string.IsNullOrEmpty(str))
            //    str = "~/default.aspx";

            //this.LoginForm.DestinationPageUrl = str;
        }

        protected void OnLoginError(object sender, EventArgs e)
        {
            if (IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Common.LoginCaptchaImageEnabled"))
            {
                var CaptchaCtrl = LoginForm.FindControl("CaptchaCtrl") as CaptchaControl;
                if (CaptchaCtrl != null)
                {
                    CaptchaCtrl.RegenerateCode();
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string url = CommonHelper.GetStoreLocation() + "register.aspx";
            string returnUrl = CommonHelper.QueryString("ReturnUrl");
            if (!String.IsNullOrEmpty(returnUrl))
            {
                url = CommonHelper.ModifyQueryString(url, "ReturnUrl=" + returnUrl, null);
            }
            Response.Redirect(url);
        }

        protected void btnCheckoutAsGuest_Click(object sender, EventArgs e)
        {
            string url = CommonHelper.GetStoreLocation() + "checkout.aspx";
            Response.Redirect(url);
        }

        public bool CheckoutAsGuestQuestion
        {
            get
            {
                bool checkoutAsGuest = CommonHelper.QueryStringBool("CheckoutAsGuest");
                return checkoutAsGuest && IoC.Resolve<ICustomerService>().AnonymousCheckoutAllowed;
            }
        }
    }
}