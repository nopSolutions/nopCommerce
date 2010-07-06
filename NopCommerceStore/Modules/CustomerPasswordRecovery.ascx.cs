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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.Common.Utils;
 

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class CustomerPasswordRecoveryControl : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {
            var passwordRecoveryToken = CommonHelper.QueryStringGuid("PRT");
            if (!passwordRecoveryToken.HasValue)
            {
                pnlResult.Visible = false;
                pnlRecover.Visible = true;
                pnlNewPassword.Visible = false;
            }
            else
            {
                string email = CommonHelper.QueryString("Email");
                var customer = CustomerManager.GetCustomerByEmail(email);
                if (customer != null)
                {
                    if (customer.PasswordRecoveryToken.ToLower() == passwordRecoveryToken.Value.ToString().ToLower())
                    {
                        pnlResult.Visible = false;
                        pnlRecover.Visible = false;
                        pnlNewPassword.Visible = true;
                    }
                    else
                        Response.Redirect(CommonHelper.GetStoreLocation());
                }
                else
                    Response.Redirect(CommonHelper.GetStoreLocation());
            }
        }

        protected void btnPasswordRecovery_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var customer = CustomerManager.GetCustomerByEmail(txtEmail.Text);
                    if (customer != null && !customer.IsGuest)
                    {
                        var passwordRecoveryToken = Guid.NewGuid();
                        customer.PasswordRecoveryToken = passwordRecoveryToken.ToString();
                        MessageManager.SendCustomerPasswordRecoveryMessage(customer, NopContext.Current.WorkingLanguage.LanguageId);

                        lResult.Text = GetLocaleResourceString("Account.PasswordRecovery.EmailHasBeenSent");
                        pnlResult.Visible = true;
                        pnlRecover.Visible = false;
                        pnlNewPassword.Visible = false;
                    }
                    else
                    {
                        lResult.Text = GetLocaleResourceString("Account.PasswordRecovery.EmailNotFound");
                        pnlResult.Visible = true;
                        pnlRecover.Visible = true;
                        pnlNewPassword.Visible = false;
                    }
                }
                catch (Exception exc)
                {
                    LogManager.InsertLog(LogTypeEnum.MailError, string.Format("Error sending \"Password recovery\" email."), exc);

                    lResult.Text = exc.Message;
                    pnlResult.Visible = true;
                    pnlRecover.Visible = false;
                    pnlNewPassword.Visible = false;
                }
            }
        }

        protected void btnNewPassword_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var passwordRecoveryToken = CommonHelper.QueryStringGuid("PRT");
                    string email = CommonHelper.QueryString("Email");
                    if (passwordRecoveryToken.HasValue && !String.IsNullOrEmpty(email))
                    {
                        var customer = CustomerManager.GetCustomerByEmail(email);
                        if (customer != null)
                        {
                            if (customer.PasswordRecoveryToken.ToLower() == passwordRecoveryToken.Value.ToString().ToLower())
                            {
                                CustomerManager.ModifyPassword(email, txtNewPassword.Text);
                                customer.PasswordRecoveryToken = string.Empty;

                                pnlResult.Visible = true;
                                pnlRecover.Visible = false;
                                pnlNewPassword.Visible = false;

                                lResult.Text = GetLocaleResourceString("Account.PasswordHasBeenChanged");
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    lResult.Text = exc.Message;

                    pnlResult.Visible = true;
                    pnlRecover.Visible = false;
                    pnlNewPassword.Visible = false;

                    LogManager.InsertLog(LogTypeEnum.MailError, string.Format("Error recovering password."), exc);
                }
            }
        }
    }
}