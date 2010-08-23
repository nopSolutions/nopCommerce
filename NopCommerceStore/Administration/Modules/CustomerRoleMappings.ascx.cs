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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerRoleMappingsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            List<int> _customerRoleIds = new List<int>();

            Customer customer = CustomerManager.GetCustomerById(this.CustomerId);
            if (customer != null)
            {
                var customerRoles = customer.CustomerRoles;
                foreach (CustomerRole customerRole in customerRoles)
                    _customerRoleIds.Add(customerRole.CustomerRoleId);
            }

            CustomerRoleMappingControl.SelectedCustomerRoleIds = _customerRoleIds;
            CustomerRoleMappingControl.BindData();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {           
                this.BindData();
            }
        }

        public void SaveInfo()
        {
            SaveInfo(this.CustomerId);
        }

        public void SaveInfo(int cusId)
        {
            Customer customer = CustomerManager.GetCustomerById(cusId);

            if (customer != null)
            {
                List<int> selectedCustomerRoleIds = this.CustomerRoleMappingControl.SelectedCustomerRoleIds;
                var existingCustomerRoles = customer.CustomerRoles;

                var allCustomerRoles = CustomerManager.GetAllCustomerRoles();
                foreach (CustomerRole customerRole in allCustomerRoles)
                {
                    if (selectedCustomerRoleIds.Contains(customerRole.CustomerRoleId))
                    {
                        if (existingCustomerRoles.Find(cr => cr.CustomerRoleId == customerRole.CustomerRoleId) == null)
                        {
                            CustomerManager.AddCustomerToRole(customer.CustomerId, customerRole.CustomerRoleId);
                        }
                    }
                    else
                    {
                        if (existingCustomerRoles.Find(cr => cr.CustomerRoleId == customerRole.CustomerRoleId) != null)
                        {
                            CustomerManager.RemoveCustomerFromRole(customer.CustomerId, customerRole.CustomerRoleId);
                        }
                    }
                }
            }
        }

        public int CustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("CustomerId");
            }
        }
    }
}