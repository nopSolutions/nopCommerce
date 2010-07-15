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
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Manufacturers;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;


namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ManufacturerNavigation : BaseNopUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindData();
        }

        protected void BindData()
        {
            var manufacturers = ManufacturerManager.GetAllManufacturers();
            if (manufacturers.Count > 0)
            {
                rptrManufacturers.DataSource = manufacturers;
                rptrManufacturers.DataBind();
            }
            else
                this.Visible = false;
        }

        protected void rptrManufacturers_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var manufacturer = e.Item.DataItem as Manufacturer;
                var hlManufacturer = e.Item.FindControl("hlManufacturer") as HyperLink;
                if (hlManufacturer != null)
                {
                    hlManufacturer.NavigateUrl = SEOHelper.GetManufacturerUrl(manufacturer);
                }
            }
        }

        public int ManufacturerId
        {
            get
            {
                return CommonHelper.QueryStringInt("ManufacturerId");
            }
        }
    }


}