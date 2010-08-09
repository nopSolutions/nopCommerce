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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Audit.UsersOnline;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using System.Diagnostics;
using NopSolutions.NopCommerce.BusinessLogic.Directory;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class OnlineCustomersControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
                BindGrid();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    OnlineUserManager.Enabled = cbEnabled.Checked;
                    Response.Redirect("OnlineCustomers.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void BindData()
        {
            cbEnabled.Checked = OnlineUserManager.Enabled;
        }

        protected void BindGrid()
        {
            if (OnlineUserManager.Enabled)
            {
                phOnlineStats.Visible = true;

                var registeredUsers = OnlineUserManager.GetRegisteredUsersOnline();
                var guests = OnlineUserManager.GetGuestList();
                var allUsers = OnlineUserManager.GetAllUserList();

                lblGuests.Text = guests.Count.ToString();
                gvCustomers.DataSource = allUsers;
                gvCustomers.DataBind();
            }
            else
            {
                phOnlineStats.Visible = false;
            }
        }

        protected string GetCustomerInfo(OnlineUserInfo oui)
        {
            string customerInfo = string.Empty;
            if (oui.AssociatedCustomerId.HasValue)
            {
                var customer = CustomerManager.GetCustomerById(oui.AssociatedCustomerId.Value);
                if (customer != null)
                {
                    customerInfo = string.Format("{0} (<a href=\"CustomerDetails.aspx?CustomerID={1}\">{2}</a>)", Server.HtmlEncode(customer.FullName), customer.CustomerId, Server.HtmlEncode(customer.Email));
                }
                else
                {
                    customerInfo = GetLocaleResourceString("Admin.OnlineCustomers.CustomerInfoColumn.Guest");
                }
            }
            else
            {
                customerInfo = GetLocaleResourceString("Admin.OnlineCustomers.CustomerInfoColumn.Guest");
            }

            return customerInfo;
        }

        protected string GetLocationInfo(OnlineUserInfo oui)
        {
            string result= string.Empty;
            try
            {
                string countryName = GeoCountryLookup.LookupCountryName(oui.IPAddress);
                result = Server.HtmlEncode(countryName);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.ToString());
            }
            return result;
        }

        protected string GetLastPageVisitedInfo(OnlineUserInfo oui)
        {
            string result= string.Empty;
            if (!String.IsNullOrEmpty(oui.LastPageVisited))
            {
                result = string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", oui.LastPageVisited);
            }
            return result;
        }
        
        protected void gvCustomers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCustomers.PageIndex = e.NewPageIndex;
            BindGrid();
        }

    }
}