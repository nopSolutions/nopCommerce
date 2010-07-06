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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerChangePasswordControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            pnlChangePasswordError.Visible = false;
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    string oldPassword = txtOldPassword.Text;
                    string password = txtNewPassword.Text;
                    string confirmPassword = txtConfirmPassword.Text;

                    if (password != confirmPassword)
                    {
                        pnlChangePasswordError.Visible = true;
                        lChangePasswordErrorMessage.Text = GetLocaleResourceString("Account.EnteredPasswordsDoNotMatch");
                    }
                    else
                    {
                        CustomerManager.ModifyPassword(NopContext.Current.User.Email, oldPassword, password);
                        pnlChangePasswordError.Visible = true;
                        lChangePasswordErrorMessage.Text = GetLocaleResourceString("Account.PasswordWasChanged");
                    }
                    txtOldPassword.Text = string.Empty;
                    txtNewPassword.Text = string.Empty;
                    txtConfirmPassword.Text = string.Empty;
                }
            }
            catch (Exception exc)
            {
                pnlChangePasswordError.Visible = true;
                lChangePasswordErrorMessage.Text = Server.HtmlEncode(exc.Message);
            }
        }
    }
}