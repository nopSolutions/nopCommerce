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
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.Common.Utils;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerRoleInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            CustomerRole customerRole = CustomerManager.GetCustomerRoleById(this.CustomerRoleId);
            if (customerRole != null)
            {
                this.txtName.Text = customerRole.Name;
                this.cbFreeShipping.Checked = customerRole.FreeShipping;
                this.cbTaxExempt.Checked = customerRole.TaxExempt;
                this.cbActive.Checked = customerRole.Active;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public CustomerRole SaveInfo()
        {
            CustomerRole customerRole = CustomerManager.GetCustomerRoleById(this.CustomerRoleId);

            if (customerRole != null)
            {
                customerRole = CustomerManager.UpdateCustomerRole(customerRole.CustomerRoleId,
                    txtName.Text, cbFreeShipping.Checked, cbTaxExempt.Checked, cbActive.Checked, customerRole.Deleted);
            }
            else
            {
                 customerRole = CustomerManager.InsertCustomerRole(txtName.Text,
                     cbFreeShipping.Checked, cbTaxExempt.Checked, cbActive.Checked, false);

            }
            return customerRole;
        }

        public int CustomerRoleId
        {
            get
            {
                return CommonHelper.QueryStringInt("CustomerRoleId");
            }
        }
    }
}