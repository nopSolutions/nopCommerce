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
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.Web;
 
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class SelectDiscountsControl : BaseNopAdministrationUserControl
    {
        protected List<int> selectedDiscountIds = new List<int>();

        public void BindData(DiscountTypeEnum? DiscountType)
        {
            var discounts = DiscountManager.GetAllDiscounts(DiscountType);
            foreach (Discount discount in discounts)
            {
                ListItem item = new ListItem(discount.Name, discount.DiscountId.ToString());
                if (this.selectedDiscountIds.Contains(discount.DiscountId))
                    item.Selected = true;
                this.cblDiscounts.Items.Add(item);
            }
            this.cblDiscounts.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string CssClass
        {
            get
            {
                return cblDiscounts.CssClass;
            }
            set
            {
                cblDiscounts.CssClass = value;
            }
        }

        public List<int> SelectedDiscountIds
        {
            get
            {
                List<int> result = new List<int>();
                foreach (ListItem item in this.cblDiscounts.Items)
                    if (item.Selected)
                        result.Add(int.Parse(item.Value));
                return result;
            }
            set
            {
                this.selectedDiscountIds = value;
            }
        }
    }
}