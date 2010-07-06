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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using System.ComponentModel;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CustomerStatisticsControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int days = Convert.ToInt32(ddlDays.SelectedValue.ToString());

            lblCustomers.Text = CustomerManager.GetRegisteredCustomersReport(days).ToString();
            lnkViewCustomers.NavigateUrl = "~/Administration/Customers.aspx?ShowDays=" + days;
        }

        [DefaultValue(false)]
        public bool DisplayTitle
        {
            get
            {
                object obj2 = this.ViewState["DisplayTitle"];
                return ((obj2 != null) && ((bool)obj2));
            }
            set
            {
                this.ViewState["DisplayTitle"] = value;
            }
        }
    }
}