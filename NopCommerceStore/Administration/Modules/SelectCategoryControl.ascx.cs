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
using System.ComponentModel;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
 
namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class SelectCategoryControl : BaseNopAdministrationUserControl
    {
        private int selectedCategoryId;

        public void BindData()
        {
            ddlCategories.Items.Clear();
            ddlCategories.Items.Add(new ListItem(this.EmptyItemText, "0"));
            BindData(0, "--");
        }

        public void BindData(int forParentEntityId, string prefix)
        {
            var categoryCollection = CategoryManager.GetAllCategories(forParentEntityId);

            foreach (Category category in categoryCollection)
            {
                ListItem item = new ListItem(prefix + category.Name, category.CategoryId.ToString());
                this.ddlCategories.Items.Add(item);
                if (category.CategoryId == this.selectedCategoryId)
                    item.Selected = true;
                if (CategoryManager.GetAllCategories(category.CategoryId).Count > 0)
                    BindData(category.CategoryId, prefix + "--");
            }

            this.ddlCategories.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        public string CssClass
        {
            get
            {
                return ddlCategories.CssClass;
            }
            set
            {
                ddlCategories.CssClass = value;
            }
        }

        public int SelectedCategoryId
        {
            get
            {
                return int.Parse(this.ddlCategories.SelectedItem.Value);
            }
            set
            {
                this.selectedCategoryId = value;
            }
        }
        
        public string EmptyItemText
        {
            get
            {
                if (ViewState["EmptyItemText"] == null)
                    return "[ --- ]";
                else
                    return (string)ViewState["EmptyItemText"];
            }
            set { ViewState["EmptyItemText"] = value; }
        }
    }
}