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

            var categories = CategoryManager.GetAllCategories();
            foreach (var category in categories)
            {
                string catName = GetCategoryFullName(category);
                ListItem item = new ListItem(catName, category.CategoryId.ToString());
                this.ddlCategories.Items.Add(item);

                if (category.CategoryId == this.selectedCategoryId)
                    item.Selected = true;
            }

            this.ddlCategories.DataBind();
        }

        protected string GetCategoryFullName(Category category)
        {
            string result = string.Empty;

            while (category != null && !category.Deleted)
            {
                if (String.IsNullOrEmpty(result))
                {
                    result = category.Name;
                }
                else
                {
                    result = "--" + result;
                }
                category = category.ParentCategory;
            }
            return result;
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