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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.ExportImport;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Xml;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class CategoriesControl : BaseNopAdministrationUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
                BindGrid();
            }
        }

        private void BindData()
        {
        }

        private void BindGrid()
        {
            var categories = GetAllCategories(0);
            gvCategories.DataSource = categories;
            gvCategories.DataBind();
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
                    result = category.Name + " >> " + result;
                }
                category = category.ParentCategory;
            }
            return Server.HtmlEncode(result);
        }

        protected void gvCategories_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategories.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected List<Category> GetAllCategories(int forParentEntityId)
        {
            var result = new List<Category>();
            var categories = CategoryManager.GetAllCategories(forParentEntityId);
            foreach (var category in categories)
            {
                result.Add(category);
                result.AddRange(GetAllCategories(category.CategoryId));
            }
            return result;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    foreach (GridViewRow row in gvCategories.Rows)
                    {
                        var hfCategoryId = row.FindControl("hfCategoryId") as HiddenField;
                        var txtDisplayOrder = row.FindControl("txtDisplayOrder") as NumericTextBox;
                        var cbPublished = row.FindControl("cbPublished") as CheckBox;
                        int categoryId = int.Parse(hfCategoryId.Value);
                        Category category = CategoryManager.GetCategoryById(categoryId);

                        if (category != null)
                        {
                            category = CategoryManager.UpdateCategory(category.CategoryId, category.Name, category.Description, category.TemplateId,
                                 category.MetaKeywords, category.MetaDescription, category.MetaTitle, category.SEName, category.ParentCategoryId,
                                category.PictureId, category.PageSize, category.PriceRanges, category.ShowOnHomePage, cbPublished.Checked, category.Deleted,
                                txtDisplayOrder.Value, category.CreatedOn, DateTime.UtcNow);
                        }
                    }
                    ShowMessage(GetLocaleResourceString("Admin.Categories.ChangesSuccessfullySaved"));
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void btnExportXML_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string fileName = string.Format("categories_{0}.xml", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
                    string xml = ExportManager.ExportCategoriesToXml();
                    CommonHelper.WriteResponseXml(xml, fileName);
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }
    }
}