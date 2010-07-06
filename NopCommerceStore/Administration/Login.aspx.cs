using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration
{
    public partial class LoginPage : BaseNopAdministrationPage
    {
        private void ApplyLocalization()
        {
            Literal lUsernameOrEmail = LoginForm.FindControl("lUsernameOrEmail") as Literal;
            if (lUsernameOrEmail != null)
            {
                if (CustomerManager.UsernamesEnabled)
                {
                    lUsernameOrEmail.Text = GetLocaleResourceString("Login.Username");
                }
                else
                {
                    lUsernameOrEmail.Text = GetLocaleResourceString("Login.E-MailAddress");
                }
            }
            RequiredFieldValidator UserNameOrEmailRequired = LoginForm.FindControl("UserNameOrEmailRequired") as RequiredFieldValidator;
            if (UserNameOrEmailRequired != null)
            {
                if (CustomerManager.UsernamesEnabled)
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

            CommonHelper.SetResponseNoCache(Response);

            string title = GetLocaleResourceString("PageTitle.Login");
            SEOHelper.RenderTitle(this, title, true);
        }
        
        protected override bool AdministratorSecurityValidationEnabled
        {
            get
            {
                return false;
            }
        }

        protected override bool IPAddressValidationEnabled
        {
            get
            {
                return false;
            }
        }
    }
}
