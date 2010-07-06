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
    public partial class CustomerAccountActivationControl : BaseNopUserControl
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
            var accountActivationToken = CommonHelper.QueryStringGuid("ACT");
            if (!accountActivationToken.HasValue)
            {
                Response.Redirect(CommonHelper.GetStoreLocation());
            }
            else
            {
                string email = CommonHelper.QueryString("Email");
                var customer = CustomerManager.GetCustomerByEmail(email);
                if (customer != null)
                {
                    if (customer.AccountActivationToken.ToLower() == accountActivationToken.Value.ToString().ToLower())
                    {
                        CustomerManager.Activate(customer.CustomerId, true);
                        customer.AccountActivationToken = string.Empty;
                        lResult.Text = GetLocaleResourceString("Account.AccountHasBeenActivated");
                    }
                    else
                    {
                        Response.Redirect(CommonHelper.GetStoreLocation());
                    }
                }
                else
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
            }
        }
    }
}