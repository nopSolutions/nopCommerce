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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.Common.Utils;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerRoleDetailsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.SelectTab(this.CustomerRoleTabs, this.TabId);
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    CustomerRole customerRole = ctrlCustomerRoleInfo.SaveInfo();
                    ctrlCustomerRoleCustomers.SaveInfo();

                    CustomerActivityManager.InsertActivity(
                        "EditCustomerRole",
                        GetLocaleResourceString("ActivityLog.EditCustomerRole"),
                        customerRole.Name);

                    Response.Redirect(string.Format("CustomerRoleDetails.aspx?CustomerRoleID={0}&TabID={1}", customerRole.CustomerRoleId, this.GetActiveTabId(this.CustomerRoleTabs)));
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                CustomerRole customerRole = CustomerManager.GetCustomerRoleById(this.CustomerRoleId);
                if (customerRole != null)
                {
                    CustomerManager.MarkCustomerRoleAsDeleted(this.CustomerRoleId);

                    CustomerActivityManager.InsertActivity(
                        "DeleteCustomerRole",
                        GetLocaleResourceString("ActivityLog.DeleteCustomerRole"),
                        customerRole.Name);
                }

                Response.Redirect("CustomerRoles.aspx");
            }
            catch (Exception exc)
            {
                ProcessException(exc);
            }
        }

        public int CustomerRoleId
        {
            get
            {
                return CommonHelper.QueryStringInt("CustomerRoleId");
            }
        }

        protected string TabId
        {
            get
            {
                return CommonHelper.QueryString("TabId");
            }
        }
    }
}